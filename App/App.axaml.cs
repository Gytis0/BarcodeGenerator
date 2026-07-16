using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using App.ViewModels;
using App.Views;
using System;
using System.IO;
namespace App;

public partial class App : Application
{
	public App()
	{
		AppDomain.CurrentDomain.UnhandledException += (s, e) =>
		{
			File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "latestCrash.txt"), e.ExceptionObject.ToString());
		};
	}

	public override void Initialize()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public override void OnFrameworkInitializationCompleted()
	{
		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			desktop.MainWindow = new MainWindow
			{
				DataContext = new MainWindowViewModel(),
				WindowState = Avalonia.Controls.WindowState.Maximized
			};
		}

		base.OnFrameworkInitializationCompleted();
	}
}