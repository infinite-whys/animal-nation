using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OverviewSpeciesUIController : MonoBehaviour
{
    [SerializeField] Image Image;
    [SerializeField] TMP_Text Text;

    public void Init(Sprite icon,string text)
    {
        Image.sprite = icon;
        Text.text = $"'{text}'";
    }
}
