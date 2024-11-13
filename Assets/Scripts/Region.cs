using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Region : MonoBehaviour
{
    [field: SerializeField] public string RegionID { get; private set; }
    [field: SerializeField] public RegionData RegionData { get; private set; } // Use this to get region info

    public bool IsSelected = false;

    [SerializeField] Material hoveredMaterial;
    [SerializeField] Material selectedMaterial;
    [SerializeField] Material defaultMaterial;

    [SerializeField] RegionSelectionSettingsSO settings;

    [field: SerializeField] public TopicData regionTopic {  get; private set; }

    public RegionInfoUI regionInfoUI;
    public AudioSource audioSource;
    public GameObject RegionTarget;
    //public Region3DPieChart pieChart;
    public Region3DBarChart barChart;

    [SerializeField] GameObject RegionOBJ;
    [SerializeField]Renderer regionRenderer;


    private void Awake()
    {
        defaultMaterial = regionRenderer.material;
        audioSource = GetComponent<AudioSource>();
        RegionTarget = transform.GetChild(0).gameObject;
        barChart = GetComponent<Region3DBarChart>();
        RegionOBJ = transform.GetChild(1).gameObject;
        regionRenderer = RegionOBJ.GetComponent<Renderer>();
    }

    private void Start()
    {
        settings = RegionSelectionManager.Instance.settings;
        RegionData = GameDictionary.Instance.GetRegionDataFromRegionID(RegionID);
        regionTopic = GameDictionary.Instance.GetTopicDataFromTopicID(RegionData.TopicID);
        //RegionData.HasHadRally = GameplayManager.Instance.RegionsRallied.Contains(RegionData);
        regionInfoUI = GetComponentInChildren<RegionInfoUI>();
        regionInfoUI.InitUI(RegionData, regionTopic, GetHeighestPopulationGroup());
        RegionSelectionManager.Instance.AddRegion(this);
    }

    public void OnHovered()
    {
        StopAllCoroutines();
        PlayClip(settings.OnHoverClip);
        if (IsSelected == false)
        {
            barChart.Update3DPieChart(RegionData.PopulationData);
            ChangeMaterials(hoveredMaterial);
            Vector3 start = RegionOBJ.transform.position;//this.transform.localPosition;
            Vector3 end = new Vector3(start.x, settings.MoveDistance / 2, start.z);
            StartCoroutine(MoveObjectCoroutine(start, end, settings.MoveTime, false));
        }
        regionInfoUI.ShowUI();
        Debug.Log("OnHovered: " + RegionData.RegionID);
    }
    public void OnSelected()
    {
        if (IsSelected && !RegionData.HasHadRally)
        {
            StartCoroutine(OnSelectRegionCoroutine());
            return;
        }
        else if (IsSelected && RegionData.HasHadRally)
        {
            PlayClip(settings.OnCantSelectClip);
            return;
        }
        PlayClip(settings.OnSelectClip);
        regionInfoUI.ShowUI();
        IsSelected = true;
        ChangeMaterials(selectedMaterial);
        Vector3 start = RegionOBJ.transform.position;//this.transform.localPosition;
        Vector3 end = new Vector3(start.x, settings.MoveDistance, start.z);
        StartCoroutine(MoveObjectCoroutine(start, end, settings.MoveTime, false));
        Debug.Log("OnSelected: " + RegionData.RegionID);
    }
    public void OnDeselected()
    {
        IsSelected = false;
        StopAllCoroutines();
        regionInfoUI.HideUI();
        barChart.ClearChart();
        ChangeMaterials(defaultMaterial);
        Vector3 start = RegionOBJ.transform.position;//this.transform.localPosition;
        Vector3 end = new Vector3(start.x, 0, start.z);
        StartCoroutine(MoveObjectCoroutine(start, end, settings.MoveTime, false));
        Debug.Log("OnDeSelected/OnHoverExited: " + RegionData.RegionID);
    }

    public void ChangeMaterials(Material newMaterial)
    {
        regionRenderer.material = newMaterial;
    }


    private IEnumerator MoveObjectCoroutine(Vector3 localStart, Vector3 localEnd, float duration, bool _bounce)
    {
        //Pause or audio to play instantly and not lag behind
        yield return null;

        Vector3 startPosition = localStart;
        Vector3 targetPosition = localEnd;
        float elapsedTime = 0f;

        // First phase: Move to the end position
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            t = t * t * (3f - 2f * t); // Cubic ease-in-out formula for smooth acceleration and deceleration
            //transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
            RegionOBJ.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the object reaches the exact target position
        //transform.localPosition = targetPosition;
        RegionOBJ.transform.position = targetPosition;
        
        if (_bounce)
        {
            // Reset elapsed time for the bounce back phase
            elapsedTime = 0f;

            // Second phase: Bounce back to the start position
            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                t = t * t * (3f - 2f * t); // Reuse the cubic ease-in-out formula for the return
                //transform.localPosition = Vector3.Lerp(targetPosition, startPosition, t);
                RegionOBJ.transform.position = Vector3.Lerp(targetPosition, startPosition, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure the object returns to the exact start position
            RegionOBJ.transform.position = startPosition;
        }
    }

    public float GetPersonaPercentage(PersonaData _persona)
    {
        // Get the total population from the PopulationData dictionary
        int totalPopulation = RegionData.PopulationData.Values.Sum();

        // Get the population count for the specified persona
        if (RegionData.PopulationData.TryGetValue(_persona.PersonaTraits.species, out int personaPopulationCount))
        {
            // Check to avoid division by zero
            if (totalPopulation > 0)
            {
                // Calculate the percentage
                float percentage = (float)personaPopulationCount / totalPopulation * 100f;
                //Debug.Log($"Percentage for {_persona.PersonaTraits.species}: {personaPopulationCount} / {totalPopulation} * 100 = {percentage}");
                return percentage;
            }
        }

        Debug.LogError("No persona population found or total population is zero! Returning -1...");
        return -1;
    }

    public string GetHeighestPopulationGroup()
    {
        int highestPopulation = 0;
        string highestGroup = null;

        foreach (var kvp in RegionData.PopulationData)
        {
            string species = kvp.Key;
            int populationCount = kvp.Value;

            if (populationCount > highestPopulation)
            {
                highestPopulation = populationCount;
                highestGroup = species;
            }
        }

        return highestGroup != null
            ? $"{highestGroup} with {highestPopulation} individuals"
            : "No population data available.";
    }

    IEnumerator OnSelectRegionCoroutine()
    {
        PlayClip(settings.OnConfirmClip);
        RegionSelectionManager.Instance.DisableRegionSelection();
        Vector3 start2 = RegionOBJ.transform.position;//this.transform.localPosition;
        Vector3 end2 = new Vector3(start2.x, settings.MoveDistance / 2, start2.z);
        StartCoroutine(MoveObjectCoroutine(start2, end2, settings.MoveTime / 2, true));

        yield return new WaitForSeconds(settings.DelayBetweenRegionSelectedAndBeginLevel);

        CameraMovementManager.Instance.OnCameraMovementFinishedEvent.AddListener(OnSelectRegionSequenceFinished);
        CameraMovementManager.Instance.MoveCamera(Camera.main, RegionTarget.transform.position, true);
        yield return null;
    }

    void OnSelectRegionSequenceFinished()
    {
        CameraMovementManager.Instance.OnCameraMovementFinishedEvent.RemoveListener(OnSelectRegionSequenceFinished);
        RegionData.HasHadRally = true;
        GameplayManager.Instance.SelectRegion(this);

        GameplayManager.Instance.CurrentRegion = RegionData;
        GameplayManager.Instance.CurrentTopic = regionTopic;
    }

    void PlayClip(AudioClip _clip)
    {
        audioSource.PlayOneShot(_clip);
    }
}