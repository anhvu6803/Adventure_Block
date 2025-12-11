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
    [SerializeField] private SquareTextureData squareTextureData ;

    private Vector2 offset = Vector2.zero;
    private List<GameObject> gridSquares = new List<GameObject>();
    private LineIndicator lineIndicator;
    private Config.SquareColor currentActiveSquareColor = Config.SquareColor.NotSet;
    private List<Config.SquareColor> colorsInTheGrid = new List<Config.SquareColor>();
    private List<Config.SquareColor> colorsInTheGridAfterLineRemove = new List<Config.SquareColor>();

    private void OnEnable()
    {
        GameEvents.CheckIfShapeCanBePlaced += CheckIfShapeCanBePlaced;
        GameEvents.UpdateSquareColor += OnUpdateSquareColor;
        GameEvents.CheckIfPlayerLost += CheckIfPlayerLost;
    }
    private void OnDisable()
    {
        GameEvents.CheckIfShapeCanBePlaced -= CheckIfShapeCanBePlaced;
        GameEvents.UpdateSquareColor -= OnUpdateSquareColor;
        GameEvents.CheckIfPlayerLost -= CheckIfPlayerLost;
    }
    void Start()
    {
        lineIndicator = GetComponent<LineIndicator>();
        CreateGrid();
        currentActiveSquareColor = squareTextureData.activeSquareTextures[0].squareColor;
    }
    private void OnUpdateSquareColor(Config.SquareColor color)
    {
        currentActiveSquareColor = color;
    }
    private List<Config.SquareColor> GetAllSquareColorsInTheGrid()
    {
        var colors = new List<Config.SquareColor>();

        foreach(var square in gridSquares)
        {
            var gridSquare = square.GetComponent<GridSquare>();
            if (gridSquare.SquareOccupied)
            {
                var color = gridSquare.GetCurrentColor();
                if(colors.Contains(color) == false)
                {
                    colors.Add(color);
                }
            }
        }

        return colors;
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
                //gridSquare.ActivateSquare();
            }
        }

        var currentSelectedShape = shapeStorage.GetCurrentSelectedShape();
        if (currentSelectedShape == null)  return;

        if(currentSelectedShape.totalSquareNumber == squareIndexes.Count)
        {
            foreach(var squareIndex in squareIndexes)
            {
                gridSquares[squareIndex].GetComponent<GridSquare>().PlaceShapeOnBoard(currentActiveSquareColor);
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

        //squares
        for(var square = 0;  square < 9; square++)
        {
            List<int> data = new List<int>(9);
            for (var index = 0; index < 9; ++index)
            {
                data.Add(lineIndicator.square_data[square, index]);
            }
            lines.Add(data.ToArray());
        }

        colorsInTheGrid = GetAllSquareColorsInTheGrid();

        var completedLines = CheckIfSquareAreCompleted(lines);

        if(completedLines >= 2)
        {
            GameEvents.ShowCongratulationWritings();
        }

        var totalScores = 10 * completedLines;
        var bonusScore = ShouldPlayColorBonusAnimation();
        GameEvents.AddScore(totalScores + bonusScore);
        GameEvents.CheckIfPlayerLost();
    }
    private int ShouldPlayColorBonusAnimation()
    {
        colorsInTheGridAfterLineRemove = GetAllSquareColorsInTheGrid();

        Config.SquareColor colorToPlayBonus = Config.SquareColor.NotSet;

        foreach(var squareColor in colorsInTheGrid)
        {
            if(colorsInTheGridAfterLineRemove.Contains(squareColor) == false)
            {
                colorToPlayBonus = squareColor;
                Debug.Log(colorToPlayBonus);
            }
        }

        if(colorToPlayBonus == Config.SquareColor.NotSet)
        {
            return 0;
        }

        if(colorToPlayBonus == currentActiveSquareColor)
        {
            return 0;
        }

        GameEvents.ShowBonusScreen(colorToPlayBonus);
        return 50;
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
    private void CheckIfPlayerLost()
    {
        var validShapes = 0;

        for(var index = 0; index < shapeStorage.shapeList.Count; index++)
        {
            var isShapeActive = shapeStorage.shapeList[index].IsAnyOfShapeSquareActive();

            if (CheckIfShapeCanBePlacedOnGrid(shapeStorage.shapeList[index]) && isShapeActive)
            {
                shapeStorage.shapeList[index]?.ActivateShape();
                validShapes++;
            }
        }

        if(validShapes == 0)
        {
            GameEvents.GameOver(false);
        }
    }
    private bool CheckIfShapeCanBePlacedOnGrid(Shape currentShape)
    {
        var currentShapeData = currentShape.CurrentShapeData;
        var shapeColumns = currentShapeData.columns;
        var shapeRows = currentShapeData.rows;

        List<int> originalShapeFilledUpSquares = new List<int>();
        var squareIndex = 0;

        for(var rowIndex = 0; rowIndex < shapeRows;  rowIndex++)
        {
            for(var columnIndex = 0; columnIndex < shapeColumns; columnIndex++)
            {
                if (currentShapeData.board[rowIndex].column[columnIndex])
                {
                    originalShapeFilledUpSquares.Add(squareIndex);
                }
                squareIndex++;
            }
        }

        if(currentShape.totalSquareNumber != originalShapeFilledUpSquares.Count)
        {
            Debug.LogError("Number of filled up squares are not the same as the original shape have.");
        }

        var squareList = GetAllSquaresCombination(shapeColumns, shapeRows);

        bool canBePlaced = false;

        foreach(var number in squareList)
        {
            bool shapeCanBePlacedOnTheBoard = true;
            foreach(var squareIndexToCheck in originalShapeFilledUpSquares)
            {
                var comp = gridSquares[number[squareIndexToCheck]].GetComponent<GridSquare>();
                if (comp.SquareOccupied)
                {
                    shapeCanBePlacedOnTheBoard = false;
                }
            }
            if(shapeCanBePlacedOnTheBoard)
            {
                canBePlaced = true;
            }
        }
        return canBePlaced;
    }
    private List<int[]> GetAllSquaresCombination(int columns, int rows)
    {
        var squareList = new List<int[]>();
        var lastColumnIndex = 0;
        var lastRowIndex = 0;

        int safeIndex = 0;
        
        while(lastRowIndex + (rows - 1) < 9)
        {
            var rowData = new List<int>();

            for(var row = lastRowIndex; row < lastRowIndex + rows; row++)
            {
                for (var col = lastColumnIndex; col < lastColumnIndex + columns; col++)
                {
                    rowData.Add(lineIndicator.line_data[row, col]);
                }
            }

            squareList.Add((rowData.ToArray()));

            lastColumnIndex++;

            if(lastColumnIndex + (columns - 1) >= 9)
            {
                lastRowIndex++;
                lastColumnIndex = 0;
            }

            safeIndex++;
            if(safeIndex > 100)
            {
                break;
            }
        }
        return squareList;
    }
}
