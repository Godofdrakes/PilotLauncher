using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using DynamicData;
using PilotLauncher.Plugins;
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

	public ReadOnlyObservableCollection<ReactivePropertyInfo> PropertyInfo => _propertyInfo;

	private readonly ReadOnlyObservableCollection<ReactivePropertyInfo> _propertyInfo;

	public EditWorkflowViewModel()
	{
		ObservableChangeSet.Create<ReactivePropertyInfo, PropertyInfo>(cache =>
			{
				var clear = this.WhenAnyValue(model => model.WorkflowNode)
					.Subscribe(_ => cache.Clear());
				var add = this.WhenAnyValue(model => model.WorkflowNode)
					.WhereNotNull()
					.OfType<WorkflowLeaf>()
					.Select(leaf => leaf.GetExposedProperties())
					.Subscribe(cache.AddOrUpdate);
				return new CompositeDisposable(clear, add);
			}, info => info.PropertyInfo)
			.Bind(out _propertyInfo)
			.Subscribe();
	}
}