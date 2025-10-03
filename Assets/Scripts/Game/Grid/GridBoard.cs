using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBoard : MonoBehaviour
{
    [SerializeField] private int rows = 0;
    [SerializeField] private int columns= 0;
    [SerializeField] private float squareGap = 0.1f;
    [SerializeField] private GameObject gridSquare;
    [SerializeField] private Vector2 startPosition = Vector2.zero;
    [SerializeField] private float squareScale = 0.1f;
    [SerializeField] private float everySquareOffset = 0;

    private Vector2 offset = Vector2.zero;
    private List<GameObject> gridSquares = new List<GameObject>();
    void Start()
    {
        CreateGrid();
    }
    private void CreateGrid()
    {
        SpawnGridSquare();
        SetGridSquarePos();
    }
    private void SpawnGridSquare()
    {
        int squareIndex = 0;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                gridSquares.Add(Instantiate(gridSquare) as  GameObject);
                gridSquares[gridSquares.Count - 1].transform.SetParent(this.transform);
                gridSquares[gridSquares.Count - 1].transform.localScale = new Vector3(squareScale, squareScale, squareScale);
                gridSquares[gridSquares.Count - 1].GetComponent<GridSquare>().SetImage(squareIndex % 2 == 0);
                squareIndex++;
            }
        }
    }
    private void SetGridSquarePos()
    {
        int columnNumber = 0;
        int rowNumber = 0;
        Vector2 squareGapNumber = Vector2.zero;
        bool rowMoved = false;

        RectTransform squareRect = gridSquares[0].GetComponent<RectTransform>();
        offset.x = squareRect.rect.width * squareRect.transform.localScale.x + everySquareOffset;
        offset.y = squareRect.rect.height * squareRect.transform.localScale.y + everySquareOffset;

        foreach (GameObject square in gridSquares)
        {
            if(columnNumber + 1 > columns)
            {
                squareGapNumber.x = 0;
                columnNumber = 0;
                rowNumber++;
                rowMoved = true;
            }

            float pos_x_offset = offset.x * columnNumber + (squareGapNumber.x * squareGap);
            float pos_y_offset = offset.y * rowNumber + (squareGapNumber.y * squareGap);

            if(columnNumber > 0 && columnNumber % 3 == 0)
            {
                squareGapNumber.x++;
                pos_x_offset += squareGap;
            }

            if (rowNumber > 0 && rowNumber % 3 == 0&& rowMoved == false) 
            { 
                squareGapNumber.y++;
                pos_y_offset += squareGap;
                rowMoved = true;
            }

            square.GetComponent<RectTransform>().anchoredPosition = new Vector2(startPosition.x + pos_x_offset, startPosition.y - pos_y_offset);
            square.GetComponent<RectTransform>().localPosition = new Vector3(startPosition.x + pos_x_offset, startPosition.y - pos_y_offset, 0);
            columnNumber++;
        }
    }
}
