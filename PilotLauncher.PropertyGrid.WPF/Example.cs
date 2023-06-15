using System.Reactive.Linq;
using System.Windows;
using ReactiveUI;

namespace PilotLauncher.PropertyGrid.WPF;

[PropertyGridCategory("Example Object")]
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

	public int Age
	{
		get => _age;
		set => this.RaiseAndSetIfChanged(ref _age, value);
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

	public bool Temp
	{
		get;
		set;
	}

	public string FullName => _fullName.Value;
	public bool IsNameVisible => _isNameVisible.Value;
	public bool IsTypeVisible => _isTypeVisible.Value;

	private string _firstName = "John";
	private string _lastName = "Doe";
	private int _age = 42;
	private Visibility _propertyNameVisibility = Visibility.Visible;
	private Visibility _propertyTypeVisibility = Visibility.Collapsed;

	private readonly ObservableAsPropertyHelper<string> _fullName;
	private readonly ObservableAsPropertyHelper<bool> _isNameVisible;
	private readonly ObservableAsPropertyHelper<bool> _isTypeVisible;

	public Example()
	{
		_fullName = this.WhenAnyValue(example => example.FirstName, example => example.LastName)
			.Select(tuple => string.Join(' ', tuple.Item1, tuple.Item2))
			.ToProperty(this, example => example.FullName);
		_isNameVisible = this.WhenAnyValue(example => example.PropertyNameVisibility)
			.Select(visibility => visibility is Visibility.Visible)
			.ToProperty(this, example => example.IsNameVisible);
		_isTypeVisible = this.WhenAnyValue(example => example.PropertyTypeVisibility)
			.Select(visibility => visibility is Visibility.Visible)
			.ToProperty(this, example => example.IsTypeVisible);
	}
}