using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameState
{
    public delegate void UpdateBoard();
    public static event UpdateBoard OnBoardUpdated;

    public delegate void UpdateSelected();
    public static event UpdateSelected OnSelectUpdated;

    public static readonly int[] PossibleValues = { 0, 1, 2, 3 };
    public static readonly int Columns = 8;
    public static GameState Global;

    private int[] board;
    public int[] Board
    {
        get => board;
    }

    //Use a value less than -8 as empty value to prevent bugs
    private int selectedIdx = -10;
    public int SelectedIdx
    {
        get => selectedIdx;
        set
        {
            this.selectedIdx = value;
            OnSelectUpdated?.Invoke();
        }
    }

    //Utility array to simulate copy semantics 
    private int[] boardCopy = new int[Columns * Columns];
    private void SwapBoards()
    {
        var copy = boardCopy;
        boardCopy = board;
        board = copy;
        OnBoardUpdated?.Invoke();
    }

    [RuntimeInitializeOnLoadMethod]
    private static void CreateState()
    {
        Global = new GameState();
        var newBoard = new int[Columns * Columns];

        int[] availableValues = new int[PossibleValues.Length];
        int lastValidIndex;

        //Cell values for the previous two top and left cells
        var top = new int[] { -1, -2 };
        var left = new int[] { -1, -2 };

        var xy = new int[] { 0, 0 };
        for (int i = 0; i < newBoard.Length; i++)
        {
            Array.Copy(PossibleValues, availableValues, PossibleValues.Length);
            lastValidIndex = PossibleValues.Length - 1;

            //To make things easier calculate which of
            //the adjacent cells has the greater value
            var topGreater = top[0] > left[0];
            var min = topGreater ? left : top;
            var max = topGreater ? top : left;

            //If necesary remove the value on the adjacent cells
            //from the possible values, do the greater one first
            if (max[0] == max[1])
            {
                availableValues[max[0]] = availableValues[lastValidIndex];
                lastValidIndex--;
            }
            //Do not remove min if it is the same as max
            if (min[0] == min[1] && min[0] != max[0])
            {
                availableValues[min[0]] = availableValues[lastValidIndex];
                lastValidIndex--;
            }

            //Draw a random value from the available values
            var selectedIndex = (int)(
                UnityEngine.Random.value * lastValidIndex
            );
            var selectedValue = availableValues[selectedIndex];
            newBoard[i] = selectedValue;

            //Calculate next iteration's adjacent values
            var nextIndex = i + 1;
            var y_2 = nextIndex - Columns - Columns;
            xy[1] = Math.DivRem(nextIndex, Columns, out xy[0]);
            //FIXME: This generates garbage
            left = xy[0] == 0 ?
                new int[] { -1, -2 } :
                new int[] { selectedValue, left[0] };
            top = xy[1] == 0 ?
                new int[] { -1, -2 } :
                new int[] { newBoard[nextIndex - Columns],
                    y_2 < 0 ? -1 : newBoard[y_2] };
        }
        Global.board = newBoard;
    }

    public void Select(int newIndex)
    {
        Array.Copy(board, boardCopy, board.Length);

        var indexDiff = Mathf.Abs(newIndex - SelectedIdx);
        if (indexDiff == 1 || indexDiff == 8)
        {
            var temp = boardCopy[newIndex];
            this.boardCopy[newIndex] = boardCopy[SelectedIdx];
            this.boardCopy[SelectedIdx] = temp;
            SelectedIdx = -10;
            SwapBoards();
            return;
        }

        SelectedIdx = newIndex;
    }
}
