using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using System.Diagnostics;
using System.Linq;

namespace UI.Views
{
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

		private async void OpenFileButton_Clicked(object sender, RoutedEventArgs args)
		{
			var topLevel = GetTopLevel(this);

			var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
			{
				Title = "Choose Excel Output Folder",
				AllowMultiple = false
			});

			if (folders.Count >= 1)
				FolderPathBox.Text = folders[0].Path.LocalPath;
		}
	}
}