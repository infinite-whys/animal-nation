using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [field: SerializeField] Dictionary<GameScreenType, List<TutorialData>> Dialogues { get; set; } = new Dictionary<GameScreenType, List<TutorialData>>();

    private void Awake()
    {
        if (Instance != null)
            Destroy(this.gameObject);
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        Dialogues.Add(GameScreenType.Intro, GameDictionary.Instance.GetTutorialDatasByGameScreenType(GameScreenType.Intro));
        Dialogues.Add(GameScreenType.RegionSelection, GameDictionary.Instance.GetTutorialDatasByGameScreenType(GameScreenType.RegionSelection));
        Dialogues.Add(GameScreenType.Rally, GameDictionary.Instance.GetTutorialDatasByGameScreenType(GameScreenType.Rally));
        Dialogues.Add(GameScreenType.Round1, GameDictionary.Instance.GetTutorialDatasByGameScreenType(GameScreenType.Round1));
        Dialogues.Add(GameScreenType.Round2, GameDictionary.Instance.GetTutorialDatasByGameScreenType(GameScreenType.Round2));
        Dialogues.Add(GameScreenType.Round3, GameDictionary.Instance.GetTutorialDatasByGameScreenType(GameScreenType.Round3));
        Dialogues.Add(GameScreenType.Results1, GameDictionary.Instance.GetTutorialDatasByGameScreenType(GameScreenType.Results1));
        Dialogues.Add(GameScreenType.Results2, GameDictionary.Instance.GetTutorialDatasByGameScreenType(GameScreenType.Results2));
        Dialogues.Add(GameScreenType.Win, GameDictionary.Instance.GetTutorialDatasByGameScreenType(GameScreenType.Win));
        Dialogues.Add(GameScreenType.Lose, GameDictionary.Instance.GetTutorialDatasByGameScreenType(GameScreenType.Lose));
    }

    public List<TutorialData> GetDialogues(GameScreenType gameScreenType)
    {
        List<TutorialData> returnDatas = Dialogues[gameScreenType].ToList();

        if (returnDatas.Count == 0)
            Debug.LogWarning($"[DialogueManager] No TutorialData found for '{gameScreenType}'. Returning Null...");
        
        return returnDatas;
    }
}