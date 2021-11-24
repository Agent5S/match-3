using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayMessage : MonoBehaviour
{
    public TextMeshProUGUI message;
    public TextMeshProUGUI score;

    private void Start()
    {
        var won = GameState.Global.Score > GameState.Goal;
        var message = won ? "Buen trabajo!" : "Mejor suerte a la próxima";
        this.message.text = message;
        this.score.text = $"{GameState.Global.Score} / {GameState.Goal}";
    }
}
