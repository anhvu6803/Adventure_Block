using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BestScoreBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI bestScoreText;
    private void OnEnable()
    {
        GameEvents.UpdateBestScore += UpdateBestScore;
    }
    private void OnDisable()
    {
        GameEvents.UpdateBestScore -= UpdateBestScore;
    }
    private void UpdateBestScore(int currentScore, int bestScore)
    {
        float currentPercent = (float)currentScore/ (float)bestScore;
        fillImage.fillAmount = currentPercent;
        bestScoreText.text = bestScore.ToString();
    }
}
