using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreText : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
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
        scoreText.text = "0";
        isNewBestScore = false;
        UpdateScoreText();
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
        }
        UpdateScoreText();
    }
    private void UpdateScoreText()
    {
        scoreText.text = currentScores.ToString();
    } 
}
