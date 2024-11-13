using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RegionSelectionManager : MonoBehaviour
{
    public static RegionSelectionManager Instance;

    // Layer mask to filter raycast to only detect objects on the "Regions" layer
    public LayerMask regionsLayerMask;

    private GameObject selectedObject;
    private GameObject lastHoveredObject;

    public RegionSelectionSettingsSO settings;

    public List<Region> Regions = new List<Region>();

    public bool CanSelect = false;
    bool StartedResults = false;

    //public bool HasHeldFirstRally => GameplayManager.Instance.RegionsRallied.Count > 0;

    private void Awake()
    {
        if (Instance != null)
            Destroy(this.gameObject);
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (!StartedResults)
        {
            //Debug.Log("Not started results");
            if (GameplayManager.Instance.AllRegionsRallied())
            {
                //Debug.Log("All regions rallied!");
                StartedResults = true;
                GameplayManager.Instance.SwitchState(GameplayState.Results);
                StartCoroutine(DisplayEndResults());
            }
        }

        if (!CanSelect)
            return;

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (GameplayManager.Instance.gameplayState != GameplayState.Results)
            if (GameplayManager.Instance.gameplayState != GameplayState.RegionSelection)
                return;


        // Check if the mouse is over any GameObject on the specified layer
        DetectHoveredObject();

        // Check for mouse click to select the hovered object
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            SelectObject();
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (selectedObject != null)
                Restart();
        }
    }

    private void DetectHoveredObject()
    {
        // Create a ray from the camera through the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Perform the raycast with the layer mask
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, regionsLayerMask))
        {
            // Get the GameObject that was hit
            GameObject hoveredObject = hit.collider.gameObject.transform.parent.gameObject;

            if (hoveredObject == lastHoveredObject)
                return;

            if (hoveredObject.GetComponent<Region>() == null)
            {
                Debug.LogWarning($"Hovered object has no Region script but is on region Layer. May be a PieChart");
                return;
            }

            if (!GameplayManager.Instance.HasRallied1stRegion)
            {
                // Only allow hover over region with RegionID "0" if no rally has been held
                if (hoveredObject.GetComponent<Region>().RegionID != "0")
                {
                    // If we are currently hovering over an object that is not RegionID "0", trigger exit
                    if (lastHoveredObject != null)
                    {
                        OnHoverExit(lastHoveredObject); // Call exit for the previous hovered object
                        lastHoveredObject = null;
                    }
                    return; // Skip the hover enter logic
                }
            }

            // If hovering over a new object, handle hover exit for the last hovered object
            if (hoveredObject != lastHoveredObject)
            {
                // Handle hover exit for the previous hovered object
                if (lastHoveredObject != null)
                {
                    OnHoverExit(lastHoveredObject);
                }

                // Handle hover enter for the current object
                OnHoverEnter(hoveredObject);
                lastHoveredObject = hoveredObject;
            }
        }
        else if (lastHoveredObject != null)
        {
            // Handle hover exit if no object is hovered
            OnHoverExit(lastHoveredObject);
            lastHoveredObject = null;
        }
    }

    public void AddRegion(Region _region)
    {
        if (!Regions.Contains(_region))
            Regions.Add(_region);
    }


    void OnHoverEnter(GameObject obj)
    {
        Debug.Log("Hover Enter: " + obj.name);
        obj.GetComponent<Region>().OnHovered();
    }
    void OnHoverExit(GameObject obj)
    {
        // Debugging hover exit to ensure it's being called correctly
        Debug.Log("Hover Exit: " + obj.name);

        // Only call OnDeselected if the object isn't selected
        if (selectedObject != obj)
        {
            obj.GetComponent<Region>().OnDeselected();
        }
    }
    void OnSelect(GameObject obj)
    {
        Debug.Log("Select: " + obj.name);
        Region region = obj.GetComponent<Region>();
        region.OnSelected();
        CameraMovementManager.Instance.MoveAndRotateCamera(Camera.main, region.regionInfoUI.UICameratarget.transform.position, 35f, false);
    }
    void OnDeselect(GameObject obj)
    {
        Debug.Log("Deselect: " + obj.name);
        obj.GetComponent<Region>().OnDeselected();
        //CameraMovementManager.Instance.MoveAndRotateCamera(Camera.main, Camera.main.transform.position, CameraMovementManager.Instance.CameraRegionSelectionPosition.transform.position, 45f, false);
    }

    private void SelectObject()
    {
        if (lastHoveredObject != null)
        {
            // If an object is hovered, handle selection
            if (selectedObject != null && selectedObject != lastHoveredObject)
            {
                // Deselect the previously selected object
                OnDeselect(selectedObject);
            }

            // Select the currently hovered object
            OnSelect(lastHoveredObject);
            selectedObject = lastHoveredObject;
        }
    }

    public void Restart()
    {
        foreach (Region region in Regions)
            region.OnDeselected();
        lastHoveredObject = null;
        selectedObject = null;
        CameraMovementManager.Instance.MoveAndRotateCamera(Camera.main, CameraMovementManager.Instance.CameraRegionSelectionPosition.transform.position, 45f, false);
    }

    public void EnableRegionSelection()
    {
        CanSelect = true;
    }
    public void DisableRegionSelection()
    {
        CanSelect = false;
    }

    public bool SelectRegionByRegionID(string regionID)
    {
        foreach (Region region in Regions)
            if (region.RegionID == regionID)
            {
                region.OnSelected();
                return true;
            }
        return false;
    }

    IEnumerator DisplayEndResults()
    {
        Debug.Log("Staring Results!!!");
        DisableRegionSelection();

        StartCoroutine(TutorialUIManager.Instance.InitTutorial(GameScreenType.Results1));
        yield return new WaitUntil(() => TutorialUIManager.Instance.TutorialFinished);

        EnableRegionSelection();

        TutorialUIManager.Instance.ShowContinueButton(true);
        yield return new WaitUntil(() => TutorialUIManager.Instance.Continue);

        DisableRegionSelection();

        StartCoroutine(TutorialUIManager.Instance.InitTutorial(GameScreenType.Results2));
        yield return new WaitUntil(() => TutorialUIManager.Instance.TutorialFinished);

        int Score = GameplayManager.Instance.GetFinalScore();
        StartCoroutine(TutorialUIManager.Instance.ShowResults());
        GameScreenType type = GameScreenType.Win;

        if (Score < 50)
            type = GameScreenType.Lose;

        yield return new WaitUntil(() => TutorialUIManager.Instance.ResultsFinished);

        StartCoroutine(TutorialUIManager.Instance.InitTutorial(type));
        yield return new WaitUntil(() => TutorialUIManager.Instance.TutorialFinished);

        yield return new WaitForSeconds(3f);
        LevelManager.Instance.LoadLevel(LevelManager.MainMenuScene);
    }
}
