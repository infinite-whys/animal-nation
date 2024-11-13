using System.Collections.Generic;
using UnityEngine;

public static class GameDictionaryReference
{
    public static class TableNames
    {
        public static string Personas => "Personas";
        public static string Regions => "Regions";
        public static string Topics => "Topics";
        public static string Tutorials => "Tutorials";
    }

    public static class PersonaColumns
    {
        public static string PersonaName => "PersonaName";
        public static string Species => "Species";
        public static string Preferences => "Preferences";
        public static string WhereTheyLiveNow => "WhereTheyLiveNow";
        public static string PainPoints => "PainPoints";
        public static string Income => "Income";
        public static string Occupation => "Occupation";
        public static string AgeRange => "AgeRange";
        public static string Education => "Education";
        public static string Family => "Family";
        public static string Icon => "Icon";
    }

    public static class RegionColumns
    {
        public static string RegionID => "RegionID";
        public static string RegionName => "RegionName";
        public static string TopicID => "TopicID";
        public static string Population => "Population";
    }

    public static class TopicColumns
    {
        public static string TopicID => "TopicID";
        public static string Topic => "Topic";
        public static string Q1 => "Q1";
        public static string Q2 => "Q2";
        public static string Q3 => "Q3";
    }

    public static class TutorialColumns
    {
        public static string Speaker => "Speaker";
        public static string DisplayName => "DisplayName";
        public static string Dialogue => "Dialogue";
        public static string Screen => "Screen";
        public static string Order => "Order";
    }
}

public class GameDictionary : MonoBehaviour
{
    public static GameDictionary Instance { get; private set; }

    [field: SerializeField] public List<TopicData> Topics { get; private set; }
    [field: SerializeField] public List<RegionData> Regions { get; private set; }
    [field: SerializeField] public List<PersonaData> Personas { get; private set; }
    [field: SerializeField] public List<TutorialData> Tutorials { get; private set; }

    private void Awake()
    {
        if (Instance != null)
            Destroy(this.gameObject);
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        LoadData();
    }

    private void Start()
    {

    }

    public void Restart()
    {
        Topics.Clear();
        Regions.Clear();
        Personas.Clear();
        Tutorials.Clear();

        Debug.Log("All Data Cleared...");
        Debug.Log($"Topics: {Topics.Count}");
        Debug.Log($"Regions: {Regions.Count}");
        Debug.Log($"Personas: {Personas.Count}");
        Debug.Log($"Tutorials: {Tutorials.Count}");

        LoadData();
    }

    public void LoadData()
    {
        SheetReader sheetReader = new SheetReader();

        GetSheetData(sheetReader, GameDictionaryReference.TableNames.Topics);
        GetSheetData(sheetReader, GameDictionaryReference.TableNames.Regions);
        GetSheetData(sheetReader, GameDictionaryReference.TableNames.Personas);
        GetSheetData(sheetReader, GameDictionaryReference.TableNames.Tutorials);
    }

    private void GetSheetData(SheetReader sheetReader, string range)
    {
        GameSheet gameSheet = new GameSheet((List<List<object>>)sheetReader.getSheetRange(range));
        for (int i = 0; i < gameSheet.data.Count; i++)
        {
            if (range == GameDictionaryReference.TableNames.Topics)
            {
                TopicData topic = new TopicData(gameSheet, i);
                Topics.Add(topic);
            }
            else if (range == GameDictionaryReference.TableNames.Regions)
            {
                RegionData region = new RegionData(gameSheet, i);
                Regions.Add(region);
            }
            else if (range == GameDictionaryReference.TableNames.Personas)
            {
                PersonaData persona = new PersonaData(gameSheet, i);
                Personas.Add(persona);
            }
            else if (range == GameDictionaryReference.TableNames.Tutorials)
            {
                TutorialData tutorial = new TutorialData(gameSheet, i);
                Tutorials.Add(tutorial);
            }
        }
    }


    public RegionData GetRegionDataFromRegionID(string regionID)
    {
        foreach (RegionData regionData in Regions)
            if (regionData.RegionID == regionID)
                return regionData;

        Debug.LogWarning($"[GameDictionary] No RegionData found for ID: '{regionID}'.");
        return null;
    }

    public TopicData GetTopicDataFromTopicID(string topicID)
    {
        foreach (TopicData topicData in Topics)
            if (topicData.TopicID == topicID)
                return topicData;

        Debug.LogWarning($"[GameDictionary] No TopicData found for ID: '{topicID}'.");
        return null;
    }


    public List<TutorialData> GetTutorialDatasByGameScreenType(GameScreenType gameScreenType)
    {
        List<TutorialData> datas = new List<TutorialData>();
        foreach (TutorialData data in Tutorials)
            if (data.GameScreen == gameScreenType)
                datas.Add(data);
        return datas;
    }
}