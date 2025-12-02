using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBoard : MonoBehaviour
{
    [SerializeField] private ShapeStorage shapeStorage;
    [SerializeField] private int rows = 0;
    [SerializeField] private int columns= 0;
    [SerializeField] private float squareGap = 0.1f;
    [SerializeField] private GameObject gridSquare;
    [SerializeField] private Vector2 startPosition = Vector2.zero;
    [SerializeField] private float squareScale = 0.1f;
    [SerializeField] private float everySquareOffset = 0;

    private Vector2 offset = Vector2.zero;
    private List<GameObject> gridSquares = new List<GameObject>();
    private LineIndicator lineIndicator;
    private void OnEnable()
    {
        GameEvents.CheckIfShapeCanBePlaced += CheckIfShapeCanBePlaced;
    }
    private void OnDisable()
    {
        GameEvents.CheckIfShapeCanBePlaced -= CheckIfShapeCanBePlaced;
    }
    void Start()
    {
        lineIndicator = GetComponent<LineIndicator>();
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
                gridSquares[gridSquares.Count - 1].GetComponent<GridSquare>().SquareIndex = squareIndex;
                gridSquares[gridSquares.Count - 1].transform.SetParent(this.transform);
                gridSquares[gridSquares.Count - 1].transform.localScale = new Vector3(squareScale, squareScale, squareScale);
                gridSquares[gridSquares.Count - 1].GetComponent<GridSquare>().SetImage(lineIndicator.GetGridSquareIndex(squareIndex) % 2 == 0);
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
    private void CheckIfShapeCanBePlaced()
    {
        var squareIndexes = new List<int>();

        foreach(var square in gridSquares)
        {
            var gridSquare = square.GetComponent<GridSquare>();
            if(gridSquare.CanWeUseThisSquare() == true)
            {
                squareIndexes.Add(gridSquare.SquareIndex);
                gridSquare.Selected = false;
                Debug.Log(squareIndexes.Count);
                //gridSquare.ActivateSquare();
            }
        }

        var currentSelectedShape = shapeStorage.GetCurrentSelectedShape();
        if (currentSelectedShape == null)  return;

        if(currentSelectedShape.totalSquareNumber == squareIndexes.Count)
        {
            foreach(var squareIndex in squareIndexes)
            {
                gridSquares[squareIndex].GetComponent<GridSquare>().PlaceShapeOnBoard();
            }

            var shapeLeft = 0;
            foreach(var shape in shapeStorage.shapeList)
            {
                if(shape.IsOnStartPosition() && shape.IsAnyOfShapeSquareActive() == true)
                {
                    shapeLeft++;
                }
            }

            if(shapeLeft == 0)
            {
                GameEvents.RequestNewShapes();
            }
            else
            {
                GameEvents.SetShapeInactive();
            }
            CheckIfAnyLineIsCompleted();
        }
        else
        {
            GameEvents.MoveShapeToStartPosition();
        }
    }
    private void CheckIfAnyLineIsCompleted()
    {
        List<int[]> lines = new List<int[]>();

        //columns
        foreach(var column in lineIndicator.columnIndexes)
        {
            lines.Add(lineIndicator.GetVerticalLine(column));
        }

        //rows
        for(var row = 0; row < 9;  row++)
        {
            List<int> data = new List<int>(9);
            for(var index = 0; index < 9; index++)
            {
                data.Add(lineIndicator.line_data[row, index]);
            }
            
            lines.Add(data.ToArray());
        }

        var completedLines = CheckIfSquareAreCompleted(lines);

        if(completedLines > 2)
        {

        }
    }
    private int CheckIfSquareAreCompleted(List<int[]> data)
    {
        List<int[]> completedLines = new List<int[]>();

        var linesCompleted = 0;
        foreach(var line in data)
        {
            var lineCompleted = true;
            foreach(var squareIndex in line)
            {
                var comp = gridSquares[squareIndex].GetComponent<GridSquare>();
                if(comp.SquareOccupied == false)
                {
                    lineCompleted = false;
                }
            }
            if(lineCompleted)
            {
                completedLines.Add(line);
            }
        }

        foreach(var line in completedLines)
        {
            var completed = false;

            foreach(var squareIndex in line)
            {
                var comp = gridSquares[squareIndex].GetComponent<GridSquare>();
                comp.Deactivate();
                completed = true;
            }

            foreach (var squareIndex in line)
            {
                var comp = gridSquares[squareIndex].GetComponent<GridSquare>();
                comp.ClearOccupied();
            }

            if (completed)
            {
                linesCompleted++;
            }
        }
        return linesCompleted;
    }
}
