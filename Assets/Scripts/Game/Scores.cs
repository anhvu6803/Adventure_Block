using TMPro;
using UnityEngine;

public class ScoreText : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    private int currentScores;
    private void Start()
    {
        scoreText.text = "0";
        UpdateScoreText();
    }
    private void OnEnable()
    {
        GameEvents.AddScore += AddScore;
    }
    private void OnDisable()
    {
        GameEvents.AddScore -= AddScore;
    }
    private void AddScore(int score)
    {
        currentScores += score;
        UpdateScoreText();
    }
    private void UpdateScoreText()
    {
        scoreText.text = currentScores.ToString();
    }
}
