using System;
using System.Reactive.Linq;
using System.Windows;
using ReactiveUI;

namespace PilotLauncher.PropertyGrid.WPF;

public class Example : ReactiveObject
{
	public string FirstName
	{
		get => _firstName;
		set => this.RaiseAndSetIfChanged(ref _firstName, value);
	}

	public string LastName
	{
		get => _lastName;
		set => this.RaiseAndSetIfChanged(ref _lastName, value);
	}

	public DateTime Birthday
	{
		get => _birthday;
		set => this.RaiseAndSetIfChanged(ref _birthday, value);
	}

	public Visibility PropertyNameVisibility
	{
		get => _propertyNameVisibility;
		set => this.RaiseAndSetIfChanged(ref _propertyNameVisibility, value);
	}

	public Visibility PropertyTypeVisibility
	{
		get => _propertyTypeVisibility;
		set => this.RaiseAndSetIfChanged(ref _propertyTypeVisibility, value);
	}

	private string _firstName;
	private string _lastName;
	private DateTime _birthday;
	private Visibility _propertyNameVisibility;
	private Visibility _propertyTypeVisibility;

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