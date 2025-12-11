using TMPro;
using UnityEngine;

public class GameOverPopup : MonoBehaviour
{
    public GameObject gameOverPopup;
    public GameObject loosePopup;
    public GameObject newBestScorePopup;
    public TextMeshProUGUI scoreText;

    [SerializeField] private Scores score;

    private void Start()
    {
        gameOverPopup.SetActive(false);
    }
    private void OnEnable()
    {
        GameEvents.GameOver += OnGameOver;
    }
    private void OnDisable()
    {
        GameEvents.GameOver -= OnGameOver;
    }
    private void OnGameOver(bool newBestScore)
    {
        gameOverPopup.SetActive(true);
        if (newBestScore)
        {
            loosePopup.SetActive(false);
            newBestScorePopup.SetActive(true);
        }
        else
        {
            loosePopup.SetActive(true);
            newBestScorePopup.SetActive(false);
        }

        scoreText.text = score.currentScores.ToString();
    }
}
