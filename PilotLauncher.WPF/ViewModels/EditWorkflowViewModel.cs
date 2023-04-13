using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using DynamicData;
using PilotLauncher.Plugins;
using PropertyInspector;
using PropertyInspector.Interfaces;
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

	public ReadOnlyObservableCollection<IPropertyInspector> PropertyInfo => _propertyInfo;

	private readonly ReadOnlyObservableCollection<IPropertyInspector> _propertyInfo;

	public EditWorkflowViewModel()
	{
		ObservableChangeSet.Create<IPropertyInspector>(list =>
			{
				var clear = this.WhenAnyValue(model => model.WorkflowNode)
					.Subscribe(_ => list.Clear());
				var add = this.WhenAnyValue(model => model.WorkflowNode)
					.WhereNotNull()
					.OfType<ReactiveObject>()
					.Select(node => node.CreatePropertyInspectors())
					.Subscribe(list.AddRange);
				return new CompositeDisposable(clear, add);
			})
			.Bind(out _propertyInfo)
			.Subscribe();
	}
}