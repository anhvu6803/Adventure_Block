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

}
