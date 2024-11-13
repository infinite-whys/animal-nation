using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TopicData
{
    [field: SerializeField] public string TopicID { get; set; }
    [field: SerializeField] public string Topic { get; set; }

    [field: SerializeField] public List<string> Questions { get; set; }

    public TopicData(GameSheet gameSheet, int i)
    {
        TopicID = gameSheet.GetValue(i, GameDictionaryReference.TopicColumns.TopicID)?.ToString();
        Topic = gameSheet.GetValue(i, GameDictionaryReference.TopicColumns.Topic)?.ToString();
        Questions = new List<string>
        {
            gameSheet.GetValue(i, GameDictionaryReference.TopicColumns.Q1)?.ToString(),
            gameSheet.GetValue(i, GameDictionaryReference.TopicColumns.Q2)?.ToString(),
            gameSheet.GetValue(i, GameDictionaryReference.TopicColumns.Q3)?.ToString()
        };
    }
}
