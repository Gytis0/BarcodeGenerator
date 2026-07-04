using ClosedXML.Excel;

namespace Core;

public class ExcelHelper
{
	public static void CreateOverwrite(List<string> sequences, string path)
	{
		using var workbook = File.Exists(path) ? new XLWorkbook(path) : new XLWorkbook();

		var worksheet = workbook.Worksheets.FirstOrDefault() ?? workbook.Worksheets.Add("Sheet1");

		worksheet.Columns(1, 2).Clear();

		WriteSequences(worksheet, sequences);

		if (File.Exists(path))
			workbook.Save();
		else
			workbook.SaveAs(path);
	}

	public static void CreateNew(List<string> sequences, string path)
	{
		if (File.Exists(path))
			throw new IOException(string.Format("The file '{0}' already exists.", path));

		using var workbook = new XLWorkbook();
		WriteSequences(workbook.Worksheets.Add("Sheet1"), sequences);
		workbook.SaveAs(path);
	}

	public static void CreateAppend(List<string> sequences, string path)
	{
		if (!File.Exists(path))
			throw new FileNotFoundException(string.Format("The file '{0}' does not exist.", path), path);

		using var workbook = new XLWorkbook(path);
		var worksheet = workbook.Worksheet(1);

		worksheet.Columns(1, 2).Clear();

		WriteSequences(worksheet, sequences);

		workbook.Save();
	}

	private static void WriteSequences(IXLWorksheet worksheet, List<string> sequences)
	{
		for (int i = 0; i < sequences.Count; i++)
		{
			worksheet.Cell(i + 1, 1).Value = string.Format("UMI_{0}", i + 1);
			worksheet.Cell(i + 1, 2).Value = sequences[i];
		}

		worksheet.Columns(1, 2).Style.Font.FontName = "Consolas";
		worksheet.Columns(1, 2).AdjustToContents();
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

	public static string EnsureNewExcelFilePath(string path)
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

	public static string EnsureExcelFilePath(string path)
	{
		if (Path.GetExtension(path).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
			return path;

		return Path.Combine(path, "UMI.xlsx");
	}
}
