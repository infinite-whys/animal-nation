using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PersonaInfoUIController : MonoBehaviour
{
    [SerializeField] Image Image;
    [SerializeField] TMP_Text Text1;
    [SerializeField] TMP_Text Text2;

    public void Init(Sprite icon, string text1, string text2)
    {
        Image.sprite = icon;
        Text1.text = text1;
        Text2.text = text2;
    }
}
