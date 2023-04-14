using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using DynamicData;
using PilotLauncher.Plugins;
using PropertyDetails;
using PropertyDetails.Interfaces;
using ReactiveUI;

namespace PilotLauncher.WPF.ViewModels;

public class EditWorkflowViewModel : ReactiveObject, IEditWorkflowViewModel
{
	public IWorkflowNode? WorkflowNode
	{
		get => _workflowNode;
		set => this.RaiseAndSetIfChanged(ref _workflowNode, value);
	}

	private IWorkflowNode? _workflowNode;

	public ReadOnlyObservableCollection<IPropertyDetails> PropertyInfo => _propertyInfo;

	private readonly ReadOnlyObservableCollection<IPropertyDetails> _propertyInfo;

	public EditWorkflowViewModel()
	{
		ObservableChangeSet.Create<IPropertyDetails>(list =>
				this.WhenAnyValue(model => model.WorkflowNode)
					.WhereNotNull()
					.OfType<ReactiveObject>()
					.Select(node => node.CreatePropertyDetails())
					.Subscribe(list.ReplaceAll))
			.Bind(out _propertyInfo)
			.Subscribe();
	}
}