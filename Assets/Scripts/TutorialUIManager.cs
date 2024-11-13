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
    [SerializeField] TextMeshProUGUI Introtext;

    [SerializeField] GameObject TutorialPanel;
    [SerializeField] TextMeshProUGUI Tutorialtext;
    [SerializeField] GameObject AnimalOBJContainer;
    [SerializeField] GameObject AnimalOBJ;
    public List<GameObject>AnimalPrefabs = new List<GameObject>();
    private List<GameObject> shuffledAnimals = new List<GameObject>();
    private int currentIndex = 0;

    //Results
    [SerializeField] GameObject ResultsPanel;
    [SerializeField] GameObject ResultsPanel2;
    [SerializeField] TextMeshProUGUI Resultstext;
    [SerializeField] TextMeshProUGUI Resultstext2;
    [SerializeField] GameObject ResultsPersonaContainer;

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
        StartCoroutine(InitTutorial(GameScreenType.Intro));
        yield return new WaitUntil(() => TutorialFinished);

        RegionSelectionManager.Instance.EnableRegionSelection();
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
        ContinueButton.onClick.AddListener(ContinueButton.GetComponent<ButtonClick>().OnButtonClick);
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
        string results = $"<color=#F3C44D>0%</color>";
        Resultstext.text = results;
        float elapsedTime = 0;

        if (GameplayManager.Instance.GetFinalScore() >= 50)
            Resultstext2.text = $"Congratulations, you are the next president of Animal Nation.";
        else
            Resultstext2.text = $"Oh no!, better luck next time!";
        Resultstext2.gameObject.SetActive(false);

        foreach (Transform child in ResultsPersonaContainer.transform)
            Destroy(child.gameObject);

        foreach (GameObject animal in AnimalPrefabs)
        {
            GameObject NewAnimal = GameObject.Instantiate(animal, ResultsPersonaContainer.transform);
        }

        ResultsPanel2.SetActive(true);
        ShowContinueButton(false);
        yield return new WaitUntil(() => Continue);

        ResultsPanel2.SetActive(false);
        ResultsPanel.SetActive(true);

        float countDuration = 1f; // Duration for counting up from 0 to targetScore
        float currentScore = 0;
        elapsedTime = 0;
        float targetScore = GameplayManager.Instance.GetFinalScore();
        while (elapsedTime < countDuration)
        {
            currentScore = Mathf.Lerp(0, targetScore, elapsedTime / countDuration);
            Resultstext.text = $"<color=#F3C44D>{Mathf.RoundToInt(currentScore)}%</color>";
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // Ensure it ends exactly at the target score
        Resultstext.text = $"<color=#F3C44D>{Mathf.RoundToInt(targetScore)}%</color>";

        Resultstext2.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);

        TutorialUIManager.Instance.ShowContinueButton(true);
        yield return new WaitUntil(() => TutorialUIManager.Instance.Continue);

        ResultsPanel.SetActive(false);
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
        ContinueButton.onClick.AddListener(ContinueButton.GetComponent<ButtonClick>().OnButtonClick);
        ContinueButton.onClick.AddListener(OnContinueClicked);

        ShowUI();

        yield return new WaitUntil(() => Continue);
        if (hideUI)
            HideUI();
        yield return new WaitForSeconds(0.2f);
        Continue = false;
    }
}
