using UnityEngine;
using UnityEngine.UI;

public class ActiveSquareImageSelector : MonoBehaviour
{
    public SquareTextureData squareTextureData;
    public bool isUpdateImageOnReachTreshold = false;

    private void OnEnable()
    {
        UpdateSquareColorBaseCurrentPoints();

        if (isUpdateImageOnReachTreshold)
        {
            GameEvents.UpdateSquareColor += UpdateSquareColor;
        }
    }
    private void OnDisable()
    {
        if (isUpdateImageOnReachTreshold)
        {
            GameEvents.UpdateSquareColor -= UpdateSquareColor;
        }
    }
    private void UpdateSquareColorBaseCurrentPoints()
    {
        foreach(var squareTexture in squareTextureData.activeSquareTextures)
        {
            if(squareTextureData.currentColor == squareTexture.squareColor)
            {
                GetComponent<Image>().sprite = squareTexture.texture;
            }
        }
    }
    private void UpdateSquareColor(Config.SquareColor color)
    {
        foreach(var squareTexture in squareTextureData.activeSquareTextures)
        {
            if(color == squareTexture.squareColor)
            {
                GetComponent<Image>().sprite = squareTexture.texture;
            }
        }
    }
}
