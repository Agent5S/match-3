using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayState : MonoBehaviour
{
    static readonly char[] Symbols = { 'A', 'B', 'C', 'D' };

    public GameObject buttonPrefab;

    private TextMeshProUGUI[] displays;

    private void Awake()
    {
        var state = GameState.Global;
        var n = state.Board.Length;

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
        var state = GameState.Global;
        for (int i = 0; i < displays.Length; i++)
        {
            var display = displays[i];
            display.text = $"{Symbols[state.Board[i]]}";
        }
    }

    private void OnEnable()
    {
        GameState.OnBoardUpdated += UpdateButtons;
    }

    private void OnDisable()
    {
        GameState.OnBoardUpdated -= UpdateButtons;
    }
}
