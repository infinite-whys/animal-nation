using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "My Assets/MainMenuSettings")]
public class MainMenuSettingsSO : ScriptableObject
{
    public AudioClip OnHoverClip;
    public AudioClip OnSelectClip;
    public AudioClip OnConfirmClip;

    public float pausebeforeLoadRegionSelection = 2f;
}
