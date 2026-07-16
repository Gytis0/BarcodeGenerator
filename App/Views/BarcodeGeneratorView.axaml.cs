using App.Models.Enums;
using App.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace App.Views;

public partial class BarcodeGeneratorView : UserControl
{
	public BarcodeGeneratorView()	
	{
		InitializeComponent();
		DataContext = new BarcodeGeneratorViewModel();
	}

	private void OnTextChanged_ClearStrings(object? sender, TextChangedEventArgs e)
	{
		if (sender is not TextBox tb || tb.Text == null)
			return;

		tb.Text = new string(tb.Text.Where(char.IsDigit).ToArray());
		tb.CaretIndex = tb.Text.Length;
	}

	private async void OnButtonClick_OpenFolderDialog(object sender, RoutedEventArgs args)
	{
		var topLevel = TopLevel.GetTopLevel(this);

		var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
		{
			Title = "Choose Excel Output Folder",
			AllowMultiple = false,
			SuggestedStartLocation = await topLevel.StorageProvider.TryGetFolderFromPathAsync(AppContext.BaseDirectory),
		});

		if (folders.Count >= 1)
		{
			FirstTextField.Text = Path.Combine(folders[0].Path.LocalPath, "UMI.xlsx");
		}
	}

	private async void OnButtonClick_OpenFileDialog_FirstTextBox(object sender, RoutedEventArgs args)
	{
		var topLevel = TopLevel.GetTopLevel(this);

		var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
		{
			Title = "Choose Excel",
			AllowMultiple = false,
			SuggestedStartLocation = await topLevel.StorageProvider.TryGetFolderFromPathAsync(AppContext.BaseDirectory),
			FileTypeFilter =
			[
				new FilePickerFileType("Excel Workbook")
				{
					Patterns = ["*.xlsx"]
				}
			]
		});

		if (files.Count >= 1)
			FirstTextField.Text = files[0].Path.LocalPath;
	}

	private async void OnButtonClick_OpenFileDialog_SecondTextBox(object sender, RoutedEventArgs args)
	{
		var topLevel = TopLevel.GetTopLevel(this);

		var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
		{
			Title = "Choose Excel",
			AllowMultiple = false,
			SuggestedStartLocation = await topLevel.StorageProvider.TryGetFolderFromPathAsync(AppContext.BaseDirectory),
			FileTypeFilter =
			[
				new FilePickerFileType("Excel Workbook")
				{
					Patterns = ["*.xlsx"]
				}
			]
		});

		if (files.Count >= 1)
			SecondTextField.Text = files[0].Path.LocalPath;
	}

	private void OnRadioButtonClick_Logic(object? sender, RoutedEventArgs e)
	{
		if (sender is not RadioButton radioButton || radioButton.IsChecked != true)
			return;

		var viewModel = (BarcodeGeneratorViewModel)DataContext!;

		if (radioButton == Radio_Create)
		{
			viewModel.SaveMode = SaveMode.Create;

			CreateNewPanel.IsVisible = false;
			FirstTextLabel.Text = "Where to create new Excel?";
			Button_OpenFolderDialog.IsVisible = true;
			Button_OpenFileDialog.IsVisible = false;
		}
		else if (radioButton == Radio_ReadCreate)
		{
			viewModel.SaveMode = SaveMode.ReadCreate;

			CreateNewPanel.IsVisible = true;
			FirstTextLabel.Text = "Where to create new Excel?";
			SecondTextLabel.Text = "Which Excel to read?";
			Button_OpenFolderDialog.IsVisible = true;
			Button_OpenFileDialog.IsVisible = false;
		}
		else if (radioButton == Radio_Append)
		{
			viewModel.SaveMode = SaveMode.Append;

			CreateNewPanel.IsVisible = false;
			FirstTextLabel.Text = "Which Excel to append to?";
			Button_OpenFolderDialog.IsVisible = false;
			Button_OpenFileDialog.IsVisible = true;
		}
	}

	private void OnButtonClick_OpenExplorer(object? sender, RoutedEventArgs e)
	{
		if (string.IsNullOrWhiteSpace(FirstTextField.Text))
			return;

		var path = FirstTextField.Text;

		if (File.Exists(path))
			path = Path.GetDirectoryName(path)!;

		if (!Directory.Exists(path))
			return;

		Process.Start(new ProcessStartInfo
		{
			FileName = "explorer.exe",
			Arguments = "/select,\"" + FirstTextField.Text + "\"",
			UseShellExecute = true
		});
	}
}