using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DisplayState : MonoBehaviour
{
    static readonly char[] Symbols = { 'A', 'B', 'C', 'D', 'E', 'F' };

    public GameObject buttonPrefab;
    public EventSystem eventSystem;
    public TextMeshProUGUI score;

    private TextMeshProUGUI[] displays;
    private Button[] buttons;

    private void Awake()
    {
        var state = GameState.Global;
        var n = state.Board.Length;

        this.displays = new TextMeshProUGUI[n];
        this.buttons = new Button[n];
        for (int i = 0; i < n; i++)
        {
            var index = i;
            GameObject button = Instantiate(buttonPrefab);
            Button btnComponent = button.GetComponent<Button>();
            //FIXME: This will create a different lambda per button,
            //put this inside a component
            btnComponent.onClick.AddListener(() => {
                StartCoroutine(ButtonSelect(index));
            });
            TextMeshProUGUI display = button
                .GetComponentInChildren<TextMeshProUGUI>();
            this.displays[i] = display;
            this.buttons[i] = btnComponent;
            button.transform.SetParent(transform);
        }
    }

    private IEnumerator ButtonSelect(int index)
    {
        if (GameState.Global.Select(index))
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].enabled = false;
            }
            int matches;
            do
            {
                yield return new WaitForSeconds(0.6f);
                if (GameState.Global.ReplaceMatches())
                {
                    yield return new WaitForSeconds(0.6f);
                }
                matches = GameState.Global.CheckMatches();
            } while (matches > 0);
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].enabled = true;
            }
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
            var character = state.Board[i] < 0 ? ' ' : Symbols[state.Board[i]];
            display.text = $"{character}";
        }
    }

    public void UpdateScore()
    {
        var state = GameState.Global;
        this.score.text = $"{state.Score}";
    }

    public void UpdateSelected()
    {
        var state = GameState.Global;
        var selected = state.SelectedIdx > 0 ?
            buttons[state.SelectedIdx].gameObject :
            null;
        this.eventSystem.SetSelectedGameObject(selected);
    }

    private void OnEnable()
    {
        GameState.OnBoardUpdated += UpdateButtons;
        GameState.OnScoreUpdated += UpdateScore;
        GameState.OnSelectUpdated += UpdateSelected;
    }

    private void OnDisable()
    {
        GameState.OnBoardUpdated -= UpdateButtons;
        GameState.OnScoreUpdated -= UpdateScore;
        GameState.OnSelectUpdated -= UpdateSelected;
    }
}
