namespace Core;

public class SequenceOptions(int count, int length, int antiComplementaryLength, int percentage, string excelPath)
{
	public int Count { get; set; } = count;
	public int Length { get; set; } = length;
	public int AntiComplementaryLength { get; set; } = antiComplementaryLength;
	public int Percentage { get; set; } = percentage;
	public string ExcelPath { get; set; } = excelPath;
}
