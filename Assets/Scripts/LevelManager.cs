using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public const string MainMenuScene = "MainMenu";
    public const string RegionSelectionScene = "RegionSelection";
    public const string RallyScene = "Rally";

    private void Awake()
    {
        if (Instance != null)
            Destroy(this.gameObject);
        Instance = this;

        DontDestroyOnLoad(this.gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case MainMenuScene:
                GameDictionary.Instance.Restart();
                break;
            case RegionSelectionScene:
                GameplayManager.Instance.SwitchState(GameplayState.RegionSelection);
                break;
            case RallyScene:
                GameplayManager.Instance.SwitchState(GameplayState.Rally);
                break;
        }
    }
}
