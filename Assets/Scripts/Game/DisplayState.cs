using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayState : MonoBehaviour
{
    static char[] Symbols = { 'A', 'B', 'C', 'D' };

    public GameObject buttonPrefab;

    private GameState state;
    private TextMeshProUGUI[] displays;

    private void Awake()
    {
        this.state = GetComponent<GameState>();
        var n = state.columns * state.columns;
        this.displays = new TextMeshProUGUI[n];

        for (int i = 0; i < n; i++)
        {
            GameObject button = Instantiate(buttonPrefab);
            TextMeshProUGUI display = button
                .GetComponentInChildren<TextMeshProUGUI>();
            this.displays[i] = display;
            button.transform.SetParent(transform);
        }
    }

    private void Start()
    {
        UpdateButtons();
    }

    public void UpdateButtons()
    {
        for (int i = 0; i < displays.Length - 1; i++)
        {
            var display = displays[i];
            display.text = $"{Symbols[state.board[i]]}";
        }
    }
}
