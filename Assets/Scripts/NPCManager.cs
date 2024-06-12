using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks; 
using TMPro;

public class NPCManager : MonoBehaviour
{
    public OpenAI.ChatGPT chatGPT;

    public GameObject npcPrefab; // NPC �������� �Ҵ��մϴ�.
    private const int gridX = 8;
    private const int gridY = 8;

    public GameObject npcMenuPanel; // NPC �޴� �г�
    public Button viewDataButton; // NPC Data ���� ��ư
    public Button talkButton; // NPC�� ��ȭ ��ư
    public Button viewStatusButton; // NPC ���� ���� ��ư
    public Button menuBackButton; // NPC �޴� �ڷ� ���� ��ư

    public GameObject npcDataPanel; // NPC �����͸� ����ϴ� UI Panel
    public TMP_Text nameText; // NPC �̸� �ؽ�Ʈ
    public TMP_Text speciesText; // NPC ���� �ؽ�Ʈ
    public TMP_Text alignmentText; // NPC ���� �ؽ�Ʈ
    public TMP_Text backgroundText; // NPC ��� �ؽ�Ʈ
    public Image npcSpriteImage; // NPC ��������Ʈ�� ǥ���ϴ� �̹��� ������Ʈ
    public Button dataBackButton; // ������ �г� �ڷ� ���� ��ư

    public bool IsOpenNPCUI { get; private set; } = false;

    private NPC selectedNPC; // ���õ� NPC

    public static NPCManager Instance;

    private NPCDataList npcDataList;
    private List<NPC> npcList;

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
        npcMenuPanel.SetActive(false); // �ʱ⿡�� �޴� �г��� ����ϴ�.
        npcDataPanel.SetActive(false); // �ʱ⿡�� ������ �г��� ����ϴ�.
        viewDataButton.onClick.AddListener(ViewNPCData);
        menuBackButton.onClick.AddListener(HideNPCMenu);
        dataBackButton.onClick.AddListener(HideNPCData);

        // NPC List �ʱ�ȭ
        npcList = new List<NPC>();
    }

    public void Simulate()
    {
        // GridWorld ����
        GridWorldManager.Instance.InitGridWorld();

        // ������ ������ NPC���� ����
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // ���� ��ġ�� NPC ����
        npcDataList = JsonManager.Instance.GetNPCDataList();

        for (int i = 0; i < npcDataList.List.Count; i++)
        {
            // ������ ��ġ�� NPC ����
            Vector3 randomPosition = new Vector3(UnityEngine.Random.Range(0, gridX), UnityEngine.Random.Range(0, gridY), 0);
            GameObject npc = Instantiate(npcPrefab, randomPosition + GridWorldManager.Instance.AddPos, Quaternion.identity, transform);
            
            // NPC �ʱ�ȭ
            var curNPC = npc.GetComponent<NPC>();
            curNPC.SetNPC(npcDataList.List[i], new Vector2(randomPosition.x, randomPosition.y));
            npcList.Add(curNPC);

            // �����
            //Debug.Log(npcDataList.List[i].Name);
            //Debug.Log(randomPosition);
        }

        // TimeManager Ȱ��ȭ
        TimeManager.Instance.Activate();

        // Log Open UI Ȱ��ȭ
        LogManager.Instance.SetActiveOpenUI();
    }

    public void StartChat()
    {
        ChatManager chatManager = ChatManager.Instance;
        if (chatManager != null)
        {
            chatManager.OpenChat(selectedNPC);
        }
    }

    public void ShowNPCMenu(NPC npc)
    {
        if (!TimeManager.Instance.GetIsPause())
        {
            TimeManager.Instance.Pause();

            selectedNPC = npc;
            npcMenuPanel.SetActive(true); // �޴� �г��� ǥ���մϴ�.

            IsOpenNPCUI = true;

            // NPC ��������Ʈ�� UI �̹��� ������Ʈ�� ����
            npcSpriteImage.sprite = selectedNPC.GetComponent<SpriteRenderer>().sprite;
        }
    }

    private void HideNPCMenu()
    {
        TimeManager.Instance.Resume();

        IsOpenNPCUI = false;

        npcMenuPanel.SetActive(false); // �޴� �г��� ����ϴ�.
    }

    private void ViewNPCData()
    {
        NPCData npcData = selectedNPC.GetNPCData();
        nameText.text = $"{npcData.Name}";
        speciesText.text = $"{npcData.Species}";
        alignmentText.text = $"{npcData.Alignment}";
        backgroundText.text = $"{npcData.Background}";
        npcDataPanel.SetActive(true); // ������ �г��� ǥ���մϴ�.
        npcMenuPanel.SetActive(false); // �޴� �г��� ����ϴ�.
    }

    private void HideNPCData()
    {
        npcDataPanel.SetActive(false); // ������ �г��� ����ϴ�.
        npcMenuPanel.SetActive(true); // �޴� �г��� ǥ���մϴ�.
    }

    public List<NPC> GetNPCList()
    {
        return npcList;
    }

    public async Task OnGameHourPassed(DateTime gameTime)
    {
        // NPC �ൿ ������Ʈ ���� �߰�
        foreach (var npc in npcList)
        {
            if (!npc.IsActing)
            {
                NPCBehaviourManager.NPCBehaviour behaviour = await chatGPT.SetNPCBehaviour(npc, gameTime);
                npc.SetBehaviour(behaviour);

                // NPC �ൿ ������Ʈ ���� �߰�
                Debug.Log($"{behaviour.NPCName}�� {behaviour.Action} ���Դϴ�.");
            }
        }
    }
}