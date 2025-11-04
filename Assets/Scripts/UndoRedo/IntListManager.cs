using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class IntListManager : MonoBehaviour
{
    //정수 리스트의 **뒤쪽**에서만 요소를 추가(Push)하거나
    //제거(Pop)할 수 있는 프로그램을 작성하세요.

    //커맨드 패턴을 사용하여 모든 작업에 대해 **Undo(실행 취소)** 와
    //** Redo(재실행)** 기능을 구현합니다


    [Header("테스트 설정")]
    [SerializeField] private int numberToAdd = 10;

    private List<int> intList = new List<int>();
    private CommandInvoker invoker = new CommandInvoker();

    private void Start()
    {
        Debug.Log("=== 커맨드 패턴 Undo/Redo 연습 시작 ===");
        PrintList();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.W))
        {
            ExecutePushBack();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.S))
        {
            ExecutePopBack();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.D))
        {
            PrintList();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Z))
        {
            ExecuteUndo();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.X))
        {
            ExecuteRedo();
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            ExecutePushFront();
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            ExecutePopFront();
        }
    }

    private void ExecutePushBack()
    {
            PushBackCommand push = new PushBackCommand(intList, numberToAdd);
            invoker.ExecuteCommand(push);

            numberToAdd += 10;
            PrintList();
    }

    private void ExecutePopBack()
    {
        if (intList != null)
        {
            int indexToRemove = intList.Count-1;
            //intList.Remove(numberToRemove);
            PopBackCommand pop = new PopBackCommand(intList, indexToRemove);
            invoker.ExecuteCommand(pop);
            PrintList();
        }
    }

    private void ExecutePushFront()
    {
        PushFrontCommand pushFront = new PushFrontCommand(intList, numberToAdd);
        invoker.ExecuteCommand(pushFront);

        numberToAdd += 10;
        PrintList();
    }

    private void ExecutePopFront()
    {
        if (intList != null)
        {
            PopFrontCommand popFront = new PopFrontCommand(intList);
            invoker.ExecuteCommand(popFront);
            PrintList();
        }
    }

    private void ExecuteUndo()
    {
        if (invoker.CanUndo())
        {
            invoker.Undo();
            Debug.Log("<color=cyan>↶ Undo</color>");
            PrintList();
        }
        else
        {
            Debug.LogWarning("Undo할 명령이 없습니다!");
        }
    }

    private void ExecuteRedo()
    {
        if (invoker.CanRedo())
        {
            invoker.Redo();
            Debug.Log("<color=magenta>↷ Redo</color>");
            PrintList();
        }
        else
        {
            Debug.LogWarning("Redo할 명령이 없습니다!");
        }
    }

    private void PrintList() //근데 그럼 이 리스트가 쓸모가있나..?
    {
        Debug.Log($"현재 리스트: [{string.Join(", ", intList)}] (개수: {intList.Count})");
    }
}

