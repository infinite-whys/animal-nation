using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RallyPersonaController : MonoBehaviour
{
    [SerializeField] Image PersonaImage;
    [SerializeField] TMP_Text SpeachBubbleText;

    [SerializeField] GameObject SpeachBubbleObj;

    public PersonaData PersonaData;

    public bool HasFinishedTalking = false;

    private void Awake()
    {
        SpeachBubbleObj.SetActive(false);
        SpeachBubbleText.text = string.Empty;
    }

    public void InitPersona(PersonaData personaData)
    {
        PersonaData = personaData;
        //if (personaData.PersonaTraits.species != "Cat")
            //PersonaImage.sprite = personaData.PersonaIcon;
    }

    public void Talk()
    {
        HasFinishedTalking = false;

        StartCoroutine(MovePersona());
    }

    IEnumerator MovePersona()
    {
        Vector3 startPosition = PersonaImage.transform.position;
        Vector3 top = new Vector3(startPosition.x, startPosition.y + 25, startPosition.z);
        float duration = 0.25f; // Time to move in each direction

        // Move up to the 'top' position
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            PersonaImage.transform.position = Vector3.Lerp(startPosition, top, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        PersonaImage.transform.position = top; // Ensure it ends exactly at 'top'

        // Move back to the 'startPosition'
        elapsedTime = 0;
        while (elapsedTime < duration)
        {
            PersonaImage.transform.position = Vector3.Lerp(top, startPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        PersonaImage.transform.position = startPosition; // Ensure it ends exactly at 'startPosition'

        StartCoroutine(ShowSpeachBubble());
    }

    IEnumerator ShowSpeachBubble()
    {
        // Clear the current text
        SpeachBubbleText.text = "";

        SpeachBubbleObj.SetActive(true);

        // Get the most recent reasoning text
        string fullText = PersonaData.History[PersonaData.History.Count - 1].reasoning;

        // Split the text into words
        string[] words = fullText.Split(" ");

        // Display each word one at a time
        foreach (string word in words)
        {
            // Append the next word
            SpeachBubbleText.text += word + " ";

            // Wait for a short delay before showing the next word
            yield return new WaitForSeconds(0.2f); // Adjust delay time as needed
        }

        yield return new WaitForSeconds(3f);

        SpeachBubbleObj.SetActive(false);
        
        HasFinishedTalking = true;
    }
}