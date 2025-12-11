using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(Button))]
public class RequestNewShapeButton : MonoBehaviour
{
    public int numberOfRequests = 3;
    public TextMeshProUGUI numberText;

    private int currentNumberOfRequests;
    private Button button;
    private bool isLocked;
    private void Start()
    {
        currentNumberOfRequests = numberOfRequests;
        numberText.text = currentNumberOfRequests.ToString();
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonDown);
        UnLock();
    }
    private void OnButtonDown()
    {
        if (isLocked == false)
        {
            currentNumberOfRequests--;
            GameEvents.RequestNewShapes();
            GameEvents.CheckIfPlayerLost();

            if (currentNumberOfRequests <= 0)
            {
                Lock();
            }

            numberText.text = currentNumberOfRequests.ToString();
        }
    }
    private void Lock()
    {
        isLocked = true;
        button.interactable = false;
        numberText.text = currentNumberOfRequests.ToString();
        numberText.color = new (1, 1, 1, 0.5f);
    }
    private void UnLock()
    {
        isLocked = false;
        button.interactable = true;
        numberText.color = new(1, 1, 1, 1f);
    }
}
