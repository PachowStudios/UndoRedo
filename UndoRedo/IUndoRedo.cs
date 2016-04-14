namespace UndoRedo
{
  public interface IUndoRedo
  {
    int AvailableUndos { get; }
    int AvailableRedos { get; }

    void Undo();

    void UndoAll();

    void Redo();

    void RedoAll();
  }
}
