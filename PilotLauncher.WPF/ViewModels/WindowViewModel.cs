using ReactiveUI;

namespace PilotLauncher.WPF.ViewModels;

public abstract class WindowViewModel : ReactiveObject
{
	public string Title
	{
		get => _title;
		set => this.RaiseAndSetIfChanged(ref _title, value);
	}

	private string _title = string.Empty;
}