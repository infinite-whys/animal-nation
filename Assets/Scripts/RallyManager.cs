using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RallyManager : MonoBehaviour
{
    public static RallyManager Instance;

    //PersonaPrefabs
    //[SerializeField] GameObject DefaultPersonaPrefab;
    //[SerializeField] GameObject CatPrefab;

    //RoundInfoPanel
    [SerializeField] GameObject RoundInfoPanel;
    [SerializeField] TMP_Text RoundText;
    [SerializeField] TMP_Text QuestionText;

    //Personas
    [SerializeField] GameObject PersonasContainer;
    [SerializeField] List<RallyPersonaController> PersonasList;

    //Banner
    [SerializeField] TMP_Text BannerText;

    //UserInput
    [SerializeField] GameObject UserUnputPanel;
    [SerializeField] TMP_InputField InputField;
    [SerializeField] Button SayButton;
    [SerializeField] GameObject DialogueBubble;
    [SerializeField] TMP_Text InputQuestionText;
    [SerializeField] Button DemographicUIButton;

    //Results
    [SerializeField] GameObject ResultsPanel;
    [SerializeField] TMP_Text ResultsText;

    bool CreatePersonasFinished = false;
    bool RoundInfoFinished = false;
    bool ResultsFinished = false;
    bool UserHasMadeInput = false;
    bool HasFinishedDispalyingPersonaThought = false;
    bool RecievedRequest = false;

    List<PersonaData> RecievedRequests = new List<PersonaData>();

    private void Awake()
    {
        if (Instance != null)
            Destroy(this.gameObject);
        Instance = this;
    }

    private void ResetRally()
    {
        CreatePersonasFinished = false;
        RoundInfoFinished = false;
        UserHasMadeInput = false;
        RecievedRequests.Clear();
        RecievedRequest = false;
    }

    public IEnumerator StartRally()
    {
        ResetRally();

        GeminiAPIManager.Instance.NewRequestReceived.AddListener(AddRecievedRequestToList);

        BannerText.text = GameplayManager.Instance.SelectedTopic.Topic;

        StartCoroutine(CreatePersonas());

        yield return new WaitUntil(() => CreatePersonasFinished);

        yield return new WaitForSeconds(1f);

        StartCoroutine(TutorialUIManager.Instance.InitTutorial(GameScreenType.Rally));
        yield return new WaitUntil(() => TutorialUIManager.Instance.TutorialFinished);

        for (int i = 0; i <= 2; i++)
        {
            RecievedRequest = false;
            GameplayManager.Instance.StatementIndex = i;
            StartCoroutine(ShowRoundInfo(GetRoundFromInt(i)));

            yield return new WaitUntil(() => RoundInfoFinished);
            RoundInfoFinished = false;

            ShowUserInput();
            yield return new WaitUntil(() => UserHasMadeInput);
            UserHasMadeInput = false;

            yield return new WaitUntil(() => RecievedRequest);

            for (int j = 0; j < GameplayManager.Instance.Personas.Count; j++)
            {
                HasFinishedDispalyingPersonaThought = false;
                StartCoroutine(DisplayPersonaThought(RecievedRequests[j]));
                yield return new WaitUntil(() => HasFinishedDispalyingPersonaThought);
            }
            GameplayManager.Instance.CalculateOverallScore(i);
            RecievedRequests.Clear();
        }

        GeminiAPIManager.Instance.NewRequestReceived.RemoveListener(AddRecievedRequestToList);

        HideUserInput();
        yield return new WaitForSeconds(3f);

        StartCoroutine(ShowResults());
        yield return new WaitUntil(() => ResultsFinished);
        ResultsFinished = false;

        StartCoroutine(ReturnToRegionSelection());
    }

    IEnumerator ShowResults()
    {
        string results = $"{GameplayManager.Instance.GetRallyScore()}%";
        ResultsText.text = results;

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

        TutorialUIManager.Instance.ShowContinueButton(true);
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

    IEnumerator ReturnToRegionSelection()
    {
        TutorialUIManager.Instance.ShowContinueButton(true);
        yield return null;

        yield return new WaitUntil(() => TutorialUIManager.Instance.Continue);
        GameplayManager.Instance.SwitchState(GameplayState.EndOfRally);
    }

    IEnumerator CreatePersonas()
    {
        foreach (Transform child in PersonasContainer.transform)
            Destroy(child.gameObject);

        foreach (PersonaData persona in GameDictionary.Instance.Personas)
        {
            GameObject prefab = persona.PersonaPrefab;
            GameObject personaOBJ = GameObject.Instantiate(prefab, PersonasContainer.transform);
            RallyPersonaController personaController = personaOBJ.GetComponent<RallyPersonaController>();
            personaController.InitPersona(persona);
            PersonasList.Add(personaController);

            yield return new WaitForSeconds(0.5f);
        }
        CreatePersonasFinished = true;
    }

    IEnumerator ShowRoundInfo(GameScreenType gameScreenType)
    {
        List<TutorialData> dialogues = DialogueManager.Instance.GetDialogues(gameScreenType);
        RoundText.text = dialogues[0].Dialogue;
        QuestionText.text = GameplayManager.Instance.GetCurrentQuestion();

        Vector3 startPosition = RoundInfoPanel.transform.position;
        Vector3 endPosition = new Vector3(startPosition.x, startPosition.y -800, startPosition.z);
        float duration = 1f; // Time to move in each direction

        // Move up to the 'top' position
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            RoundInfoPanel.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        RoundInfoPanel.transform.position = endPosition; // Ensure it ends exactly at 'top'

        TutorialUIManager.Instance.ShowContinueButton(true);
        yield return new WaitUntil(() => TutorialUIManager.Instance.Continue);

        // Move back to the 'startPosition'
        elapsedTime = 0;
        while (elapsedTime < duration)
        {
            RoundInfoPanel.transform.position = Vector3.Lerp(endPosition, startPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        RoundInfoPanel.transform.position = startPosition; // Ensure it ends exactly at 'startPosition'

        RoundInfoFinished = true;
    }

    IEnumerator DisplayPersonaThought(PersonaData data)
    {
        foreach (RallyPersonaController persona in PersonasList)
        {
            if (persona.PersonaData == data)
            {
                persona.Talk();
                yield return new WaitUntil(() => persona.HasFinishedTalking);
            }
        }

        HasFinishedDispalyingPersonaThought = true;
    }
    
    GameScreenType GetRoundFromInt(int i)
    {
        if (i == 0)
            return GameScreenType.Round1;
        else if (i == 1)
            return GameScreenType.Round2;
        else if (i == 2)
            return GameScreenType.Round3;

        return GameScreenType.Round1;
    }

    void AddRecievedRequestToList(PersonaData data)
    {
        RecievedRequests.Add(data);
        RecievedRequest = true;
    }

    void ShowUserInput()
    {
        InputField.text = string.Empty;
        InputQuestionText.text = GameplayManager.Instance.GetCurrentQuestion();

        SayButton.onClick.AddListener(SayOnClick);

        DialogueBubble.SetActive(true);
        UserUnputPanel.SetActive(true);
    }

    void HideUserInput()
    {
        UserUnputPanel.SetActive(false);
    }

    void SayOnClick()
    {
        string prompt = InputField.text;
        GeminiAPIManager.Instance.ProvidePrompt(prompt);

        UserHasMadeInput = true;
        DialogueBubble.SetActive(false);
        SayButton.onClick.RemoveListener(SayOnClick);
    }

    public void ShowDemographicDetails()
    {
        DemographicDetialsUIController.Instance.OpenUI(GameplayManager.Instance.CurrentRegion, GameplayManager.Instance.CurrentTopic);
    }
    public void CloseDemographicUI()
    {

    }
}
