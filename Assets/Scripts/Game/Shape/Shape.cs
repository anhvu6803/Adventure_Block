using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Shape : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler,
    IPointerDownHandler
{
    public GameObject squareShapeImage;
    public Vector3 shapeSelectedScale;

    [HideInInspector]
    public ShapeData CurrentShapeData;

    private List<GameObject> currentShape = new List<GameObject>();
    private Vector3 shapeStartScale;
    private Vector2 dragOffset;

    private RectTransform _transform;
    private bool isShapeDragable = true;
    private Canvas _canvas;

    public void Awake()
    {
        shapeStartScale = this.GetComponent<RectTransform>().localScale;
        _transform = this.GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        isShapeDragable = true;
    }
    //void Start()
    //{
    //    CreateShape(CurrentShapeData);
    //}
    //public void RequestNewShape(ShapeData shapeData)
    //{
    //    CreateShape(shapeData);
    //}
    public void CreateShape(ShapeData shapeData)
    {
        if (shapeData == null || shapeData.board == null) return;

        CurrentShapeData = shapeData;
        var totalSquareNumber = GetNumberOfSquares(shapeData);

        // Ensure we have exactly totalSquareNumber items in pool
        if (currentShape.Count < totalSquareNumber)
        {
            for (int i = currentShape.Count; i < totalSquareNumber; i++)
            {
                var obj = Instantiate(squareShapeImage, transform) as GameObject;
                currentShape.Add(obj);
            }
        }
        else if (currentShape.Count > totalSquareNumber)
        {
            // Deactivate extras (or Destroy if you prefer)
            for (int i = totalSquareNumber; i < currentShape.Count; i++)
            {
                currentShape[i].SetActive(false);
            }
            // keep list length as-is to reuse later
        }

        // Reset all items to local zero and deactivate
        foreach (var square in currentShape)
        {
            var rt = square.GetComponent<RectTransform>();
            if (rt != null) rt.localPosition = Vector2.zero;
            square.SetActive(false);
        }

        // spacing between squares (width/height of prefab)
        var squareRect = squareShapeImage.GetComponent<RectTransform>();
        var moveDistance = new Vector2(
            squareRect.rect.width * squareRect.localScale.x,
            squareRect.rect.height * squareRect.localScale.y
        );

        int currentIndexInList = 0;
        for (var row = 0; row < shapeData.rows; row++)
        {
            for (var column = 0; column < shapeData.columns; column++)
            {
                if (shapeData.board[row].column[column])
                {
                    if (currentIndexInList >= currentShape.Count)
                    {
                        Debug.LogWarning("Index out of range - skipping");
                        continue;
                    }

                    var sq = currentShape[currentIndexInList];
                    sq.SetActive(true);

                    // Set localPosition using a simple centered formula:
                    Vector2 cellPos = GetCellLocalPosition(row, column, moveDistance, shapeData);
                    sq.GetComponent<RectTransform>().localPosition = cellPos;

                    currentIndexInList++;
                }
            }
        }
    }

    private Vector2 GetCellLocalPosition(int row, int column, Vector2 moveDistance, ShapeData shapeData)
    {
        // center coordinates (can be fractional)
        float centerX = (shapeData.columns - 1) * 0.5f;
        float centerY = (shapeData.rows - 1) * 0.5f;

        // x: (col - center) * spacing
        float x = (column - centerX) * moveDistance.x;

        // y: (center - row) * spacing  -> row 0 will be top if you want that; 
        // adjust sign if you want row 0 bottom: use (row - centerY)
        float y = (centerY - row) * moveDistance.y;

        return new Vector2(x, y);
    }

    private int GetNumberOfSquares(ShapeData shapeData)
    {
        int number = 0;

        foreach(var rowData in shapeData.board)
        {
            foreach(var active in rowData.column)
            {
                if (active) {  number++; }
            }
        }
        return number;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
       
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");

        this.GetComponent<RectTransform>().localScale = shapeSelectedScale;

        // Tính offset giữa con trỏ và vị trí shape
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localMousePos
        );
        dragOffset = (Vector2)_transform.localPosition - localMousePos;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");

        // Không cần chỉnh anchor/pivot
        Vector2 localMousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform,
            eventData.position,
            _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : eventData.pressEventCamera,
            out localMousePos
        );

        _transform.localPosition = localMousePos + dragOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        this.GetComponent<RectTransform>().localScale = shapeStartScale;
    }


    public void OnPointerDown(PointerEventData eventData)
    {

    }
}
