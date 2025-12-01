using System.Collections.Generic;
using UnityEngine;

public class ShapeStorage : MonoBehaviour
{
    [SerializeField] private List<ShapeData> shapeDatas;
    [SerializeField] public List<Shape> shapeList;
    private void OnEnable()
    {
        GameEvents.RequestNewShapes += RequestNewShapes;
    }
    private void OnDisable()
    {
        GameEvents.RequestNewShapes -= RequestNewShapes;
    }
    void Start()
    {
        foreach (var shape in shapeList)
        {
            int shapeIndex = Random.Range(0, shapeDatas.Count);
            shape.CreateShape(shapeDatas[shapeIndex]);
        }
    }
    public Shape GetCurrentSelectedShape()
    {
        Debug.Log("GetCurrentSelectedShape");
        foreach(var shape in shapeList)
        {
            if(shape.IsOnStartPosition() == false && shape.IsAnyOfShapeSquareActive())
            {
                return shape;
            }
        }
        Debug.LogError("there is no shape selected");
        return null;
    }

    private void RequestNewShapes()
    {
        foreach(var shape in shapeList)
        {
            var shapeIndex = UnityEngine.Random.Range(0, shapeDatas.Count);
            shape.RequestNewShape(shapeDatas[shapeIndex]);
        }
    }
}
