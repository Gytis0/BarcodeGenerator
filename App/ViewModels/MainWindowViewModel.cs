using App.Attributes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;

namespace App.ViewModels;

public partial class MainWindowViewModel : ObservableValidator
{
	[ObservableProperty]
	[IsInt(1)]
	[NotifyDataErrorInfo]
	[Required(ErrorMessage = "This field is required")]
	private string barcodeCount = "40";

	[ObservableProperty]
	[IsInt(0)]
	[NotifyDataErrorInfo]
	[Required(ErrorMessage = "This field is required")]
	private string barcodeLength = "20";

	[ObservableProperty]
	[IsInt(0)]
	[NotifyDataErrorInfo]
	[Required(ErrorMessage = "This field is required")]
	private string antiComplementaryLength = "6";

	[ObservableProperty]
	[IsInt(1, 100)]
	[NotifyDataErrorInfo]
	[Required(ErrorMessage = "This field is required")]
	private string percentage = "50";

	[ObservableProperty]
	[Required(ErrorMessage = "This field is required")]
	[NotifyDataErrorInfo]
	private string excelPath = Path.Combine(AppContext.BaseDirectory, "UMI.xlsx");

	[ObservableProperty]
	[Required(ErrorMessage = "This field is required")]
	[IsSequence]
	[NotifyDataErrorInfo]
	private string insertGenomicSequence = "";

	[ObservableProperty]
	private bool isOverwrite = false;

	[ObservableProperty]
	private bool isAppend = false;

	[ObservableProperty]
	private string status = "";

	[RelayCommand]
	private async Task Generate()
	{
		Status = "";
		ValidateAllProperties();

		if (HasErrors) return;

		try
		{
			var options = new SequenceOptions(int.Parse(BarcodeCount), int.Parse(BarcodeLength), int.Parse(AntiComplementaryLength), int.Parse(Percentage), ExcelPath);

			if (IsOverwrite)
			{
				ExcelPath = ExcelHelper.EnsureExcelFilePath(ExcelPath);
				Status = "Generating...";
				List<string> sequences = await Task.Run(() => Generator.GenerateNewSequences(options, InsertGenomicSequence));

				Status = "Creating new excel file...";
				await Task.Run(() => ExcelHelper.CreateOverwrite(sequences, ExcelPath));

				Status = "Generated and Created!";
			}
			else if (IsAppend)
			{
				if (!File.Exists(ExcelPath))
				{
					Status = "File does not exist. Can't append";
					return;
				}

				Status = "Reading...";
				List<string> existingSequences = await Task.Run(() => ExcelHelper.Read(ExcelPath));

				if (options.Count <= existingSequences.Count)
				{
					Status = string.Format("Count ({0}) is less or equal to existing sequences count ({1}). Skipping generation", options.Count, existingSequences.Count);
					return;
				}

				Status = "Generating...";
				List<string> sequences = await Task.Run(() => Generator.GenerateAndAppendSequences(options, InsertGenomicSequence, existingSequences));

				Status = "Appending...";
				await Task.Run(() => ExcelHelper.CreateAppend(sequences, ExcelPath));

				Status = "Generated and Appended!";
			}
			else
			{
				List<string> existingSequences = [];
				bool read = false;
				if (File.Exists(ExcelPath))
				{
					Status = "Reading...";
					existingSequences = await Task.Run(() => ExcelHelper.Read(ExcelPath));
					read = true;
				}

				ExcelPath = ExcelHelper.EnsureNewExcelFilePath(ExcelPath);

				Status = "Generating...";
				List<string> sequences = await Task.Run(() => Generator.GenerateNewSequences(options, InsertGenomicSequence, existingSequences));

				Status = "Creating new excel file...";
				await Task.Run(() => ExcelHelper.CreateNew(sequences, ExcelPath));

				Status = read ? "Read, Generated and Created!" : "Generated and Created!";
			}

			await Task.Delay(3000);
			Status = "";
		}
		catch (Exception ex)
		{
			Status = string.Format("Could not generate sequences: {0}", ex.Message);
		}
	}
}