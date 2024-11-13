using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance;

    [field: SerializeField] public GameplayState gameplayState { get; private set; }

    [field: SerializeField] public List<PersonaData> Personas { get; private set; }
    [field: SerializeField] public TopicData SelectedTopic { get; private set; }
    public int StatementIndex { get; set; } = -1;
    [field: SerializeField] public int MaxStatementIndex { get; private set; } = 2;
    [field: SerializeField] public List<int> AllScores { get; private set; }
    [field: SerializeField] public List<int> AllFinalScores { get; private set; }

    //[field: SerializeField] public List<RegionData> RegionsRallied { get; private set; } = new List<RegionData>();

    [SerializeField] Region SelectedRegion;
    public GameObject PromptUICanvas;

    public RegionData CurrentRegion;
    public TopicData CurrentTopic;

    public bool HasRallied1stRegion = false;
    /*{
        get { return RegionsRallied.Count > 0; }
        private set {}
    }*/

    private void Awake()
    {
        //set singleton instance
        if (Instance != null)
            Destroy(this.gameObject);
        Instance = this;

        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        Personas = GameDictionary.Instance.Personas;
        SwitchState(GameplayState.Menu);
    }

    public void SwitchState(GameplayState newState)
    {
        if (gameplayState == newState)
        {
            Debug.LogWarning($"[GameplayManager] Attempted to switch GameplayState to the current GameplayState. Ignoring...");
            return;
        }

        gameplayState = newState;
        switch(newState)
        {
            case GameplayState.Rally:
                StartCoroutine(RallyCoroutine());
                break;
            case GameplayState.RegionSelection:
                StartCoroutine(RegionSelectionCoroutine());
                break;
            case GameplayState.Menu:
                break;
            case GameplayState.EndOfRally:
                StartCoroutine(EndOfRallyCoroutine());
                break;
            case GameplayState.Restarting:
                StartCoroutine(RestartingGameCoroutine());
                break;
        }
    }

    IEnumerator RallyCoroutine()
    {
        StatementIndex = 0;
        StartCoroutine(RallyManager.Instance.StartRally());
        yield return null;
    }


    IEnumerator EndOfRallyCoroutine()
    {
        //RegionsRallied.Add(SelectedRegion.RegionData);
        HasRallied1stRegion = true;
        SelectedRegion.RegionData.HasHadRally = true;
        int score = GetRallyScore();
        AllFinalScores.Add(score);
        SelectedRegion.RegionData.PercentageOfWinning = score;
        StartCoroutine(ResetCoroutine());
        LevelManager.Instance.LoadLevel(LevelManager.RegionSelectionScene);
        yield return null;
    }

    IEnumerator ResetCoroutine()
    {
        StatementIndex = -1;
        AllScores.Clear();
        foreach (PersonaData persona in Personas)
        {
            persona.History.Clear();
            persona.chatHistories.Clear();
        }
        GeminiAPIManager.Instance.Restart();
        yield return null;
    }

    IEnumerator RestartingGameCoroutine()
    {
        StatementIndex = -1;
        AllScores.Clear();
        foreach (PersonaData persona in Personas)
        {
            persona.History.Clear();
            persona.chatHistories.Clear();
        }
        GeminiAPIManager.Instance.Restart();
        RegionSelectionManager.Instance.Restart();
        yield return null;
        OnRestartingGameComplete();
    }

    IEnumerator RegionSelectionCoroutine()
    {
        CameraMovementManager.Instance.Restart();

        if (!HasRallied1stRegion)
            StartCoroutine(TutorialUIManager.Instance.InitIntro());
        else
            RegionSelectionManager.Instance.EnableRegionSelection();

        yield return null;
    }


    public void OnRestartingGameComplete()
    {
        SwitchState(GameplayState.Menu);
    }
    public void OnRegionSelectionComplete()
    {
        LevelManager.Instance.LoadLevel(LevelManager.RallyScene);
    }

    public bool AllRegionsRallied()
    {
        bool allregionRallied = true;
        foreach (RegionData region in GameDictionary.Instance.Regions)
            if (!region.HasHadRally)
                allregionRallied = false;

        return allregionRallied;
    }

    public void OnRestartingGame()
    {
        SwitchState(GameplayState.Restarting);
    }

    public string GetNextQuestion()
    {
        return SelectedTopic.Questions[StatementIndex + 1];
    }
    public string  GetCurrentQuestion()
    {
        return SelectedTopic.Questions[StatementIndex];
    }
    public string GetCurrentTopic()
    {
        return SelectedTopic.Topic;
    }
    public void CalculateOverallScore(int _statementIndex)
    {
        int score = 0;
        //Debug.Log($"Total Personas: {Personas.Count}");

        // Iterate through each persona and calculate the score
        foreach (PersonaData persona in Personas)
        {
            int personaWeightedScore = persona.GetWeightedScore(_statementIndex);
            float personaRegionPercentage = SelectedRegion.GetPersonaPercentage(persona) / 100f; // Convert to a fraction

            // Calculate the contribution of this persona
            float fScore = personaWeightedScore * personaRegionPercentage;

            // Add the rounded result to the total score
            score += Mathf.RoundToInt(fScore);

            //Debug.Log($"[PersonaWeightedScore]: {personaWeightedScore}");
            //Debug.Log($"[PersonaRegionPercentage]: {personaRegionPercentage}");
            //Debug.Log($"[fScore]: {fScore}");
            //Debug.Log($"[Current Total Score]: {score}");
        }

        // Multiply by 10 to get a score out of 100
        score *= 10; // Adjust the score to be out of 100

        // Cap the score to a maximum of 100
        score = Mathf.Min(score, 100);

        AllScores.Add(score);
    }
    public int GetRallyScore()
    {
        // Check if AllScores is null or empty to prevent errors
        if (AllScores == null || AllScores.Count == 0)
        {
            Debug.LogWarning($"[GetRallyScore] AllScores list is NULL or Empty...returning the devils number...\n" + devilAsciiArt);
            return 666; // or some default value you prefer
        }
        // Calculate the average and cast to int (truncates the decimal)
        return (int)AllScores.Average();
    }

    public int GetFinalScore()
    {
        AllFinalScores.Clear();
        // Calculate the average and cast to int (truncates the decimal)
        foreach (RegionData data in GameDictionary.Instance.Regions)
            AllFinalScores.Add(data.PercentageOfWinning);
        return (int)AllFinalScores.Average();
    }

    public void SelectRegion(Region _region)
    {
        if (_region != null)
        {
            SelectedRegion = _region;
            SelectedTopic = _region.regionTopic;
            OnRegionSelectionComplete();
        }
    }

    string devilAsciiArt = @"
               ,     ,
              (\____/)
               (_oo_)
                 (O)
               __||__    \)
            []/______\[] /
            / \______/ \/
           /    /__\
          (\   /____\
";
}

public enum GameplayState
{
    Idle,
    Menu,
    RegionSelection,
    Rally,
    EndOfRally,
    Results,
    GameOver,
    Restarting
}
