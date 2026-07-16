using App.Attributes;
using App.Models.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;

namespace App.ViewModels;

public partial class BarcodeGeneratorViewModel : ObservableValidator
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
	private string antiComplementaryLength = "7";

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
	[CustomValidation(typeof(BarcodeGeneratorViewModel), nameof(ValidateReadFilePath))]
	[NotifyDataErrorInfo]
	private string readFilePath = "";

	[ObservableProperty]
	[Required(ErrorMessage = "This field is required")]
	[IsSequence]
	[NotifyDataErrorInfo]
	private string insertGenomicSequence = "";

	[ObservableProperty]
	private SaveMode saveMode = SaveMode.Create;

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

			if (SaveMode == SaveMode.Create)
			{
				Status = "Generating...";
				List<string> sequences = await Task.Run(() => Generator.GenerateNewSequences(options, InsertGenomicSequence));

				Status = "Creating new excel file...";
				string newPath = await Task.Run(() => ExcelHelper.Create(sequences, ExcelPath));

				Status = string.Format("Generated and Created {0}", Path.GetFileName(newPath));
			
				ExcelPath = newPath;
			}
			else if (SaveMode == SaveMode.Append)
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
				string newPath = await Task.Run(() => ExcelHelper.Append(sequences, ExcelPath));

				Status = string.Format("Generated and Appended {0}", Path.GetFileName(newPath));
			}
			else if (SaveMode == SaveMode.ReadCreate)
			{
				{
					List<string> existingSequences = [];
					bool read = false;
					if (File.Exists(ReadFilePath))
					{
						Status = "Reading...";
						existingSequences = await Task.Run(() => ExcelHelper.Read(ReadFilePath));
						read = true;
					}

					Status = "Generating...";
					List<string> sequences = await Task.Run(() => Generator.GenerateNewSequences(options, InsertGenomicSequence, existingSequences));

					Status = "Creating new excel file...";
					string newPath = await Task.Run(() => ExcelHelper.Create(sequences, ExcelPath));

					Status = string.Format("{0} {1}", read ? "Read, Generated and Created" : "Couldn't read. Generated and Created", Path.GetFileName(newPath));

					ExcelPath = newPath;
				}
			}
		}
		catch (Exception ex)
		{
			Status = string.Format("Could not generate sequences: {0}", ex.Message);
		}
	}

	public static ValidationResult? ValidateReadFilePath(string? value, ValidationContext context)
	{
		var vm = (BarcodeGeneratorViewModel)context.ObjectInstance;

		if (vm.SaveMode == SaveMode.ReadCreate && string.IsNullOrWhiteSpace(value))
			return new ValidationResult("This field is required.");

		return ValidationResult.Success;
	}
}