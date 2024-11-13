using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuEvents : MonoBehaviour
{
    [SerializeField] MainMenuSettingsSO settings;

    //UIDocument uiDoc;

    [SerializeField] AudioSource audioSource;

    public Button startButton;
    public List<Button> menuButtons = new List<Button>();

    private void Awake()
    {
        //uiDoc = GetComponent<UIDocument>();
        audioSource = GetComponent<AudioSource>();

        //startButton = uiDoc.rootVisualElement.Q("Start") as Button;
        startButton.onClick.AddListener(OnStartClicked);

        //menuButtons = uiDoc.rootVisualElement.Query<Button>().ToList();
        foreach (var button in menuButtons)
        {
            button.onClick.AddListener(OnAllButtonsClicked);
            //button.RegisterCallback<MouseOverEvent>(OnAllButtonsHovered);
        }
        startButton.image.alphaHitTestMinimumThreshold = 0.1f;
        InvokeRepeating("BounceButton", 0f, 3f);
    }

    private void OnDisable()
    {
        startButton.onClick.RemoveListener(OnStartClicked);
        foreach (var button in menuButtons)
        {
            button.onClick.RemoveListener(OnAllButtonsClicked);
            //button.UnregisterCallback<MouseOverEvent>(OnAllButtonsHovered);
        }
    }

    void OnStartClicked()
    {
        StartCoroutine(LoadRegionSelectionCoroutine());
    }

    void OnAllButtonsClicked()
    {
        PlayOneShot(settings.OnConfirmClip);
    }
    void OnAllButtonsHovered()
    {
        PlayOneShot(settings.OnHoverClip);
    }

    void PlayOneShot(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    IEnumerator LoadRegionSelectionCoroutine()
    {
        yield return new WaitForSeconds(settings.pausebeforeLoadRegionSelection);

        LevelManager.Instance.LoadLevel(LevelManager.RegionSelectionScene);
    }

    void BounceButton()
    {
        StartCoroutine(BounceButtonCoroutine());
    }

    IEnumerator BounceButtonCoroutine()
    {
        Vector3 startPosition = startButton.transform.position;
        Vector3 top = new Vector3(startPosition.x, startPosition.y + 25, startPosition.z);
        float duration = 0.25f; // Time to move in each direction

        // Move up to the 'top' position
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            startButton.transform.position = Vector3.Lerp(startPosition, top, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        startButton.transform.position = top; // Ensure it ends exactly at 'top'

        // Move back to the 'startPosition'
        elapsedTime = 0;
        while (elapsedTime < duration)
        {
            startButton.transform.position = Vector3.Lerp(top, startPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        startButton.transform.position = startPosition; // Ensure it ends exactly at 'startPosition'
    }

    public void OnQuit()
    {
        Application.Quit();
    }
}
