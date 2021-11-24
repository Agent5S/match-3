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

    public delegate void UpateScore();
    public static event UpateScore OnScoreUpdated;

    public static readonly int[] PossibleValues = { 0, 1, 2, 3, 4, 5 };
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
        private set
        {
            this.selectedIdx = value;
            OnSelectUpdated?.Invoke();
        }
    }

    private int score = 0;
    public int Score
    {
        get => score;
        private set
        {
            this.score = value;
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

    //Creating my own implementation might be more performant on some cases
    private SortedSet<int> matches = new SortedSet<int>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
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
            if (min[0] != max[0] && min[0] == min[1])
            {
                availableValues[min[0]] = availableValues[lastValidIndex];
                lastValidIndex--;
            }

            //Draw a random value from the available values
            var selectedIndex =
                (int)(UnityEngine.Random.value * lastValidIndex);
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

    public bool Select(int newIndex)
    {
        Array.Copy(board, boardCopy, board.Length);

        var indexDiff = Mathf.Abs(newIndex - SelectedIdx);
        if (indexDiff == 1 || indexDiff == 8)
        {
            var temp = boardCopy[newIndex];
            this.boardCopy[newIndex] = boardCopy[SelectedIdx];
            this.boardCopy[SelectedIdx] = temp;
            SwapBoards();
            SelectedIdx = -10;
            return true;
        }

        SelectedIdx = newIndex;
        return false;
    }

    public int CheckMatches()
    {
        matches.Clear();
        //Horizontal check
        for (int i = 0; i < GameState.Columns; i++)
        {
            var rowStart = i * GameState.Columns;
            var firstMatch = rowStart;
            for (int j = 1; j < GameState.Columns; j++)
            {
                var index = rowStart + j;
                //If they match keep going
                if (Board[index] == Board[index - 1])
                {
                    continue;
                }

                //If more than two matching cells precede 
                //add them to the matching array
                if (index - firstMatch > 2)
                {
                    for (int a = firstMatch; a < index; a++)
                    {
                        matches.Add(a);
                    }
                }
                firstMatch = index;
            }
            //Used for matching the last cell on the row
            var nextIndex = rowStart + GameState.Columns;
            if (nextIndex - firstMatch > 2)
            {
                for (int a = firstMatch; a < nextIndex; a++)
                {
                    matches.Add(a);
                }
            }
        }

        //Vertical check
        for (int i = 0; i < GameState.Columns; i++)
        {
            var columnStart = i;
            var firstMatch = columnStart;
            for (int j = 8; j < 64; j += 8)
            {
                var index = columnStart + j;
                //If they match keep going
                if (Board[index] == Board[index - 8])
                {
                    continue;
                }

                //If more than two matching cells precede 
                //add them to the matching array
                if (index - firstMatch > 2 * 8)
                {
                    for (int a = firstMatch; a < index; a += 8)
                    {
                        matches.Add(a);
                    }
                }
                firstMatch = index;
            }
            //Used for matching the last cell on the column
            var nextIndex = columnStart + 64;
            if (nextIndex - firstMatch > 2 * 8)
            {
                for (int a = firstMatch; a < nextIndex; a += 8)
                {
                    matches.Add(a);
                }
            }
        }

        if (matches.Count > 0)
        {
            Score += matches.Count;
            //Leave matching spaces blank
            Array.Copy(board, boardCopy, board.Length);
            foreach (var match in matches)
            {
                this.boardCopy[match] = -1;
            }
            SwapBoards();
        }

        return matches.Count;
    }

    //This could be integrated into CheckMatches with a do-while loop
    public bool ReplaceMatches()
    {
        //Should not need to do this if called in order
        //Array.Copy(board, boardCopy, board.Length);

        if (matches.Count > 0)
        {
            //FIXME: Do we still need to avoid foreach in 2021?
            foreach (var match in matches)
            {
                var index = match;
                for (int i = index - 8; i > 0; i -= 8)
                {
                    this.boardCopy[index] = boardCopy[i];
                    index = i;
                }

                this.boardCopy[index] =
                    (int)(UnityEngine.Random.value * PossibleValues.Length - 1);
            }
            SwapBoards();
            return true;
        }

        return false;
    }
}
