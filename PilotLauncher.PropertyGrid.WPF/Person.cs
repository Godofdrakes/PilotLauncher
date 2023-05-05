using System;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PilotLauncher.PropertyGrid.WPF;

public class Person : ReactiveObject
{
	[Reactive] public string FirstName { get; set; }
	[Reactive] public string LastName { get; set; }
	[Reactive] public DateTime Birthday { get; set; }

	public int Age => _age.Value;

	private readonly ObservableAsPropertyHelper<int> _age;

	public Person()
	{
		FirstName = "John";
		LastName = "Doe";
		Birthday = DateTime.MinValue;

		_age = this.WhenAnyValue(person => person.Birthday)
			.Select(GetAge)
			.ToProperty(this, person => person.Age);
	}

	// Not correct but good enough
	private static int GetAge(DateTime birthday) => Math.Max(0, DateTime.Now.Year - birthday.Year);
}