using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using UI.Attributes;

namespace UI.ViewModels;

public partial class MainWindowViewModel : ObservableValidator
{
	[ObservableProperty]
	[IsInt(1)]
	[NotifyDataErrorInfo]
	[Required]
	private string barcodeCount = "40";

	[ObservableProperty]
	[IsInt(0)]
	[NotifyDataErrorInfo]
	[Required]
	private string basesLength = "20";

	[ObservableProperty]
	[IsInt(0)]
	[NotifyDataErrorInfo]
	[Required]
	private string antiComplementaryLength = "6";

	[ObservableProperty]
	[IsInt(1, 100)]
	[NotifyDataErrorInfo]
	[Required]
	private string percentage = "50";

	[ObservableProperty]
	private string excelPath = "";

	[ObservableProperty]
	private string folderPath = AppContext.BaseDirectory;

	[ObservableProperty]
	private string status = "";

	[RelayCommand]
	public async Task PickFolder(Window window)
	{
		var result = await window.StorageProvider.OpenFolderPickerAsync(new Avalonia.Platform.Storage.FolderPickerOpenOptions { AllowMultiple = false });
		if (result != null && result.Count > 0)
		{
			FolderPath = result[0].Path.LocalPath;
		}
	}

	[RelayCommand]
	private async Task Generate()
	{
		Status = "";
		ValidateAllProperties();

		if (HasErrors) return;

		try
		{
			var options = new SequenceOptions(int.Parse(BarcodeCount), int.Parse(BasesLength), int.Parse(AntiComplementaryLength), int.Parse(Percentage), FolderPath);

			Status = "Generating...";
			string[] sequences = await Task.Run(() => Generator.GenerateSequences(options));

			Status = "Saving...";
			await Task.Run(() => ExcelHelper.Create(sequences, FolderPath));

			Status = "Generated and saved!";

			await Task.Delay(3000);
			Status = "";
		}
		catch (Exception ex)
		{
			Status = string.Format("Could not generate sequences: {0}", ex.Message);
		}
	}
}