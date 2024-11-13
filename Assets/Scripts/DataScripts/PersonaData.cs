using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PersonaData
{
    [field: SerializeField] public string PersonaName { get; private set; }
    public string PersonaPrompt { get { return GetPersonaPrompt(); } }
    [field: SerializeField] public PersonaTraits PersonaTraits { get; private set; }
    [field: SerializeField] public List<PersonaCriteria> History { get; private set; } = new List<PersonaCriteria>();
    [field: SerializeField] public List<ConversationEntry> chatHistories { get; private set; } = new List<ConversationEntry>();
    public Sprite PersonaIcon;
    public Sprite PersonaAnimation;
    public GameObject PersonaPrefab
    { 
        get 
        {
            GameObject prefab = Resources.Load<GameObject>($"PersonaPrefabs/{PersonaTraits.species}");
            if (prefab == null )
                prefab = Resources.Load<GameObject>($"PersonaPrefabs/Persona");
            return prefab;
        } 
    }

    public readonly Dictionary<string, float> WeightDict = new Dictionary<string, float>()
    {
        { "benefit", 0.3f },
        { "ideologicalCompatibility", 0.2f },
        { "relatability", 0.2f },
        { "publicAppeal", 0.2f },
        { "viability", 0.1f }
    };

    public PersonaData(GameSheet gameSheet, int i)
    {
        PersonaName = gameSheet.GetValue(i, GameDictionaryReference.PersonaColumns.PersonaName)?.ToString();
        PersonaTraits = new PersonaTraits();
        PersonaTraits.name = PersonaName;
        PersonaTraits.species = gameSheet.GetValue(i, GameDictionaryReference.PersonaColumns.Species)?.ToString();
        PersonaTraits.preferences = gameSheet.GetValue(i, GameDictionaryReference.PersonaColumns.Preferences)?.ToString();
        PersonaTraits.whereTheyLiveNow = gameSheet.GetValue(i, GameDictionaryReference.PersonaColumns.WhereTheyLiveNow)?.ToString();
        PersonaTraits.painPoints = gameSheet.GetValue(i, GameDictionaryReference.PersonaColumns.PainPoints)?.ToString();
        PersonaTraits.income = gameSheet.GetValue(i, GameDictionaryReference.PersonaColumns.Income)?.ToString();
        PersonaTraits.occupation = gameSheet.GetValue(i, GameDictionaryReference.PersonaColumns.Occupation)?.ToString();
        PersonaTraits.ageRange = gameSheet.GetValue(i, GameDictionaryReference.PersonaColumns.AgeRange)?.ToString();
        PersonaTraits.education = gameSheet.GetValue(i, GameDictionaryReference.PersonaColumns.Education)?.ToString();
        PersonaTraits.family = gameSheet.GetValue(i, GameDictionaryReference.PersonaColumns.Family)?.ToString();
        string path = $"Icons/{gameSheet.GetValue(i, GameDictionaryReference.PersonaColumns.Icon)}";
        PersonaIcon = Resources.Load<Sprite>(path);
    }

    string GetPersonaPrompt()
    {
        string prompt = "You are a persona in the Animal Nation with the following traits:\n";
        prompt += PersonaTraits.GetPersonaTraitsJsonString();
        return prompt;
    }
    public int GetWeightedScore(int statementIndex)
    {
        PersonaCriteria criteria = History[statementIndex];

        // Calculate the weighted sum
        float weightedSum = 0f;
        weightedSum += criteria.benefit * WeightDict["benefit"];
        weightedSum += criteria.ideologicalCompatibility * WeightDict["ideologicalCompatibility"];
        weightedSum += criteria.relatability * WeightDict["relatability"];
        weightedSum += criteria.publicAppeal * WeightDict["publicAppeal"];
        weightedSum += criteria.viability * WeightDict["viability"];

        // Calculate weighted average without rounding
        float weightedAverage = weightedSum;

        // Scale relevance to 0-1 by dividing by 10
        float relevanceFactor = criteria.relevanceToTheTopic / 10f;

        // Apply scaled relevance factor and round at the end
        float adjustedScore = weightedAverage * relevanceFactor;
        int weightedScore = Mathf.RoundToInt(adjustedScore);

        return weightedScore;
    }
}

[System.Serializable]
public class PersonaCriteria
{
    public string name;
    public int relevanceToTheTopic;
    public int benefit;
    public int ideologicalCompatibility;
    public int relatability;
    public int publicAppeal;
    public int viability;
    public string reasoning;
    public string history;
}

[System.Serializable]
public class PersonaTraits
{
    public string name;
    public string species;
    public string preferences;
    public string whereTheyLiveNow;
    public string painPoints;
    public string income;
    public string occupation;
    public string ageRange;
    public string education;
    public string family;

    public string GetPersonaTraitsJsonString()
    {
        return JsonUtility.ToJson(this);
    }
}
