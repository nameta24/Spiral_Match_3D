using MoreMountains.Feedbacks;
using UnityEngine;

public class MyHapticsManager : MonoBehaviour
{
    public static MyHapticsManager Instance;
    [Header("Haptic Players")]  [SerializeField] private MMF_Player lightHapticPlayer,mediumHapticPlayer,softHapticPlayer,selectionHapticPlayer;
    void Awake()
    {
        Instance = this;
    }

    public void PlayLightHaptics()
    {
        if (SettingsUiManager.Instance.CheckIfVibrationIsEnabled())
        {
            lightHapticPlayer.PlayFeedbacks();
        }
    }
    public void PlayMediumHaptics()
    {
        if (SettingsUiManager.Instance.CheckIfVibrationIsEnabled())
        {
            mediumHapticPlayer.PlayFeedbacks();

        }
    }
    public void PlaySoftHaptics()
    {
        if (SettingsUiManager.Instance.CheckIfVibrationIsEnabled())
        {
            softHapticPlayer.PlayFeedbacks();

        }
    }
    public void PlaySelectionHaptics()
    {
        if (SettingsUiManager.Instance.CheckIfVibrationIsEnabled())
        {
            selectionHapticPlayer.PlayFeedbacks();

        }
    }
}
