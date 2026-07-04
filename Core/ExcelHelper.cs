using ClosedXML.Excel;

namespace Core;

public class ExcelHelper
{
	public static void Create(string[] sequences, string path)
	{
		var workbook = new XLWorkbook();
		var worksheet = workbook.Worksheets.Add("Sheet1");

		for (int i = 0; i < sequences.Length; i++)
		{
			worksheet.Cell(i + 1, 1).Value = string.Format("UMI_{0}", i + 1);
			worksheet.Cell(i + 1, 2).Value = sequences[i];
		}

		worksheet.Columns().AdjustToContents();
		workbook.SaveAs(Path.Combine(path, "UMI.xlsx"));
	}
}
