using ReactiveUI;

namespace PilotLauncher.WPF.Views;

public class UserControlFor<T> : ReactiveUserControl<T>
	where T : ReactiveObject
{
	protected UserControlFor()
	{
		this.WhenAnyValue(window => window.ViewModel)
			.BindTo(this, window => window.DataContext);
	}
}