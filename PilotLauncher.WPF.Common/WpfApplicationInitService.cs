using System.Windows;
using System.Windows.Markup;
using Dapplo.Microsoft.Extensions.Hosting.Wpf;

namespace PilotLauncher.WPF.Common;

public class WpfApplicationInitService : IWpfService
{
	public void Initialize(Application application)
	{
		if (application is IComponentConnector component)
		{
			component.InitializeComponent();
		}
	}
}