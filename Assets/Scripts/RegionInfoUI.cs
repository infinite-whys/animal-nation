using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegionInfoUI : MonoBehaviour
{
    public GameObject regionCanvas;
    public Image PanelImage;

    public TMP_Text RegionText;
    public TMP_Text TopicText;
    public TMP_Text HighestPopulationGroupText;
    public Button DemographicDetailsButton;
    public Image HasHeldRallyCheckmark;
    public GameObject UICameratarget;
    RegionData RegionData;
    TopicData TopicData;

    public Color DefaultColour;
    public Color RalliedColour;

    bool isInit = false;

    public void ShowUI()
    {
        if(isInit)
            regionCanvas.SetActive(true);
        DemographicDetailsButton.onClick.AddListener(OnDemographicDetialsButtonClicked);
    }
    public void HideUI()
    {
        regionCanvas.SetActive(false);
        DemographicDetailsButton.onClick.RemoveListener(OnDemographicDetialsButtonClicked);
    }

    public void InitUI(RegionData regionData, TopicData topicData, string _highestPopulationGroup)
    {
        PanelImage.color = DefaultColour;
        HasHeldRallyCheckmark.gameObject.SetActive(false);
        RegionData = regionData;
        TopicData = topicData;
        RegionText.text = "Region: " + regionData.RegionName;
        TopicText.text = "Topic: " + topicData.Topic;
        HighestPopulationGroupText.text = "Highest Population Group: " + _highestPopulationGroup;
        isInit = true;
        if (RegionData.HasHadRally)
        {
            HasHeldRallyCheckmark.gameObject.SetActive(true);
            HighestPopulationGroupText.text = $"Chance of winning: {regionData.PercentageOfWinning}%";
            DemographicDetailsButton.gameObject.SetActive(false);
            PanelImage.color = RalliedColour;
        }
    }

    void OnDemographicDetialsButtonClicked()
    {
        DemographicDetialsUIController.Instance.OpenUI(RegionData, TopicData);
    }
}
