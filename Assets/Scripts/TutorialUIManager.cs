using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUIManager : MonoBehaviour
{
    public static TutorialUIManager Instance;

    [SerializeField] GameObject UI;
    
    [SerializeField] Button ContinueButton;

    [SerializeField] GameObject IntroPanel;
    [SerializeField] TMP_Text Introtext;

    [SerializeField] GameObject TutorialPanel;
    [SerializeField] TMP_Text Tutorialtext;
    [SerializeField] GameObject AnimalOBJContainer;
    [SerializeField] GameObject AnimalOBJ;
    public List<GameObject>AnimalPrefabs = new List<GameObject>();
    private List<GameObject> shuffledAnimals = new List<GameObject>();
    private int currentIndex = 0;
    //Results

    [SerializeField] GameObject ResultsPanel;
    [SerializeField] TMP_Text Resultstext;

    [field: SerializeField] public bool Continue { get; private set; } = false;

    [SerializeField] List<TutorialData> ActiveTutorialDatas = new List<TutorialData>();

    public bool TutorialFinished = false;
    public bool ResultsFinished = false;

    private void Awake()
    {
        if (Instance != null)
            Destroy(this.gameObject);
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        ShuffleAnimals();
    }


    public IEnumerator InitIntro()
    {
        /*TutorialPanel.SetActive(false);

        ActiveTutorialDatas.Clear();
        ActiveTutorialDatas = DialogueManager.Instance.GetDialogues(GameScreenType.Intro);
        Introtext.text = ActiveTutorialDatas[0].Dialogue;

        ContinueButton.onClick.RemoveAllListeners();
        ContinueButton.onClick.AddListener(OnIntroContinueClicked);

        IntroPanel.SetActive(true);
        ShowUI();*/
        StartCoroutine(InitTutorial(GameScreenType.Intro));
        yield return new WaitUntil(() => TutorialFinished);

        RegionSelectionManager.Instance.EnableRegionSelection();
        //StartCoroutine(InitTutorial(GameScreenType.RegionSelection));
    }

    void OnIntroContinueClicked()
    {
        IntroPanel.SetActive(false);
        StartCoroutine(InitTutorial(GameScreenType.RegionSelection));
    }

    public IEnumerator InitTutorial(GameScreenType gameScreenType)
    {
        //Debug.Log("Starting tutorial: " + gameScreenType);
        TutorialFinished = false;
        Continue = false;
        IntroPanel.SetActive(false);

        ActiveTutorialDatas.Clear();
        ActiveTutorialDatas = DialogueManager.Instance.GetDialogues(gameScreenType);
        
        ContinueButton.onClick.RemoveAllListeners();
        ContinueButton.onClick.AddListener(OnContinueClicked);

        if (gameScreenType ==GameScreenType.RegionSelection)
            RegionSelectionManager.Instance.SelectRegionByRegionID("0");

        TutorialPanel.SetActive(true);
        ShowUI();

        while (ActiveTutorialDatas.Count > 0)
        {
            if (AnimalOBJ != null)
                Destroy(AnimalOBJ);
            AnimalOBJ = GameObject.Instantiate(GetRandomAnimal(), AnimalOBJContainer.transform);

            Tutorialtext.text = ActiveTutorialDatas[0].Dialogue;

            yield return new WaitUntil(() => Continue);
            ActiveTutorialDatas.RemoveAt(0);
            Continue = false;
        }
        if (gameScreenType == GameScreenType.RegionSelection)
        {
            RegionSelectionManager.Instance.Restart();
            RegionSelectionManager.Instance.EnableRegionSelection();
        }
        TutorialFinished = true;
        HideUI();
        yield return null;
        TutorialFinished = false;
    }

    GameObject GetRandomAnimal()
    {
        // If we've used all animals, reshuffle the list
        if (currentIndex >= shuffledAnimals.Count)
        {
            ShuffleAnimals();
            currentIndex = 0;
        }

        // Get the next animal in the shuffled list and increment the index
        GameObject selectedAnimal = shuffledAnimals[currentIndex];
        currentIndex++;
        return selectedAnimal;
    }
    void ShuffleAnimals()
    {
        // Create a shuffled copy of the AnimalPrefabs list
        shuffledAnimals = new List<GameObject>(AnimalPrefabs);

        // Use Fisher-Yates shuffle algorithm
        for (int i = shuffledAnimals.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            GameObject temp = shuffledAnimals[i];
            shuffledAnimals[i] = shuffledAnimals[randomIndex];
            shuffledAnimals[randomIndex] = temp;
        }
    }

    public IEnumerator ShowResults()
    {
        ResultsFinished = false;
        ShowUI();
        string results = $"{GameplayManager.Instance.GetFinalScore()}%";
        Resultstext.text = results;

        Vector3 startPosition = ResultsPanel.transform.position;
        Vector3 endPosition = new Vector3(startPosition.x, startPosition.y - 800, startPosition.z);
        float duration = 1f; // Time to move in each direction

        // Move up to the 'top' position
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            ResultsPanel.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        ResultsPanel.transform.position = endPosition; // Ensure it ends exactly at 'top'

        TutorialUIManager.Instance.ShowContinueButton(false);
        yield return new WaitUntil(() => TutorialUIManager.Instance.Continue);

        // Move back to the 'startPosition'
        elapsedTime = 0;
        while (elapsedTime < duration)
        {
            ResultsPanel.transform.position = Vector3.Lerp(endPosition, startPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        ResultsPanel.transform.position = startPosition; // Ensure it ends exactly at 'startPosition'
        ResultsFinished = true;
    }

    void OnContinueClicked()
    {
        Continue = true;
    }

    void ShowUI()
    {
        UI.SetActive(true);
    }
    void HideUI()
    {
        UI.SetActive(false);
    }

    public void ShowContinueButton(bool hideUI)
    {
        StartCoroutine(WaitOnContinue(hideUI));
    }

    IEnumerator WaitOnContinue(bool hideUI)
    {
        Continue = false;

        TutorialPanel.SetActive(false);
        IntroPanel.SetActive(false);
        ContinueButton.onClick.RemoveAllListeners();
        ContinueButton.onClick.AddListener(OnContinueClicked);

        ShowUI();

        yield return new WaitUntil(() => Continue);
        if (hideUI)
            HideUI();
        yield return new WaitForSeconds(0.2f);
        Continue = false;
    }
}
