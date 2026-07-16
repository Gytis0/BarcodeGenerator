using CommunityToolkit.Mvvm.ComponentModel;

namespace App.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
	public BarcodeGeneratorViewModel BarcodeGeneratorViewModel { get; } = new();

	public SequenceSplittingViewModel SequenceSplittingViewModel { get; } = new();
}