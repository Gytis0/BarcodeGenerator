using App.Elements;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace App.ViewModels;

public partial class SequenceSplittingViewModel : ObservableValidator
{
	[ObservableProperty]
	private int parts = 2;

	[ObservableProperty]
	private int maxLengthDifference = 2;

	[ObservableProperty]
	private string output = "";

	public ObservableCollection<SequenceEntry> Sequences { get; } = new();

	public SequenceSplittingViewModel()
	{
		Sequences.CollectionChanged += OnSequencesChanged;
		AddSequence();
	}

	private void OnSequencesChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		if (e.NewItems != null)
		{
			foreach (SequenceEntry entry in e.NewItems)
				entry.SequenceChanged += OnSequenceEntryChanged;
		}

		if (e.OldItems != null)
		{
			foreach (SequenceEntry entry in e.OldItems)
				entry.SequenceChanged -= OnSequenceEntryChanged;
		}

		UpdateOutput();
	}

	private void OnSequenceEntryChanged(object? sender, EventArgs e)
	{
		UpdateOutput();
	}

	[RelayCommand]
	public void AddSequence()
	{
		Sequences.Add(new SequenceEntry());
	}

	public void CleanSequences()
	{
		for (int i = Sequences.Count - 1; i >= 0; i--)
			if (string.IsNullOrWhiteSpace(Sequences[i].Name) && string.IsNullOrWhiteSpace(Sequences[i].Sequence))
				Sequences.RemoveAt(i);
	}

	public void AddSequence(string name, string sequence)
	{
		Sequences.Add(new(name, sequence));
	}

	[RelayCommand]
	private void RemoveSequence(SequenceEntry sequence)
	{
		Sequences.Remove(sequence);
	}

	partial void OnPartsChanged(int value)
	{
		UpdateOutput();
	}

	partial void OnMaxLengthDifferenceChanged(int value)
	{
		UpdateOutput();
	}

	private void UpdateOutput()
	{
		try
		{
			if (Sequences.Count <= 0 || Parts <= 1)
			{
				Output = "";
				return;
			}

			HashSet<string> endings = [];
			StringBuilder sb = new StringBuilder();
			foreach(var sequence in Sequences)
			{
				if(string.IsNullOrWhiteSpace(sequence.Name) && string.IsNullOrWhiteSpace(sequence.Sequence))
					continue;

				var result = SequenceSplitter.SplitSequenceGreedy_OffsetBy4(sequence.Sequence, Parts, MaxLengthDifference, endings);
				var adjusted = SequenceSplitter.ManuallyAddBases(result.output);
				sb.AppendLine(sequence.Name);
				foreach(var part in adjusted)
					sb.AppendLine(part);
				foreach (var ending in result.existingEndings)
					endings.Add(ending);
			}

			Output = sb.ToString();
		}
		catch (Exception ex)
		{
			Output = ex.Message;
		}
	}
}