using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalculateGridCellSize : MonoBehaviour
{
    public int buttonsPerRow = 8;

    private void Start()
    {
        var xform = GetComponent<RectTransform>();
        var grid = GetComponent<GridLayoutGroup>();
        var width = xform.rect.width / buttonsPerRow;
        grid.cellSize = new Vector2(width, width);
    }
}
