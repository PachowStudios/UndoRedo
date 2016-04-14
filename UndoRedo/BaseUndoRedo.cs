using System.Collections.Generic;

namespace UndoRedo
{
  public abstract class BaseUndoRedo<T> : IUndoRedo
  {
    protected readonly object @lock = new object();

    protected readonly Stack<T> undoStack = new Stack<T>();
    protected readonly Stack<T> redoStack = new Stack<T>();

    public int AvailableUndos => this.undoStack.Count;
    public int AvailableRedos => this.redoStack.Count;

    public abstract void Undo();

    public abstract void Redo();

    public void UndoAll()
    {
      lock (this.@lock)
        while (AvailableUndos > 0)
          Undo();
    }

    public void RedoAll()
    {
      lock (this.@lock)
        while (AvailableRedos > 0)
          Redo();
    }
  }
}
