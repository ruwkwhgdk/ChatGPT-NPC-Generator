using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class NPCDataUI : MonoBehaviour
{
    // UI ����Ʈ
    public TMP_InputField NameInputField;
    public TMP_InputField SpriteInputField;
    public TMP_Dropdown AlignmentDropdown;
    public TMP_Dropdown SpeciesDropdown;
    public TMP_InputField BackgroundInputField;
    public Button SubmitButton;
    public GameObject WaitPanel; // �۾� ������ �˷��ִ� UI Panel

    // ChatGPT
    public OpenAI.ChatGPT ChatGPT;

    // SpriteManager �߰�
    public SpriteManager SpriteManager;

    private void Start()
    {
        // Alignment Dropdown �ʱ�ȭ
        AlignmentDropdown.options.Clear();
        foreach (var alignment in System.Enum.GetNames(typeof(NPCFeature.NPCAlignment)))
        {
            AlignmentDropdown.options.Add(new TMP_Dropdown.OptionData() { text = alignment });
        }

        // Species Dropdown �ʱ�ȭ
        SpeciesDropdown.options.Clear();
        foreach (var species in System.Enum.GetNames(typeof(NPCFeature.NPCSpecies)))
        {
            SpeciesDropdown.options.Add(new TMP_Dropdown.OptionData() { text = species });
        }

        // ��ư Ŭ�� �̺�Ʈ �߰�
        SubmitButton.onClick.AddListener(OnSubmit);
    }

    private async void OnSubmit()
    {
        // WaitPanel Ȱ��ȭ
        WaitPanel.SetActive(true);

        // NPC ������ ChatGPT�� ����
        ChatGPT.ApplyChatGPTNPCData(NameInputField.text, SpriteInputField.text,
            (NPCFeature.NPCAlignment)AlignmentDropdown.value, (NPCFeature.NPCSpecies)SpeciesDropdown.value, BackgroundInputField.text);

        // NPC ���� ����
        string npcDescription = $"Name: {NameInputField.text}, Alignment: {AlignmentDropdown.options[AlignmentDropdown.value].text}, Species: {SpeciesDropdown.options[SpeciesDropdown.value].text}, Background: {BackgroundInputField.text}";
        string additionalPrompt = " / Generation criteria: Generate a dot pixel graphic of an NPC in the style of Final Fantasy series, should follow the same color palette and pixel art style as the example : https://imgur.com/a/XGUpUWr";

        // SpriteManager�� ���� NPC ��������Ʈ ����
        await SpriteManager.GenerateNPCSprite(
            npcDescription + additionalPrompt, "Assets/Resources/" + SpriteInputField.text + ".png");

        // WaitPanel ��Ȱ��ȭ
        WaitPanel.SetActive(false);
    }
}
