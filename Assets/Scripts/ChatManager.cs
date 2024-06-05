using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;

public class ChatManager : MonoBehaviour
{
    public GameObject chatPanel;
    public ScrollRect chatScrollRect;
    public Transform chatContent;
    public GameObject MessagePrefab;
    public TMP_InputField chatInputField;
    public Button sendButton;

    private string OPENAI_API_KEY;

    private NPC curNPC;
    private NPCData curNPCData;

    public static ChatManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        OPENAI_API_KEY = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

        sendButton.onClick.AddListener(SendMessageToChatGPT);
    }

    public void OpenChat(NPC selectedNPC)
    {
        curNPC = selectedNPC;
        curNPCData = selectedNPC.GetNPCData();
        chatPanel.SetActive(true);
    }

    public void CloseChat()
    {
        curNPC = null;
        curNPCData = null;
        chatPanel.SetActive(false);
        ClearChat();
    }

    private void ClearChat()
    {
        foreach (Transform child in chatContent)
        {
            Destroy(child.gameObject);
        }
    }

    private void SendMessageToChatGPT()
    {
        string userMessage = chatInputField.text;
        if (string.IsNullOrEmpty(userMessage)) return;

        AddMessageToChat(userMessage, true);
        chatInputField.text = string.Empty;

        StartCoroutine(SendChatGPTRequest(userMessage));
    }

    private IEnumerator SendChatGPTRequest(string userMessage)
    {
        string apiUrl = "https://api.openai.com/v1/chat/completions";
        var request = new UnityWebRequest(apiUrl, "POST");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + OPENAI_API_KEY);

        var requestData = new
        {
            model = "gpt-3.5-turbo", // 사용할 모델 이름
            messages = new[]
            {
                new { role = "system", content = "You're fantasy RPG game NPC. Your name is " + curNPCData.Name +
                    ", your alignment is " + curNPCData.Alignment + ", your species is " + curNPCData.Species +
                    ", your background is " + curNPCData.Background + ". You must keep these setting and talk with user."},
                new { role = "user", content = userMessage }
            },
            max_tokens = 150,
            temperature = 0.7
        };

        string jsonData = JsonConvert.SerializeObject(requestData);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            var responseText = request.downloadHandler.text;
            Debug.Log("Response Text: " + responseText); // 응답 내용을 콘솔에 출력

            var responseJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseText);
            if (responseJson != null && responseJson.ContainsKey("choices"))
            {
                var choices = responseJson["choices"] as JArray;
                if (choices != null && choices.Count > 0)
                {
                    var choice = choices[0] as JObject;
                    if (choice != null && choice.ContainsKey("message"))
                    {
                        var message = choice["message"] as JObject;
                        if (message != null && message.ContainsKey("content"))
                        {
                            var text = message["content"].ToString().Trim();
                            AddMessageToChat(text, false);
                        }
                        else
                        {
                            Debug.LogError("Message content is null or missing");
                        }
                    }
                    else
                    {
                        Debug.LogError("Message key is missing in choice");
                    }
                }
                else
                {
                    Debug.LogError("Choices are null or empty");
                }
            }
            else
            {
                Debug.LogError("Choices key is missing in response JSON");
            }
        }
    }

    public void AddMessageToChat(string message, bool isUser)
    {
        GameObject newMessage = Instantiate(MessagePrefab, chatContent);
        TextMeshProUGUI messageText = newMessage.GetComponentInChildren<TextMeshProUGUI>();

        if (isUser)
        {
            messageText.text = "User : " + message;
        }
        else
        {
            messageText.text = curNPCData.Name + " : " + message;
        }
    }
}