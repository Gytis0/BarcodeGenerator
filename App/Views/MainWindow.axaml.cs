using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using System.IO;
using System.Linq;

namespace App.Views;

public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
	}

	private void OnTextChanged_ClearStrings(object? sender, TextChangedEventArgs e)
	{
		if (sender is not TextBox tb)
			return;

		tb.Text = new string(tb.Text.Where(char.IsDigit).ToArray());
		tb.CaretIndex = tb.Text.Length;
	}

	private void OnCheckBoxTick_CheckLogic(object? sender, RoutedEventArgs e)
	{
		if (CheckBox_Overwrite.IsChecked!.Value)
		{
			CheckBox_Append.IsEnabled = false;
			Button_Generate.Content = "Generate and Create";
			ToolTip.SetTip(Button_Generate, "Generate new sequences and save, overwriting and ignoring any existing excel file with the same name.");
		}
		else
		{
			CheckBox_Append.IsEnabled = true;
			if (CheckBox_Append.IsChecked!.Value)
			{
				Button_Generate.Content = "Read, Generate and Append";
				ToolTip.SetTip(Button_Generate, "Read this excel first. Then Generate new sequences that would not duplicate with the existing excel and append to the same excel file.");
			}
			else
			{
				Button_Generate.Content = "Read, Generate and Create";
				ToolTip.SetTip(Button_Generate, "If this excel exists, read it first. Then generate new sequences that would not duplicate with the existing excel and create a new excel file.");
			}
		}
	}

	private async void OnButtonClick_OpenFolderDialog(object sender, RoutedEventArgs args)
	{
		var topLevel = GetTopLevel(this);

		var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
		{
			Title = "Choose Excel Output Folder",
			AllowMultiple = false
		});

		if (folders.Count >= 1)
		{
			ExcelPathBox.Text = Path.Combine(folders[0].Path.LocalPath, "UMI.xlsx");
		}
	}

	private async void OnButtonClick_OpenFileDialog(object sender, RoutedEventArgs args)
	{
		var topLevel = GetTopLevel(this);

		var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
		{
			Title = "Choose Excel",
			AllowMultiple = false,
			FileTypeFilter =
			[
				new FilePickerFileType("Excel Workbook")
				{
					Patterns = ["*.xlsx"]
				}
			]
		});

		if (files.Count >= 1)
			ExcelPathBox.Text = files[0].Path.LocalPath;
	}
}