using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapeSquare : MonoBehaviour
{
    [SerializeField] private Image occupiedImage;
    private void Start()
    {
        occupiedImage.gameObject.SetActive(false);
    }
}
