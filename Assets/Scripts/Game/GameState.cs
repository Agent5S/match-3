using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameState : MonoBehaviour
{
    //TODO: allow for more values to be added dynamically
    private static int[] PossibleValues = { 0, 1, 2, 3 };
    public int columns = 8;

    private int[] _board;
    public int[] board
    {
        private set => _board = value;
        get => _board;
    }

    private void Awake()
    {
        board = new int[columns * columns];

        int[] availableValues = new int[PossibleValues.Length];
        int lastValidIndex;

        //Cell values for the previous two top and left cells
        var top = new int[] { -1, -2 };
        var left = new int[] { -1, -2 };

        var xy = new int[] { 0, 0 };
        for (int i = 0; i < board.Length; i++)
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
            if (min[0] == min[1])
            {
                availableValues[min[0]] = availableValues[lastValidIndex];
                lastValidIndex--;
            }

            //Draw a random value from the available values
            var selectedIndex = (int)(
                UnityEngine.Random.value * lastValidIndex
            );
            var selectedValue = availableValues[selectedIndex];
            board[i] = selectedValue;

            //Calculate next iteration's adjacent values
            var nextIndex = i + 1;
            var y_2 = nextIndex - columns - columns;
            xy[1] = Math.DivRem(nextIndex, columns, out xy[0]);
            left = xy[0] == 0 ?
                new int[]{ -1, -2 } :
                new int[]{ selectedValue, left[0] };
            top = xy[1] == 0 ?
                new int[] { -1, -2 } :
                new int[] { board[nextIndex - columns],
                    y_2 < 0 ? -1 : board[y_2] };
        }
    }
}
