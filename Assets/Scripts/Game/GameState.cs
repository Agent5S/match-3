using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public uint columns = 8;

    private int[] board;

    private void Awake()
    {
        board = new int[columns * columns];
    }

    private void Start()
    {
        for (int i = 0; i < board.Length; i++)
        {
            //Draw a random value from 0-4
            var selectedValue = (int)(Random.value * 3);
            board[i] = selectedValue;
        }
    }
}
