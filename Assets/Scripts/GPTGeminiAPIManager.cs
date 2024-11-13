using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace GGPTGeminiTest
{
    public class GPTGeminiAPIManager : MonoBehaviour
    {
        private string openAIKey = "YOUR_OPENAI_API_KEY";  // Replace with your actual OpenAI API key
        private string geminiKey = "AIzaSyB2dGpRAtu2j7ugKYBjBth7LX1cUPXMZrU";  // Replace with your actual Gemini API key
        //private string aiBackend = "GEMINI";
        private string model = "gemini-1.5-flash";  // Or "gpt-3.5-turbo"

        public List<string> convoHistory = new List<string>();

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Detect(convoHistory));
        }

        private IEnumerator Detect(List<string> chat)
        {
            string userContent = string.Join("", chat);
            Debug.Log(userContent);

            if (model.StartsWith("gpt"))
            {
                var messages = new List<Message>
            {
                new Message { role = "system", content = "System prompt here" },
                new Message { role = "user", content = userContent }
            };

                var requestBody = new RequestBody
                {
                    model = model,
                    messages = messages,
                    temperature = 0
                };

                string jsonBody = JsonUtility.ToJson(requestBody);//JsonConvert.SerializeObject(requestBody);
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);

                UnityWebRequest request = new UnityWebRequest("https://api.openai.com/v1/chat/completions", "POST");
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", "Bearer " + openAIKey);

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("Error: " + request.error);
                }
                else
                {
                    string responseText = request.downloadHandler.text;
                    Debug.Log("Response: " + responseText);
                    // Process the response as needed
                }
            }
            else if (model.StartsWith("gemini"))
            {
                var generationConfig = new GenerativeModelConfig
                {
                    temperature = 0f,
                    top_p = 0.95f,
                    top_k = 64,
                    max_output_tokens = 8192,
                    response_mime_type = "application/json"
                };

                var modelRequest = new GenerativeModelRequest
                {
                    model_name = model,
                    generation_config = generationConfig
                };

                var chatSession = new ChatSessionRequest
                {
                    model_name = model,
                    chat_session = new ChatSession { history = new List<string>() },
                    prompt = userContent
                };

                string jsonBody = JsonUtility.ToJson(chatSession);
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);

                UnityWebRequest request = new UnityWebRequest("https://generativelanguage.googleapis.com/v1/models/gemini-pro:generateContent?key=" + geminiKey, "POST");
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("Error: " + request.error);
                }
                else
                {
                    string responseText = request.downloadHandler.text;
                    Debug.Log("Response: " + responseText);
                    // Process the response as needed
                }
            }
        }
    }


    [System.Serializable]
    public class Message
    {
        public string role;
        public string content;
    }

    [System.Serializable]
    public class RequestBody
    {
        public string model;
        public List<Message> messages;
        public float temperature;
    }

    [System.Serializable]
    public class GenerativeModelConfig
    {
        public float temperature;
        public float top_p;
        public int top_k;
        public int max_output_tokens;
        public string response_mime_type;
    }

    [System.Serializable]
    public class GenerativeModelRequest
    {
        public string model_name;
        public GenerativeModelConfig generation_config;
    }

    [System.Serializable]
    public class ChatSession
    {
        public List<string> history;
    }

    [System.Serializable]
    public class ChatSessionRequest
    {
        public string model_name;
        public ChatSession chat_session;
        public string prompt;
    }
}