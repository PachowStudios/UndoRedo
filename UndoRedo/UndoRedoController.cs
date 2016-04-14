using System;
using System.Collections.Generic;

namespace UndoRedo
{
  public class UndoRedoController : BaseUndoRedo<List<IUndoRedo>>
  {
    private bool isTransactionOpen;

    public override void Undo()
    {
      if (AvailableUndos == 0)
        throw new InvalidOperationException("No undos are available.");

      lock (this.@lock)
      {
        var fields = this.undoStack.Pop();

        foreach (var field in fields)
          field.Undo();

        this.redoStack.Push(fields);
      }
    }

    public override void Redo()
    {
      if (AvailableRedos == 0)
        throw new InvalidOperationException("No redos are available.");

      lock (this.@lock)
      {
        var fields = this.redoStack.Pop();

        foreach (var field in fields)
          field.Redo();

        this.undoStack.Push(fields);
      }
    }

    public UndoRedoField<T> CreateField<T>()
    {
      return new UndoRedoField<T>(this);
    }

    public IDisposable OpenTransaction()
    {
      lock (this.@lock)
      {
        if (this.isTransactionOpen)
          throw new InvalidOperationException("An undo/redo transaction is already open.");

        this.isTransactionOpen = true;
        this.undoStack.Push(new List<IUndoRedo>());
      }

      return new UndoTransaction(this);
    }

    internal void RegisterFieldChange(IUndoRedo field)
    {
      lock (this.@lock)
      {
        if (this.isTransactionOpen)
          this.undoStack.Peek().Add(field);
        else
          this.undoStack.Push(new List<IUndoRedo>(new[] { field }));

        this.redoStack.Clear();
      }
    }

    private void CloseTransaction()
    {
      lock (this.@lock)
      {
        if (!this.isTransactionOpen)
          throw new InvalidOperationException("An undo/redo transaction isn't open.");

        this.isTransactionOpen = false;
      }
    }

    private class UndoTransaction : IDisposable
    {
      private readonly UndoRedoController controller;

      public UndoTransaction(UndoRedoController controller)
      {
        this.controller = controller;
      }

      public void Dispose()
      {
        this.controller.CloseTransaction();
      }
    }
  }
}
