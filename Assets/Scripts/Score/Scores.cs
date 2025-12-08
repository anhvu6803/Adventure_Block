using System.Collections;
using System.ComponentModel.Design;
using TMPro;
using UnityEngine;

public class ScoreText : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public SquareTextureData squareTextureData;

    private bool isNewBestScore = false;
    private BestScoreData bestScoreData = new BestScoreData();
    private int currentScores;

    private string bestScoreKey = "bsdat";
    private void Awake()
    {
        if (BinaryDataStream.Exist(bestScoreKey))
        {
            StartCoroutine(ReadDataFile());
        }
    }
    private IEnumerator ReadDataFile()
    {
        bestScoreData = BinaryDataStream.Read<BestScoreData>(bestScoreKey);
        yield return new WaitForEndOfFrame();
    }
    private void Start()
    {
        isNewBestScore = false;
        squareTextureData.SetStartColor();
        UpdateScoreText();
        GameEvents.UpdateBestScore(currentScores, bestScoreData.score);
    }
    private void OnEnable()
    {
        GameEvents.AddScore += AddScore;
        GameEvents.GameOver += SaveBestScore;
    }
    private void OnDisable()
    {
        GameEvents.AddScore -= AddScore;
        GameEvents.GameOver -= SaveBestScore;
    }
    private void SaveBestScore(bool isNewBestScore)
    {
        Debug.Log(bestScoreData.score);
        BinaryDataStream.Save<BestScoreData>(bestScoreData, bestScoreKey);
    }
    private void AddScore(int score)
    {
        currentScores += score;
        if(currentScores > bestScoreData.score)
        {
            bestScoreData.score = currentScores;
            isNewBestScore = true;
            SaveBestScore(isNewBestScore);
        }

        UpdateSquareColor();
        GameEvents.UpdateBestScore(currentScores, bestScoreData.score);
        UpdateScoreText();
    }
    private void UpdateSquareColor()
    {
        if( GameEvents.UpdateSquareColor != null && currentScores >= squareTextureData.tresholdVal)
        {
            squareTextureData.UpdateColors(currentScores);
            GameEvents.UpdateSquareColor(squareTextureData.currentColor);
        }
    }
    private void UpdateScoreText()
    {
        scoreText.text = currentScores.ToString();
    } 
}
