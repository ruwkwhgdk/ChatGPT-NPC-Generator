using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace OpenAI
{
    public class ChatGPT : MonoBehaviour
    {
        [SerializeField] private NPCDataUI NPCUIInput;

        private static readonly HttpClient client = new HttpClient();
        private string apiKey;

        private float height;
        private OpenAIApi openai = new OpenAIApi();

        private List<ChatMessage> messages = new List<ChatMessage>();

        public JsonManager JsonManager;

        // NPC 생성 기본 메시지
        private string defaultNPCGenerateMessage = "Generate a JSON object for fantasy RPG Game NPC with fields for 'Name', 'Sprite', 'Alignment', ''Species', and 'Background'.";
        private string alignmentFormatMessage = "Alignment uses DND alignment system, and is printed by number : Lawful Good = 0, Neutral Good = 1, Chaotic Good = 2, Lawful Neutral = 3, True Neutral = 4, Chaotic Neutral = 5, Lawful Evil = 6, Neutral Evil = 7, Chaotic Evil = 8";
        private string speciesFormatMessage = "Species is printed by number : Human = 0, Halfling = 1, Dwarf = 2, Goblin = 3, Gnome = 4, Nymph = 5, Orc = 6, Elf = 7, Lizardman = 8, Kobold = 9, Angel = 10, Devil = 11, Dragon = 12, Android = 13, Ogre = 14, Ghoul = 15, Skeleton = 16, Vampire = 17, Etc = 18";
        private string backgroundFormatMessage = "Background is NPC's detail story, You write background by NPC's other feature.";

        private class GPTResponse
        {
            public Choice[] choices { get; set; }
        }

        private class Choice
        {
            public Message message { get; set; }
        }

        private class Message
        {
            public string content { get; set; }
        }

        private void Start()
        {
            apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            // Json Import
            JsonManager.ImportJson();
        }

        public async void ApplyChatGPTNPCData(
            string name, string spriteName, NPCFeature.NPCAlignment alignment, NPCFeature.NPCSpecies species, string background)
        {
            var systemMessage = new ChatMessage()
            {
                Role = "system",
                Content = defaultNPCGenerateMessage
            };

            var alignmentMessage = new ChatMessage()
            {
                Role = "system",
                Content = alignmentFormatMessage
            };

            var speciesMessage = new ChatMessage()
            {
                Role = "system",
                Content = speciesFormatMessage
            };

            var backgroundMessage = new ChatMessage()
            {
                Role = "system",
                Content = backgroundFormatMessage
            };

            var curText = "Name : " + name + " / SpriteName : " + spriteName + " / Alignment : " + alignment +
                " / Species : " + species + " / Background : " + background;

            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = curText
            };

            newMessage.Content = curText;
            
            messages.Add(systemMessage);
            messages.Add(alignmentMessage);
            messages.Add(speciesMessage);
            messages.Add(backgroundMessage);
            messages.Add(newMessage);
            
            // Complete the instruction
            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = "gpt-3.5-turbo-0613",
                Messages = messages
            });

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                var message = completionResponse.Choices[0].Message;
                message.Content = message.Content.Trim();
                messages.Add(message);

                Debug.Log(message.Content);

                // 출력한 메시지를 NPCData 형태로 저장
                var newData = NPCData.SetNPCData(message.Content);
                JsonManager.AddNPCData(newData);
            }
            else
            {
                Debug.LogWarning("No text was generated from this prompt.");
            }
        }

        public async Task<NPCBehaviourManager.NPCBehaviour> SetNPCBehaviour(NPC npc, DateTime gameTime)
        {
            string npcName = npc.GetNPCData().Name;
            int currentTime = gameTime.Hour;

            // NPC가 이동할 위치 결정
            Vector2Int targetLocation = DetermineTargetLocation(npc);

            // ChatGPT API 요청을 만드는 부분
            NPCData npcData = npc.GetNPCData();
            string promptSetting = "NPC Setting - Name : " + npcData.Name + ", Alignment : " + npcData.Alignment +
                ", Species : " + npcData.Species + ", Background : " + npcData.Background +
                "\nYou must create NPC behaviors that reflect these settings. " +
                "\nWhen creating behaviors for NPCs, you should consider the current time, race, and disposition." +
                "\nFor example, if it is nighttime, NPCs might sleep or be active depending on their species." + 
                "\nAdditionally, the background settings of NPCs also influence their behavior." +
                "\nCurrent time : " + currentTime;

            string promptBehaviour = $"Provide the behavior details for {npcName} at {gameTime:yyyy-MM-dd HH:mm:ss} in JSON format. " +
                "Include TravelTime, ActionTime, Action, IsInteractNPC, and Details. " +
                "TravelTime and ActionTime should be in the format 'N minutes'." + 
                "Action is a summary of the NPC's behavior in a single word." + 
                "Details should be a single string containing all relevant information." +
                "The NPC's behavior should be like that of actual inhabitants of a fantasy world, excluding game-related elements such as players or quests.";

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = "You are a simulation NPC behavior manager." },
                     new { role = "system", content = promptBehaviour },
                    new { role = "user", content = promptBehaviour }
                }
            };

            string jsonBody = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            Debug.Log("API Key: " + apiKey);

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            // ChatGPT API 호출
            var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
            string responseBody = await response.Content.ReadAsStringAsync();

            Debug.Log($"API Response: {responseBody}");

            try
            {
                response.EnsureSuccessStatusCode();

                var gptResponse = JsonConvert.DeserializeObject<GPTResponse>(responseBody);
                string messageContent = gptResponse.choices[0].message.content;

                // JSON 코드 블록 문법 제거
                messageContent = Regex.Replace(messageContent, "^```json\\s*", "").Trim();
                messageContent = Regex.Replace(messageContent, "```$", "").Trim();

                // 여기서 messageContent를 파싱하여 NPCBehaviour 구조체로 변환합니다.
                NPCBehaviourManager.NPCBehaviour npcBehaviour = ParseNPCBehaviour(npcName, currentTime, targetLocation, messageContent);
                npcBehaviour.DebugNPCBehaviour();

                LogManager.Instance.AddLog(npcBehaviour);

                return npcBehaviour;
            }
            catch (HttpRequestException httpEx)
            {
                Debug.LogError($"HTTP Request Exception: {httpEx.Message}");
            }
            catch (JsonException jsonEx)
            {
                Debug.LogError($"JSON Exception: {jsonEx.Message}");
                Debug.LogError($"Response Content: {responseBody}");
            }

            // 기본값 반환
            return default(NPCBehaviourManager.NPCBehaviour);
        }

        private Vector2Int DetermineTargetLocation(NPC npc)
        {
            List<Vector2Int> possibleLocations = new List<Vector2Int>();

            // 8x8 그리드 내의 가능한 위치를 모두 추가
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Vector2Int location = new Vector2Int(x, y);

                    // 다른 NPC가 있는 장소는 제외
                    bool locationOccupied = false;
                    foreach (var otherNpc in NPCManager.Instance.GetNPCList())
                    {
                        if (otherNpc.TargetPosition == location)
                        {
                            locationOccupied = true;
                            break;
                        }
                    }

                    if (!locationOccupied)
                    {
                        possibleLocations.Add(location);
                    }
                }
            }

            // NPC의 현재 위치에서 가까운 곳일수록 이동 확률이 높아지도록 가중치 부여
            Dictionary<Vector2Int, float> weightedLocations = new Dictionary<Vector2Int, float>();
            foreach (var location in possibleLocations)
            {
                float distance = Vector2Int.Distance(new Vector2Int((int)npc.Location.x, (int)npc.Location.y), location);
                float weight = 1 / (distance + 1);  // 거리가 가까울수록 가중치가 높아짐
                weightedLocations[location] = weight;
            }

            // 가중치에 따라 랜덤하게 목표 위치 선택
            float totalWeight = 0;
            foreach (var weight in weightedLocations.Values)
            {
                totalWeight += weight;
            }

            float randomValue = UnityEngine.Random.Range(0, totalWeight);
            float cumulativeWeight = 0;
            foreach (var location in weightedLocations)
            {
                cumulativeWeight += location.Value;
                if (randomValue <= cumulativeWeight)
                {
                    return location.Key;
                }
            }

            // 기본값 반환 (여기까지 도달하면 안됨)
            return new Vector2Int(0, 0);
        }

        private NPCBehaviourManager.NPCBehaviour ParseNPCBehaviour(string npcName, int currentTime, Vector2Int targetLocation, string messageContent)
        {
            try
            {
                var jsonResponse = JObject.Parse(messageContent);

                // 디버깅을 위해 파싱된 JSON을 로그로 출력
                Debug.Log($"Parsed JSON Response: {jsonResponse}");

                // JSON 응답에 Behavior 객체가 있는지 확인
                var behavior = jsonResponse["Behavior"] ?? jsonResponse;

                NPCBehaviourManager.NPCBehaviour behaviour = new NPCBehaviourManager.NPCBehaviour
                {
                    NPCName = npcName,
                    CurrentTime = currentTime,
                    Location = targetLocation,
                    TravelTime = behavior["TravelTime"] != null ? ParseTime((string)behavior["TravelTime"]) : 5,  // 기본값 5분
                    ActionTime = behavior["ActionTime"] != null ? ParseTime((string)behavior["ActionTime"]) : 5,  // 기본값 5분
                    Action = behavior["Action"] != null ? (string)behavior["Action"] : "None",
                    Details = behavior["Details"] != null ? (string)behavior["Details"] : string.Empty
                };

                // 디버깅을 위해 파싱된 NPCBehaviour 객체를 로그로 출력
                Debug.Log($"Parsed NPCBehaviour: {behaviour.NPCName}, {behaviour.CurrentTime}, {behaviour.Location}, {behaviour.TravelTime}, {behaviour.ActionTime}, {behaviour.Action}, {behaviour.Details}");

                return behaviour;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error parsing NPC behaviour: {ex.Message}");
                throw;
            }
        }

        private int ParseTime(string timeStr)
        {
            // "10 minutes"와 같은 문자열을 파싱하여 분 단위의 시간을 반환
            var match = Regex.Match(timeStr, @"(\d+)\s*minutes?");
            if (match.Success && int.TryParse(match.Groups[1].Value, out int minutes))
            {
                return minutes; // 분 단위로 반환
            }

            return 5;  // 기본값으로 5분
        }

        private void OnApplicationQuit()
        {
            JsonManager.ExportJson();
        }
    }
}
