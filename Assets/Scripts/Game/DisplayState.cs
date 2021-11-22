using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayState : MonoBehaviour
{
    public GameObject buttonPrefab;

    private GameState state;

    private void Awake()
    {
        this.state = GetComponent<GameState>();

        for (int i = 0; i < state.columns * state.columns; i++)
        {
            GameObject button = Instantiate(buttonPrefab);
            button.transform.SetParent(transform);
        }
    }
}
