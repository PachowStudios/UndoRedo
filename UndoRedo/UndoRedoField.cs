using System;

namespace UndoRedo
{
  public class UndoRedoField<T> : BaseUndoRedo<T>
  {
    private readonly UndoRedoController controller;

    private T value;

    public T Value
    {
      get { return this.value; }
      set { SetValue(value); }
    }

    public UndoRedoField(UndoRedoController controller = null)
    {
      this.controller = controller;
    }

    public override void Undo()
    {
      if (AvailableUndos == 0)
        throw new InvalidOperationException("No undos are available.");

      lock (this.@lock)
      {
        this.redoStack.Push(this.value);
        this.value = this.undoStack.Pop();
      }
    }

    public override void Redo()
    {
      if (AvailableRedos == 0)
        throw new InvalidOperationException("No redos are available.");

      lock (this.@lock)
      {
        this.undoStack.Push(this.value);
        this.value = this.redoStack.Pop();
      }
    }

    private void SetValue(T newValue)
    {
      lock (this.@lock)
      {
        this.undoStack.Push(this.value);
        this.value = newValue;
        this.redoStack.Clear();
        this.controller?.RegisterFieldChange(this);
      }
    }
  }
}
