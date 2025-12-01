using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static Action CheckIfShapeCanBePlaced;
    
    public static Action MoveShapeToStartPosition;

    public static Action RequestNewShapes;

    public static Action SetShapeInactive;
}
