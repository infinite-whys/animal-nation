using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]public class RegionData
{
    [field: SerializeField] public string RegionID { get; private set; }
    [field: SerializeField] public string RegionName { get; private set; }
    [field: SerializeField] public string TopicID { get; private set; }
    public bool HasHadRally = false;
    public int PercentageOfWinning = 0;

    public Dictionary<string, int> PopulationData { get; private set; }

    public RegionData(GameSheet gameSheet, int i)
    {
        RegionID = gameSheet.GetValue(i, GameDictionaryReference.RegionColumns.RegionID)?.ToString();
        RegionName = gameSheet.GetValue(i, GameDictionaryReference.RegionColumns.RegionName)?.ToString();
        TopicID = gameSheet.GetValue(i, GameDictionaryReference.RegionColumns.TopicID)?.ToString();
        PopulationData = ParsePopulationJson(gameSheet.GetValue(i, GameDictionaryReference.RegionColumns.Population)?.ToString());
        HasHadRally = false;

        //foreach (KeyValuePair<string, int> keyValuePair in PopulationData)
            //Debug.Log($"[PopulationData] species(key): '{keyValuePair.Key}', Population(value): '{keyValuePair.Value}'.");
    }

    private Dictionary<string, int> ParsePopulationJson(string populationJson)
    {
        Dictionary<string, int> populationData = new Dictionary<string, int>();

        if (!string.IsNullOrEmpty(populationJson))
        {
            try
            {
                // Deserialize directly into a dictionary
                populationData = JsonConvert.DeserializeObject<Dictionary<string, int>>(populationJson);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to parse population JSON for region '{RegionName}': {ex.Message}");
            }
        }
        else
        {
            Debug.LogWarning($"Population JSON is null or empty for region '{RegionName}'.");
        }

        return populationData; // Return the populated dictionary
    }
}