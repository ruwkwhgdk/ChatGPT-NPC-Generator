using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using System.IO;

public class SpriteManager : MonoBehaviour
{
    private string OPENAI_API_KEY = System.Environment.GetEnvironmentVariable("OPENAI_API_KEY");

    public static SpriteManager Instance;

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

    public async Task<Texture2D> GenerateNPCSprite(string npcDescription, string path)
    {
        string imageUrl = await GetGeneratedImageUrl(npcDescription);
        if (string.IsNullOrEmpty(imageUrl))
        {
            Debug.LogError("Failed to generate image URL");
            return null;
        }
        
        Texture2D texture = await DownloadImage(imageUrl);
        if (texture == null)
        {
            Debug.LogError("Failed to download image");
            return null;
        }
        
        Texture2D transparentTexture = MakeTextureTransparent(texture); // 투명하게 만드는 메서드 호출
        SaveTextureAsPNG(transparentTexture, path);

        return transparentTexture;
    }

    private async Task<string> GetGeneratedImageUrl(string prompt)
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {OPENAI_API_KEY}");

            var requestData = new
            {
                prompt = prompt,
                n = 1,
                size = "256x256"
            };

            var json = JsonConvert.SerializeObject(requestData);
            Debug.Log($"Request JSON: {json}"); // 요청 데이터를 로그로 출력

            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/images/generations", content);
        
            if (!response.IsSuccessStatusCode)
            {
                Debug.LogError($"Error fetching image: {response.ReasonPhrase}");
                string errorResponseBody = await response.Content.ReadAsStringAsync(); // 변수 이름 변경
                Debug.LogError($"Response Body: {errorResponseBody}"); // 응답 본문을 로그로 출력
                return null;
            }

            string responseBody = await response.Content.ReadAsStringAsync();
            JObject jsonResponse = JObject.Parse(responseBody);
            return jsonResponse["data"]?[0]?["url"]?.ToString();
        }
    }

    private async Task<Texture2D> DownloadImage(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl))
        {
            Debug.LogError("Image URL is null or empty");
            return null;
        }
        
        using (HttpClient client = new HttpClient())
        {
            byte[] imageBytes = await client.GetByteArrayAsync(imageUrl);
            if (imageBytes == null || imageBytes.Length == 0)
            {
                Debug.LogError("Failed to download image bytes");
                return null;
            }

            Texture2D texture = new Texture2D(64, 64);
            texture.LoadImage(imageBytes);
            return texture;
        }
    }

    private void SaveTextureAsPNG(Texture2D texture, string filePath)
    {
        if (texture == null)
        {
            Debug.LogError("Texture is null");
            return;
        }

        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);
        Debug.Log($"Saved NPC sprite to {filePath}");
    }

    private Texture2D MakeTextureTransparent(Texture2D texture)
    {
        Color[] pixels = texture.GetPixels();

        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].r > 0.9f && pixels[i].g > 0.9f && pixels[i].b > 0.9f) // 흰색으로 판단되는 픽셀
            {
                pixels[i] = new Color(pixels[i].r, pixels[i].g, pixels[i].b, 0); // 알파 채널을 0으로 설정하여 투명하게 만듦
            }
        }

        Texture2D transparentTexture = new Texture2D(texture.width, texture.height);
        transparentTexture.SetPixels(pixels);
        transparentTexture.Apply();

        return transparentTexture;
    }
}
