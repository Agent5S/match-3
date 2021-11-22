using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
            var index = i;
            GameObject button = Instantiate(buttonPrefab);
            Button btnComponent = button.GetComponent<Button>();
            //FIXME: This will create a different lambda per button,
            //put this inside a component
            btnComponent.onClick.AddListener(() => { state.Select(index); });
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
        for (int i = 0; i < displays.Length; i++)
        {
            var display = displays[i];
            display.text = $"{Symbols[state.board[i]]}";
        }
    }
}
