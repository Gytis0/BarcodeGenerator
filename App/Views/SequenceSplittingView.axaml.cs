using App.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Media;
using Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Views;

public partial class SequenceSplittingView : UserControl
{
	public SequenceSplittingView()
	{
		InitializeComponent();
	}

	private void OnDragEnter(object? sender, DragEventArgs e)
	{
		if (sender is Border border)
		{
			border.BorderBrush = Brushes.Purple;

			DropOverlay.IsVisible = true;
		}
	}

	private void OnDragLeave(object? sender, DragEventArgs e)
	{
		if (sender is Border border)
		{
			border.BorderBrush = null;
			DropOverlay.IsVisible = false;
		}
	}

	private void OnDragOver(object? sender, DragEventArgs e)
	{
		// Check if we can accept the data
		if (e.DataTransfer.Formats.Contains(DataFormat.File))
		{
			e.DragEffects = DragDropEffects.Copy;
		}
		else
		{
			e.DragEffects = DragDropEffects.None;
		}
	}

	private void OnDrop(object? sender, DragEventArgs e)
	{
		if (sender is not Border border) return;

		border.BorderBrush = null;

		if (!e.DataTransfer.Formats.Contains(DataFormat.File))
			return;

		var files = e.DataTransfer.TryGetFiles();
		if (files == null)
			return;

		try
		{
			if (DataContext is not SequenceSplittingViewModel vm) return;

			DropOverlay.IsVisible = false;

			List<Tuple<string, string>> fileTuples = [];

			foreach (var file in files)
				fileTuples.Add(new Tuple<string, string>(Path.GetFileNameWithoutExtension(file.Name), SequenceHelper.ExtractSequence(file.Path.LocalPath)));

			vm.CleanSequences();

			foreach (var tuple in fileTuples)
				vm.AddSequence(tuple.Item1, tuple.Item2);

			Status.Text = "";
		}
		catch (Exception ex)
		{
			Status.Text = ex.Message;
		}
	}

	private async void CopyAllClick(object? sender, RoutedEventArgs e)
	{
		if (DataContext is not SequenceSplittingViewModel vm)
			return;

		var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;

		if (clipboard != null)
		{
			await clipboard.SetTextAsync(vm.Output ?? string.Empty);
			Button_CopyAll.Content = "Copied!";
			await Task.Delay(5000);
			Button_CopyAll.Content = "Copy All";
		}
	}
}