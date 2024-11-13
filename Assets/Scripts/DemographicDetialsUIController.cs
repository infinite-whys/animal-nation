using ChartAndGraph;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DemographicDetialsUIController : MonoBehaviour
{
    public static DemographicDetialsUIController Instance;

    [SerializeField] GameObject OverviewSpeciesUIPrefab;
    [SerializeField] GameObject PersonaInfoUIPrefab;

    [SerializeField] List<GameObject> Tabs = new List<GameObject>();
    [SerializeField] List<GameObject> Panels = new List<GameObject>();

    [SerializeField] GameObject UICanvas;

    RegionData CurrentRegion;
    TopicData CurrentTopic;

    //BarChart
    [SerializeField] CanvasBarChart barChart;
    [SerializeField] Material[] materials;
    [SerializeField] string[] Groups = { "Population", "Age", "", "", "", "" };

    //OverViewPage
    [SerializeField] GameObject OverviewGridContainer;
    [SerializeField] TMP_Text IntroText;
    [SerializeField] TMP_Text TopicText;
    [SerializeField] TMP_Text CenturalTopicText;
    [SerializeField] TMP_Text SurveyText;

    //Population
    [SerializeField] GameObject Container;


    private void Awake()
    {
        if (Instance != null)
            Destroy(this.gameObject);
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        CloseUI();
    }

    public void OnShowOverview()
    {
        HideAllPanels();

        foreach (Transform child in OverviewGridContainer.transform)
            Destroy(child.gameObject);

        foreach (PersonaData persona in GameplayManager.Instance.Personas)
        {
            GameObject newOBJ = GameObject.Instantiate(OverviewSpeciesUIPrefab, OverviewGridContainer.transform);
            newOBJ.GetComponent<OverviewSpeciesUIController>().Init(persona.PersonaIcon, persona.PersonaTraits.painPoints);
        }

        IntroText.text = $"{CurrentRegion.RegionName} is a metropolitan region where...";
        TopicText.text = $"{CurrentTopic.Topic}";
        CenturalTopicText.text = $"CentralTopic:";
        SurveyText.text = $"Here is our initial survey results regarding the topic:";

        Panels[0].SetActive(true);
    }
    public void OnShowPopulation()
    {
        HideAllPanels();

        foreach (Transform child in Container.transform)
            Destroy(child.gameObject);

        ClearChart();
        barChart.DataSource.AddGroup(Groups[0]);

        int i = 0;

        foreach (PersonaData persona in GameplayManager.Instance.Personas)
        {
            if (i > materials.Length - 1)
                i = 0;

            GameObject newOBJ = GameObject.Instantiate(PersonaInfoUIPrefab, Container.transform);
            newOBJ.GetComponent<PersonaInfoUIController>().Init(persona.PersonaIcon, persona.PersonaTraits.species, CurrentRegion.PopulationData[persona.PersonaTraits.species].ToString());
            barChart.DataSource.AddCategory(persona.PersonaTraits.species, materials[i]);
            barChart.DataSource.SetValue(persona.PersonaTraits.species, Groups[0], (double)CurrentRegion.PopulationData[persona.PersonaTraits.species]);
            i++;
        }

        Panels[1].SetActive(true);
    }
    public void ShowPreferences()
    {
        HideAllPanels();

        foreach (Transform child in OverviewGridContainer.transform)
            Destroy(child.gameObject);

        foreach (PersonaData persona in GameplayManager.Instance.Personas)
        {
            GameObject newOBJ = GameObject.Instantiate(OverviewSpeciesUIPrefab, OverviewGridContainer.transform);
            newOBJ.GetComponent<OverviewSpeciesUIController>().Init(persona.PersonaIcon, persona.PersonaTraits.preferences);
        }

        IntroText.text = $"Preferences";
        TopicText.text = string.Empty;
        CenturalTopicText.text = string.Empty;
        SurveyText.text = string.Empty;

        Panels[0].SetActive(true);
    }
    public void ShowPainPoints()
    {
        HideAllPanels();

        foreach (Transform child in OverviewGridContainer.transform)
            Destroy(child.gameObject);

        foreach (PersonaData persona in GameplayManager.Instance.Personas)
        {
            GameObject newOBJ = GameObject.Instantiate(OverviewSpeciesUIPrefab, OverviewGridContainer.transform);
            newOBJ.GetComponent<OverviewSpeciesUIController>().Init(persona.PersonaIcon, persona.PersonaTraits.painPoints);
        }

        IntroText.text = $"PainPoints";
        TopicText.text = string.Empty;
        CenturalTopicText.text = string.Empty;
        SurveyText.text = string.Empty;

        Panels[0].SetActive(true);
    }
    public void ShowOcupation()
    {
        HideAllPanels();

        foreach (Transform child in OverviewGridContainer.transform)
            Destroy(child.gameObject);

        foreach (PersonaData persona in GameplayManager.Instance.Personas)
        {
            GameObject newOBJ = GameObject.Instantiate(OverviewSpeciesUIPrefab, OverviewGridContainer.transform);
            newOBJ.GetComponent<OverviewSpeciesUIController>().Init(persona.PersonaIcon, persona.PersonaTraits.occupation);
        }

        IntroText.text = $"Ocupation";
        TopicText.text = string.Empty;
        CenturalTopicText.text = string.Empty;
        SurveyText.text = string.Empty;

        Panels[0].SetActive(true);
    }
    public void ShowEducation()
    {
        HideAllPanels();

        foreach (Transform child in OverviewGridContainer.transform)
            Destroy(child.gameObject);

        foreach (PersonaData persona in GameplayManager.Instance.Personas)
        {
            GameObject newOBJ = GameObject.Instantiate(OverviewSpeciesUIPrefab, OverviewGridContainer.transform);
            newOBJ.GetComponent<OverviewSpeciesUIController>().Init(persona.PersonaIcon, persona.PersonaTraits.education);
        }

        IntroText.text = $"Education";
        TopicText.text = string.Empty;
        CenturalTopicText.text = string.Empty;
        SurveyText.text = string.Empty;

        Panels[0].SetActive(true);
    }
    public void ShowIncome()
    {
        HideAllPanels();

        foreach (Transform child in OverviewGridContainer.transform)
            Destroy(child.gameObject);

        foreach (PersonaData persona in GameplayManager.Instance.Personas)
        {
            GameObject newOBJ = GameObject.Instantiate(OverviewSpeciesUIPrefab, OverviewGridContainer.transform);
            newOBJ.GetComponent<OverviewSpeciesUIController>().Init(persona.PersonaIcon, persona.PersonaTraits.income);
        }

        IntroText.text = $"Income";
        TopicText.text = string.Empty;
        CenturalTopicText.text = string.Empty;
        SurveyText.text = string.Empty;

        Panels[0].SetActive(true);
    }
    public void ShowAge()
    {
        HideAllPanels();

        foreach (Transform child in Container.transform)
            Destroy(child.gameObject);

        foreach (PersonaData persona in GameplayManager.Instance.Personas)
        {
            GameObject newOBJ = GameObject.Instantiate(PersonaInfoUIPrefab, Container.transform);
            newOBJ.GetComponent<PersonaInfoUIController>().Init(persona.PersonaIcon, persona.PersonaTraits.species, persona.PersonaTraits.ageRange);
        }

        Panels[1].SetActive(true);
    }
    public void ShowLivingSituation()
    {
        HideAllPanels();

        foreach (Transform child in OverviewGridContainer.transform)
            Destroy(child.gameObject);

        foreach (PersonaData persona in GameplayManager.Instance.Personas)
        {
            GameObject newOBJ = GameObject.Instantiate(OverviewSpeciesUIPrefab, OverviewGridContainer.transform);
            newOBJ.GetComponent<OverviewSpeciesUIController>().Init(persona.PersonaIcon, persona.PersonaTraits.whereTheyLiveNow);
        }

        IntroText.text = $"Living Situation";
        TopicText.text = string.Empty;
        CenturalTopicText.text = string.Empty;
        SurveyText.text = string.Empty;

        Panels[0].SetActive(true);
    }
    public void ShowFamily()
    {
        HideAllPanels();

        foreach (Transform child in OverviewGridContainer.transform)
            Destroy(child.gameObject);

        foreach (PersonaData persona in GameplayManager.Instance.Personas)
        {
            GameObject newOBJ = GameObject.Instantiate(OverviewSpeciesUIPrefab, OverviewGridContainer.transform);
            newOBJ.GetComponent<OverviewSpeciesUIController>().Init(persona.PersonaIcon, persona.PersonaTraits.family);
        }

        IntroText.text = $"Family";
        TopicText.text = string.Empty;
        CenturalTopicText.text = string.Empty;
        SurveyText.text = string.Empty;

        Panels[0].SetActive(true);
    }
    
    void ClearChart()
    {
        barChart.DataSource.ClearCategories();
        barChart.DataSource.ClearGroups();
    }

    void HideAllPanels()
    {
        foreach (GameObject obj in Panels)
            obj.SetActive(false);
    }

    public void OpenUI(RegionData regionData, TopicData topicData)
    {
        CurrentRegion = regionData;
        CurrentTopic = topicData;
        OnShowOverview();
        UICanvas.SetActive(true);
    }
    public void CloseUI()
    {
        UICanvas.SetActive(false);
    }
}
