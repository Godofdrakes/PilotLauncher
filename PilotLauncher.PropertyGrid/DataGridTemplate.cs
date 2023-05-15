using System.Windows;

namespace PilotLauncher.PropertyGrid;

public class DataGridTemplate : FrameworkTemplate
{
	public DataGridTemplate()
	{
		
	}

	public DataGridTemplate(FrameworkElementFactory root)
	{
		VisualTree = root;
	}
}