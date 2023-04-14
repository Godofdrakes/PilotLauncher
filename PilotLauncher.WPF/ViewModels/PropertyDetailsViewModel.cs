using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using PilotLauncher.Common;
using PropertyDetails;
using PropertyDetails.Interfaces;
using ReactiveUI;

namespace PilotLauncher.WPF.ViewModels;

public class PropertyDetailsViewModel : ReactiveObject, IEditWorkflowViewModel
{
	public IWorkflowNode? WorkflowNode
	{
		get => _workflowNode;
		set => this.RaiseAndSetIfChanged(ref _workflowNode, value);
	}

	private IWorkflowNode? _workflowNode;

	public ReadOnlyObservableCollection<IPropertyDetails> PropertyInfo => _propertyInfo;

	private readonly ReadOnlyObservableCollection<IPropertyDetails> _propertyInfo;

	public PropertyDetailsViewModel()
	{
		ObservableChangeSet.Create<IPropertyDetails>(list =>
				this.WhenAnyValue(model => model.WorkflowNode)!
					.Cast<ReactiveObject?>()
					.Select(node => node?.CreatePropertyDetails() ?? Enumerable.Empty<IPropertyDetails>())
					.Subscribe(list.ReplaceAll))
			.Bind(out _propertyInfo)
			.Subscribe();
	}
}