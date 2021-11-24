using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;

public class DisplayState : MonoBehaviour
{
    static readonly char[] Symbols = { 'A', 'B', 'C', 'D', 'E', 'F' };

    public GameObject buttonPrefab;
    public EventSystem eventSystem;
    public TextMeshProUGUI score;
    public TextMeshProUGUI time;
    public TextMeshProUGUI goal;

    private TextMeshProUGUI[] displays;
    private Button[] buttons;
    private WaitForSeconds delay = new WaitForSeconds(0.3f);

    private void Awake()
    {
        GameState.CreateState();
        var state = GameState.Global;
        var n = state.Board.Length;

        this.displays = new TextMeshProUGUI[n];
        this.buttons = new Button[n];
        this.time.text = $"{state.Seconds}";
        this.goal.text = $"{GameState.Goal}";
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

    private void Start()
    {
        UpdateButtons();
        InvokeRepeating("CallTick", 1, 1);
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

    public void UpdateSeconds()
    {
        var state = GameState.Global;
        if (state.Seconds == 0)
        {
            SceneManager.LoadSceneAsync(2);
        }
        this.time.text = $"{state.Seconds}";
    }

    private void CallTick()
    {
        GameState.Global.Tick();
    }

    private void OnEnable()
    {
        GameState.OnBoardUpdated += UpdateButtons;
        GameState.OnScoreUpdated += UpdateScore;
        GameState.OnSelectUpdated += UpdateSelected;
        GameState.OnSecondsUpdated += UpdateSeconds;
    }

    private void OnDisable()
    {
        GameState.OnBoardUpdated -= UpdateButtons;
        GameState.OnScoreUpdated -= UpdateScore;
        GameState.OnSelectUpdated -= UpdateSelected;
        GameState.OnSecondsUpdated -= UpdateSeconds;
    }
}
