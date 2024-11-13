using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegionInfoUI : MonoBehaviour
{
    public GameObject regionCanvas;
    public Image PanelImage;

    public TextMeshProUGUI RegionText;
    public Button DemographicDetailsButton;
    public Button StartRallyButton;
    public Image HasHeldRallyCheckmark;
    public GameObject UICameratarget;
    RegionData RegionData;
    TopicData TopicData;

    [SerializeField] GameObject BarChartUI;

    bool isInit = false;

    public void ShowUI()
    {
        if(isInit)
        {
            regionCanvas.SetActive(true);
            BarChartUI.SetActive(true);
        }
        StartRallyButton.onClick.AddListener(OnStartRallyClicked);
        DemographicDetailsButton.onClick.AddListener(OnDemographicDetialsButtonClicked);
    }
    public void HideUI()
    {
        regionCanvas.SetActive(false);
        BarChartUI.SetActive(false);
        StartRallyButton.onClick.RemoveListener(OnStartRallyClicked);
        DemographicDetailsButton.onClick.RemoveListener(OnDemographicDetialsButtonClicked);
    }

    void OnStartRallyClicked()
    {
        Region region = GetComponentInParent<Region>();
        if (region != null )
        {
            region.OnSelected();
        }
    }

    public void InitUI(RegionData regionData, TopicData topicData, string _highestPopulationGroup)
    {

        HasHeldRallyCheckmark.gameObject.SetActive(false);
        RegionData = regionData;
        TopicData = topicData;
        RegionText.text = $"The most concerning issue amoung the residents of {regionData.RegionName} is <color=#F3C44D>{topicData.Topic}</color>";
        isInit = true;
        if (RegionData.HasHadRally)
        {
            HasHeldRallyCheckmark.gameObject.SetActive(true);
            DemographicDetailsButton.gameObject.SetActive(false);
            StartRallyButton.gameObject.SetActive(false);
            string regionText = RegionText.text + "\n" + $"Chance of winning: <color=#F3C44D>{regionData.PercentageOfWinning}%</color>";
            RegionText.text = regionText;
        }
    }

    void OnDemographicDetialsButtonClicked()
    {
        DemographicDetialsUIController.Instance.OpenUI(RegionData, TopicData);
    }
}
