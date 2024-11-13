using System;
using UnityEngine;

[System.Serializable]
public class TutorialData
{
    [field: SerializeField] public string Speaker {  get; set; }
    [field: SerializeField] public string DisplayName {  get; set; }
    [field: SerializeField] public GameScreenType? GameScreen {  get; set; }
    [field: SerializeField] public string Dialogue { get; set; }
    [field: SerializeField] public int Order { get; set; }

    public TutorialData(GameSheet gamesheet, int i)
    {
        Speaker = gamesheet.GetValue(i, GameDictionaryReference.TutorialColumns.Speaker)?.ToString();
        DisplayName = gamesheet.GetValue(i, GameDictionaryReference.TutorialColumns.DisplayName)?.ToString();
        Dialogue = gamesheet.GetValue(i, GameDictionaryReference.TutorialColumns.Dialogue)?.ToString();
        Order = int.Parse(gamesheet.GetValue(i, GameDictionaryReference.TutorialColumns.Order)?.ToString());

        GameScreen = GetGameScreenType(gamesheet.GetValue(i, GameDictionaryReference.TutorialColumns.Screen)?.ToString());
    }

    GameScreenType? GetGameScreenType(string gameScreenString)
    {
        if (Enum.TryParse(gameScreenString, true, out GameScreenType result))
        {
            return result;
        }
        else
        {
            Debug.LogWarning($"Could not parse '{gameScreenString}' to an GameScreenType enum.");
            return null; // or handle the case as needed
        }
    }
}

public enum GameScreenType
{
    Menu,
    Intro,
    RegionSelection,
    Rally,
    Round1,
    Round2,
    Round3,
    Results1,
    Results2,
    Win,
    Lose
}