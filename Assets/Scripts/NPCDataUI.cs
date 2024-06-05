using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class NPCDataUI : MonoBehaviour
{
    // UI 리스트
    public TMP_InputField NameInputField;
    public TMP_InputField SpriteInputField;
    public TMP_Dropdown AlignmentDropdown;
    public TMP_Dropdown SpeciesDropdown;
    public TMP_InputField BackgroundInputField;
    public Button SubmitButton;
    public GameObject WaitPanel; // 작업 중임을 알려주는 UI Panel

    // ChatGPT
    public OpenAI.ChatGPT ChatGPT;

    // SpriteManager 추가
    public SpriteManager SpriteManager;

    private void Start()
    {
        // Alignment Dropdown 초기화
        AlignmentDropdown.options.Clear();
        foreach (var alignment in System.Enum.GetNames(typeof(NPCFeature.NPCAlignment)))
        {
            AlignmentDropdown.options.Add(new TMP_Dropdown.OptionData() { text = alignment });
        }

        // Species Dropdown 초기화
        SpeciesDropdown.options.Clear();
        foreach (var species in System.Enum.GetNames(typeof(NPCFeature.NPCSpecies)))
        {
            SpeciesDropdown.options.Add(new TMP_Dropdown.OptionData() { text = species });
        }

        // 버튼 클릭 이벤트 추가
        SubmitButton.onClick.AddListener(OnSubmit);
    }

    private async void OnSubmit()
    {
        // WaitPanel 활성화
        WaitPanel.SetActive(true);

        // NPC 데이터 ChatGPT에 적용
        ChatGPT.ApplyChatGPTNPCData(NameInputField.text, SpriteInputField.text,
            (NPCFeature.NPCAlignment)AlignmentDropdown.value, (NPCFeature.NPCSpecies)SpeciesDropdown.value, BackgroundInputField.text);

        // NPC 설명 생성
        string npcDescription = $"Name: {NameInputField.text}, Alignment: {AlignmentDropdown.options[AlignmentDropdown.value].text}, Species: {SpeciesDropdown.options[SpeciesDropdown.value].text}, Background: {BackgroundInputField.text}";
        string additionalPrompt = " / Generation criteria: Generate a dot pixel graphic of an NPC in the style of Final Fantasy series, should follow the same color palette and pixel art style as the example : https://imgur.com/a/XGUpUWr";

        // SpriteManager를 통해 NPC 스프라이트 생성
        await SpriteManager.GenerateNPCSprite(
            npcDescription + additionalPrompt, "Assets/Resources/" + SpriteInputField.text + ".png");

        // WaitPanel 비활성화
        WaitPanel.SetActive(false);
    }
}
