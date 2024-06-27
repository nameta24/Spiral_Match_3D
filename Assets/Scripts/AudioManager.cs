using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEditor;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public MMF_Player selectionSound,spiralReachTarget,spiralFall,spiralFormation,flyTowardsTarget,
        win,fail,boosterAppear,fireCrackerActive,uiButton,starUi,tileShuffle;

    public static AudioManager Instance;
    void Awake()
    {
        Instance= this;
    }

    public void PlaySound(MMF_Player mmfPlayer)
    {
        if (SettingsUiManager.Instance.CheckIfSoundIsEnabled())
        {
            mmfPlayer.PlayFeedbacks();
        }
    }
    
}
