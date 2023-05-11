using System;
using System.Reactive.Linq;
using System.Windows;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PilotLauncher.PropertyGrid.WPF;

public class Example : ReactiveObject
{
	[Reactive] public string FirstName { get; set; }
	[Reactive] public string LastName { get; set; }
	[Reactive] public DateTime Birthday { get; set; }

	[Reactive] public Visibility PropertyNameVisibility { get; set; }
	[Reactive] public Visibility PropertyTypeVisibility { get; set; }

	public int Age => _age.Value;
	public bool IsNameVisible => _isNameVisible.Value;
	public bool IsTypeVisible => _isTypeVisible.Value;

	private readonly ObservableAsPropertyHelper<int> _age;
	private readonly ObservableAsPropertyHelper<bool> _isNameVisible;
	private readonly ObservableAsPropertyHelper<bool> _isTypeVisible;

	public Example()
	{
		FirstName = "John";
		LastName = "Doe";
		Birthday = DateTime.MinValue;

		PropertyNameVisibility = Visibility.Visible;
		PropertyTypeVisibility = Visibility.Collapsed;

		_age = this.WhenAnyValue(person => person.Birthday)
			.Select(GetAge)
			.ToProperty(this, person => person.Age);
		_isNameVisible = this.WhenAnyValue(example => example.PropertyNameVisibility)
			.Select(visibility => visibility is Visibility.Visible)
			.ToProperty(this, example => example.IsNameVisible);
		_isTypeVisible = this.WhenAnyValue(example => example.PropertyTypeVisibility)
			.Select(visibility => visibility is Visibility.Visible)
			.ToProperty(this, example => example.IsTypeVisible);
	}

	// Not correct but good enough
	private static int GetAge(DateTime birthday) => Math.Max(0, DateTime.Now.Year - birthday.Year);
}