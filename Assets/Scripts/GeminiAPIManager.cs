using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class GeminiAPIManager : MonoBehaviour
{
    public static GeminiAPIManager Instance;

    const string apiURL = "https://generativelanguage.googleapis.com/v1/models/gemini-1.5-flash:generateContent?key=";
    const string JSONTemplate = @"
    {
      ""name"": String, # exactly the name of the voter,
      ""relevanceToTheTopic"": Number, # a score in the range of  1-10, How relevant the candidate's statement is to the topic.
      ""benefit"": Number, # a score in the range of  1-10, How much the statement the candidate proposes can benefit the voter.
      ""ideologicalCompatibility"": Number, # a score in the range of  1-10, How well the candidate's statement aligns with the beliefs and values of the voter.
      ""relatability"": Number, # a score in the range of  1-10, how practical this statement is for the persona.
      ""publicAppeal"": Number, # a score in the range of  1-10, The novelty, likability of the statement, and how well the statement was articulated. Plagiarism scores low.
      ""viability"": Number, # a score in the range of  1-10, How realistic the statement the candidate makes are.
      ""reasoning"": String # in no more than 10 words, summarise why or why not the voter likes the statement and include as much info from previous statements and your previous answers to those statements as possible. This must be in the 1st person singular.
      ""history"": String, # 'TRUE' if you have access to our previous message history otherwise 'FALSE'
    }

    Do not include any other text including ""```json"" before the json string and ""```"" after.";

    const string HowToUseTemplate = "Using the chat history if available, the provided statement, voter traits and JSON template generate a JSON object.";

    string URL { get { return apiURL + Resources.Load<TextAsset>("Creds/key").ToString(); } }

    public List<Response> Responses = new List<Response>();
    public bool AllResponsesRecieved { get; set; }

    // Event for when each new request is received
    public UnityEvent<PersonaData> NewRequestReceived;

    private void Awake()
    {
        if (Instance != null)
            Destroy(this.gameObject);
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void ProvidePrompt(string _prompt)
    {
        AllResponsesRecieved = false;
        Responses.Clear();
        List<string> requests = new List<string>();
        Dictionary<PersonaData, string> personaRequestPairs = new Dictionary<PersonaData, string>();

        foreach (PersonaData persona in GameplayManager.Instance.Personas)
        {
            string topic = GameplayManager.Instance.GetCurrentTopic();
            string question = GameplayManager.Instance.GetCurrentQuestion();
            string json = JsonUtility.ToJson(CreatePersonaRequest(persona, _prompt, topic, question));
            personaRequestPairs.Add(persona, json);
        }

        SendMultipleRequestsAsync(personaRequestPairs);
    }

    RequestBody CreatePersonaRequest(PersonaData _persona, string _prompt, string _topic, string _questionAsked)
    {
        string fullPrompt = _persona.PersonaPrompt + "\n" + HowToUseTemplate + "\nTemplate: " + JSONTemplate + "\nTopic: " + _topic + "\nQuestion Asked: " + _questionAsked + "\nStatement: " + _prompt;

        List<Content> contents = new List<Content>();
        foreach (var entry in _persona.chatHistories)
        {
            contents.Add(new Content
            {
                role = entry.Role,
                parts = new Part[]
                {
                    new Part { text = entry.Text }
                }
            });
        }

        contents.Add(new Content
        {
            role = "user",
            parts = new Part[]
            {
                new Part { text = fullPrompt }
            }
        });

        _persona.chatHistories.Add(new ConversationEntry { Role = "user", Text = fullPrompt });

        return new RequestBody
        {
            contents = contents.ToArray(),
            generation_config = new GenerationConfig { temperature = 0.7f, top_p = 0.9f, top_k = 50, max_output_tokens = 512 },
            safety_settings = new[]
            {
                new SafetySettings { category = "HARM_CATEGORY_SEXUALLY_EXPLICIT", threshold = "BLOCK_NONE" },
                new SafetySettings { category = "HARM_CATEGORY_HATE_SPEECH", threshold = "BLOCK_NONE" },
                new SafetySettings { category = "HARM_CATEGORY_HARASSMENT", threshold = "BLOCK_NONE" },
                new SafetySettings { category = "HARM_CATEGORY_DANGEROUS_CONTENT", threshold = "BLOCK_NONE" }
            }
        };
    }

    public async Task<Response> SendPromptAsync(string _jsonBody)
    {
        byte[] bodyRaw = Encoding.UTF8.GetBytes(_jsonBody);
        UnityWebRequest request = new UnityWebRequest(URL, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        var operation = request.SendWebRequest();
        while (!operation.isDone)
        {
            await Task.Yield();
        }

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            return null;
        }
        else
        {
            string responseText = request.downloadHandler.text;
            Response responseObj = JsonUtility.FromJson<Response>(responseText);
            Responses.Add(responseObj);
            return responseObj;
        }
    }

    public async void SendMultipleRequestsAsync(Dictionary<PersonaData, string> _personasDict)
    {
        const float delayBetweenRequests = 0.1f;
        List<Response> responses = new List<Response>();

        foreach (var kvp in _personasDict)
        {
            PersonaData persona = kvp.Key;
            string jsonBody = kvp.Value;

            Response response = await SendPromptAsync(jsonBody);
            if (response != null)
            {
                responses.Add(response);
                string personaJSON = response.candidates[0].content.parts[0].text;
                PersonaCriteria criteria = JsonUtility.FromJson<PersonaCriteria>(personaJSON);
                string personaName = criteria.name;

                foreach (PersonaData p in GameplayManager.Instance.Personas)
                {
                    if (p.PersonaName == personaName)
                    {
                        p.History.Add(criteria);
                        p.chatHistories.Add(new ConversationEntry { Role = "model", Text = personaJSON });
                        
                        // Invoke the event to notify that a new request has been received
                        NewRequestReceived?.Invoke(p);
                    }
                }
            }

            await Task.Delay((int)(delayBetweenRequests * 1000));
        }

        //Debug.Log("All requests completed.");
        OnAllResponsesRecieved();
    }

    void OnAllResponsesRecieved()
    {
        AllResponsesRecieved = true;
    }

    public void Restart()
    {
        Responses.Clear();
    }
}

[System.Serializable]
public class Part
{
    public string text;
}

[System.Serializable]
public class Content
{
    public string role;
    public Part[] parts;
}

[System.Serializable]
public class Candidate
{
    public Content content;
    public string finishReason;
    public int index;
    // You can add SafetyRatings and other fields if needed
}

[System.Serializable]
public class RequestBody
{
    public Content[] contents;
    public GenerationConfig generation_config;
    public SafetySettings[] safety_settings;
}

[System.Serializable]
public class Response
{
    public Candidate[] candidates;
    // You can add UsageMetadata and other fields if needed
}

[System.Serializable]
public class SafetySettings
{
    public string category;
    public string threshold;
}

[System.Serializable]
public class GenerationConfig
{
    public float temperature;
    public float top_p;
    public int top_k;
    public int max_output_tokens;
}

[System.Serializable]
public class ConversationEntry
{
    [field: SerializeField] public string Role { get; set; } // "user" or "model"
    [field: SerializeField] public string Text { get; set; }
}