using UnityEngine;
using UnityEngine.UI;

public class SettingButton : MonoBehaviour
{
    public Button openSettingButton;
    public Button closeSettingButton;

    public void SettingsOpened()
    {
        openSettingButton.gameObject.SetActive(false);

        closeSettingButton.gameObject.SetActive(true);
        closeSettingButton.interactable = true;
    }
    public void SettingsClosed()
    {
        openSettingButton.gameObject.SetActive(true);
        openSettingButton.interactable = true;

        closeSettingButton.gameObject.SetActive(false);
    }
}
