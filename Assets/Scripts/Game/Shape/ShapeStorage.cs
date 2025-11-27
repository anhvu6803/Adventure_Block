using System.Collections.Generic;
using UnityEngine;

public class ShapeStorage : MonoBehaviour
{
    [SerializeField] private List<ShapeData> shapeDatas;
    [SerializeField] private List<Shape> shapeList;
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
        foreach(var shape in shapeList)
        {
            if(shape.IsOnStartPosition() == false && shape.IsAnyOfShapeSquareActive())
            {
                return shape;
            }
        }
        Debug.Log("there is no shape selected");
        return null;
    }
}
