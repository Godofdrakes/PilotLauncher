using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using PilotLauncher.Common;

namespace PilotLauncher.WPF;

[ContentProperty(nameof(DefaultTemplate))]
public class WorkflowNodeTemplateSelector : DataTemplateSelector
{
	public DataTemplate? BranchTemplate { get; set; }

	public DataTemplate? DefaultTemplate { get; set; }

	public override DataTemplate? SelectTemplate(object item, DependencyObject container)
	{
		if (item is not IWorkflowNode)
			return null;

		return item switch
		{
			WorkflowBranch => BranchTemplate,
			_              => DefaultTemplate,
		};
	}
}