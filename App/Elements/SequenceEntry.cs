using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace App.Elements;

public partial class SequenceEntry : ObservableObject
{
	[ObservableProperty]
	private string name = "";

	[ObservableProperty]
	private string sequence = "";

	public event EventHandler? SequenceChanged;

	public SequenceEntry() { }

	public SequenceEntry(string name, string sequence)
	{
		this.name = name;
		this.sequence = sequence;
	}

	partial void OnSequenceChanged(string value)
	{
		SequenceChanged?.Invoke(this, EventArgs.Empty);
	}

	partial void OnNameChanged(string value)
	{
		SequenceChanged?.Invoke(this, EventArgs.Empty);
	}
}