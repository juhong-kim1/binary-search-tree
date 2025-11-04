using System.Collections.Generic;
using System.Windows.Input;
using UnityEngine;

public interface ICommand
{
    void Execute();
    void Undo();
}

public class CommandInvoker
{
    private Stack<ICommand> undoStack = new Stack<ICommand>();
    private Stack<ICommand> redoStack = new Stack<ICommand>();

    public void ExecuteCommand(ICommand command) //명령 실행
    {
        command.Execute();
        undoStack.Push(command);
        redoStack.Clear();
    }

    public void Undo() //행동 취소 후 이전 상태로 되돌림
    {
        if (undoStack.Count > 0)
        {
            ICommand command = undoStack.Pop();
            command.Undo();
            redoStack.Push(command);
        }
    }

    public void Redo() //되돌린 행동 다시 실행
    {
        if (redoStack.Count > 0)
        {
            ICommand command = redoStack.Pop();
            command.Execute();
            undoStack.Push(command);
        }
    }

    public bool CanUndo() => undoStack.Count > 0;
    public bool CanRedo() => redoStack.Count > 0;
}

public class PushBackCommand : ICommand
{
    public List<int> intList;
    public int numberToAdd;

    public PushBackCommand(List<int> intList, int numberToAdd)
    {
        this.intList = intList;
        this.numberToAdd = numberToAdd;
    }

    public void Execute()
    {
        intList.Add(numberToAdd);
    }

    public void Undo()
    {
        intList.RemoveAt(intList.Count-1);
    }
}

public class PopBackCommand : ICommand
{
    public List<int> intList;
    public int indexToRemove;
    public int number;

    public PopBackCommand(List<int> intList, int indexToRemove)
    {
        this.intList = intList;
        this.indexToRemove = indexToRemove;

        number = intList[indexToRemove];
    }

    public void Execute()
    {
        if (intList.Count > 0)
            intList.RemoveAt(indexToRemove);
    }

    public void Undo()
    {
        intList.Add(number);
    }
}

public class PushFrontCommand : ICommand
{ 
    public List<int> intList;
    public int numberToAdd;

    public PushFrontCommand(List<int> intList, int numberToAdd)
    {
        this.intList = intList;
        this.numberToAdd = numberToAdd;
    }

    public void Execute()
    {
        intList.Insert(0, numberToAdd);
    }

    public void Undo()
    {
        intList.RemoveAt(0);
    }
}

public class PopFrontCommand : ICommand
{
    public List<int> intList;
    public int number;

    public PopFrontCommand(List<int> intList)
    {
        this.intList = intList;
        number = intList[0];
    }

    public void Execute()
    {
        if (intList.Count > 0)
            intList.RemoveAt(0);
    }

    public void Undo()
    {
        intList.Insert(0, number);
    }
}
