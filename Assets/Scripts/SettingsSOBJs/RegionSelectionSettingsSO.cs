using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "My Assets/RegionSelectionSettings")]
public class RegionSelectionSettingsSO : ScriptableObject
{
    [field: SerializeField]
    public float MoveDistance { get; private set; } = 0.5f;
    [field: SerializeField]
    public float MoveTime { get; private set; } = 0.2f;
    [field: SerializeField]
    public float DelayBetweenRegionSelectedAndBeginLevel { get; private set; } = 2f;

    public AudioClip OnHoverClip;
    public AudioClip OnSelectClip;
    public AudioClip OnConfirmClip;
    public AudioClip OnCantSelectClip;
}
