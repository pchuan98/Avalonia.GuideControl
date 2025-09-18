using Avalonia.Controls;
using Avalonia.GuideControl.DevTools.Record;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace Avalonia.GuideControl;

public partial class StepManagerView : UserControl
{
    private StepEditorViewModel? _draggedItem;
    private int _draggedIndex = -1;

    public StepManagerView()
    {
        InitializeComponent();
        AddHandler(DragDrop.DropEvent, OnDrop);
        AddHandler(DragDrop.DragOverEvent, OnDragOver);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (e.Source is not Control control) return;

        var listBoxItem = control.FindAncestorOfType<ListBoxItem>();

        if (listBoxItem?.DataContext is not StepEditorViewModel stepEditor) return;

        _draggedItem = stepEditor;

        if (DataContext is not StepsOrderViewModel viewModel) return;

        _draggedIndex = viewModel.Steps.IndexOf(stepEditor);
        viewModel.SelectedStep = stepEditor;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        if (_draggedItem == null || !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;

        var data = new DataObject();
        data.Set("application/step-editor", _draggedItem);
        DragDrop.DoDragDrop(e, data, DragDropEffects.Move);
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = DragDropEffects.None;
        
        if (e.Data.Contains("application/step-editor"))
        {
            e.DragEffects = DragDropEffects.Move;
        }
        
        e.Handled = true;
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        if (e.Data.Get("application/step-editor") is StepEditorViewModel draggedItem &&
            DataContext is StepsOrderViewModel viewModel)
        {
            var targetControl = e.Source as Control;
            var targetListBoxItem = targetControl?.FindAncestorOfType<ListBoxItem>();
            
            if (targetListBoxItem?.DataContext is StepEditorViewModel targetItem)
            {
                var targetIndex = viewModel.Steps.IndexOf(targetItem);
                var sourceIndex = viewModel.Steps.IndexOf(draggedItem);
                
                if (sourceIndex != -1 && targetIndex != -1 && sourceIndex != targetIndex)
                {
                    viewModel.Steps.Move(sourceIndex, targetIndex);
                    viewModel.SelectedStep = draggedItem;
                }
            }
        }
        
        _draggedItem = null;
        _draggedIndex = -1;
        e.Handled = true;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        _draggedItem = null;
        _draggedIndex = -1;
    }
}