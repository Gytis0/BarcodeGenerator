using ClosedXML.Excel;

namespace Core;

public static class ExcelHelper
{
	public static string Create(List<string> sequences, string path)
	{
		path = EnsureNewExcelFilePath(path);

		using var workbook = new XLWorkbook();
		WriteSequences(workbook.Worksheets.Add("Sheet1"), sequences, 1);
		workbook.SaveAs(path);

		return path;
	}

	public static string Append(List<string> sequences, string path)
	{
		if (!File.Exists(path))
			return Create(sequences, path);

		using var workbook = new XLWorkbook(path);
		var worksheet = workbook.Worksheet(1);

		int row = 1;

		WriteSequences(worksheet, sequences, row);

		workbook.SaveAs(path);

		return path;
	}

	public static List<string> Read(string path)
	{
		using var workbook = new XLWorkbook(path);
		var worksheet = workbook.Worksheet(1);

		List<string> sequences = [];

		int row = 1;

		while (!worksheet.Cell(row, 2).IsEmpty())
		{
			sequences.Add(worksheet.Cell(row, 2).GetString());
			row++;
		}

		return sequences;
	}

	private static void WriteSequences(IXLWorksheet worksheet, List<string> sequences, int startRow)
	{
		int row = startRow;

		for (int i = 0; i < sequences.Count; i++)
		{
			worksheet.Cell(row, 1).Value = string.Format("UMI_{0}", row);
			worksheet.Cell(row, 2).Value = sequences[i];
			row++;
		}

		worksheet.Columns(1, 2).Style.Font.FontName = "Consolas";
		worksheet.Columns(1, 2).AdjustToContents();
	}

	private static string EnsureNewExcelFilePath(string path)
	{
		string directory;
		string fileNameWithoutExtension;

		if (Path.GetExtension(path).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
		{
			directory = Path.GetDirectoryName(path)!;
			fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
		}
		else
		{
			directory = path;
			fileNameWithoutExtension = "UMI";
		}

		var match = System.Text.RegularExpressions.Regex.Match(fileNameWithoutExtension, @"^(.*)\(\d+\)$");

		if (match.Success)
			fileNameWithoutExtension = match.Groups[1].Value;

		string filePath = Path.Combine(directory, string.Format("{0}.xlsx", fileNameWithoutExtension));

		if (!File.Exists(filePath))
			return filePath;

		int index = 1;

		do
		{
			filePath = Path.Combine(directory, string.Format("{0}({1}).xlsx", fileNameWithoutExtension, index));
			index++;
		}
		while (File.Exists(filePath));

		return filePath;
	}
}