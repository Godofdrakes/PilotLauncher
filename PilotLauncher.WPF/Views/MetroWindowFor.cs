using System.Reactive.Disposables;
using System.Windows;
using MahApps.Metro.Controls;
using PilotLauncher.WPF.ViewModels;
using ReactiveUI;

namespace PilotLauncher.WPF.Views;

public class MetroWindowFor<TViewModel> : MetroWindow, IViewFor<TViewModel>
	where TViewModel : WindowViewModel
{
	/// <summary>
	/// The view model dependency property.
	/// </summary>
	public static readonly DependencyProperty ViewModelProperty =
		DependencyProperty.Register(
			nameof(ViewModel),
			typeof(TViewModel),
			typeof(MetroWindowFor<TViewModel>),
			new PropertyMetadata(null));

	/// <summary>
	/// Gets the binding root view model.
	/// </summary>
	public TViewModel? BindingRoot => ViewModel;

	/// <inheritdoc/>
	public TViewModel? ViewModel
	{
		get => (TViewModel)GetValue(ViewModelProperty);
		set => SetValue(ViewModelProperty, value);
	}

	/// <inheritdoc/>
	object? IViewFor.ViewModel
	{
		get => ViewModel;
		set => ViewModel = (TViewModel?)value;
	}

	protected MetroWindowFor()
	{
		this.WhenActivated(d =>
		{
			this.OneWayBind(ViewModel, model => model.Title, window => window.Title)
				.DisposeWith(d);
			this.WhenAnyValue(window => window.ViewModel)
				.BindTo(this, window => window.DataContext)
				.DisposeWith(d);
		});
	}
}