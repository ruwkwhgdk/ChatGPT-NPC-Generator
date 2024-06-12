using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks; 
using TMPro;

public class NPCManager : MonoBehaviour
{
    public OpenAI.ChatGPT chatGPT;

    public GameObject npcPrefab; // NPC 프리팹을 할당합니다.
    private const int gridX = 8;
    private const int gridY = 8;

    public GameObject npcMenuPanel; // NPC 메뉴 패널
    public Button viewDataButton; // NPC Data 보기 버튼
    public Button talkButton; // NPC와 대화 버튼
    public Button viewStatusButton; // NPC 상태 보기 버튼
    public Button menuBackButton; // NPC 메뉴 뒤로 가기 버튼

    public GameObject npcDataPanel; // NPC 데이터를 출력하는 UI Panel
    public TMP_Text nameText; // NPC 이름 텍스트
    public TMP_Text speciesText; // NPC 종족 텍스트
    public TMP_Text alignmentText; // NPC 성향 텍스트
    public TMP_Text backgroundText; // NPC 배경 텍스트
    public Image npcSpriteImage; // NPC 스프라이트를 표시하는 이미지 컴포넌트
    public Button dataBackButton; // 데이터 패널 뒤로 가기 버튼

    public bool IsOpenNPCUI { get; private set; } = false;

    private NPC selectedNPC; // 선택된 NPC

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
        npcMenuPanel.SetActive(false); // 초기에는 메뉴 패널을 숨깁니다.
        npcDataPanel.SetActive(false); // 초기에는 데이터 패널을 숨깁니다.
        viewDataButton.onClick.AddListener(ViewNPCData);
        menuBackButton.onClick.AddListener(HideNPCMenu);
        dataBackButton.onClick.AddListener(HideNPCData);

        // NPC List 초기화
        npcList = new List<NPC>();
    }

    public void Simulate()
    {
        // GridWorld 생성
        GridWorldManager.Instance.InitGridWorld();

        // 기존에 생성된 NPC들을 제거
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // 랜덤 위치에 NPC 생성
        npcDataList = JsonManager.Instance.GetNPCDataList();

        for (int i = 0; i < npcDataList.List.Count; i++)
        {
            // 랜덤한 위치에 NPC 생성
            Vector3 randomPosition = new Vector3(UnityEngine.Random.Range(0, gridX), UnityEngine.Random.Range(0, gridY), 0);
            GameObject npc = Instantiate(npcPrefab, randomPosition + GridWorldManager.Instance.AddPos, Quaternion.identity, transform);
            
            // NPC 초기화
            var curNPC = npc.GetComponent<NPC>();
            curNPC.SetNPC(npcDataList.List[i], new Vector2(randomPosition.x, randomPosition.y));
            npcList.Add(curNPC);

            // 디버그
            //Debug.Log(npcDataList.List[i].Name);
            //Debug.Log(randomPosition);
        }

        // TimeManager 활성화
        TimeManager.Instance.Activate();

        // Log Open UI 활성화
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
            npcMenuPanel.SetActive(true); // 메뉴 패널을 표시합니다.

            IsOpenNPCUI = true;

            // NPC 스프라이트를 UI 이미지 컴포넌트에 설정
            npcSpriteImage.sprite = selectedNPC.GetComponent<SpriteRenderer>().sprite;
        }
    }

    private void HideNPCMenu()
    {
        TimeManager.Instance.Resume();

        IsOpenNPCUI = false;

        npcMenuPanel.SetActive(false); // 메뉴 패널을 숨깁니다.
    }

    private void ViewNPCData()
    {
        NPCData npcData = selectedNPC.GetNPCData();
        nameText.text = $"{npcData.Name}";
        speciesText.text = $"{npcData.Species}";
        alignmentText.text = $"{npcData.Alignment}";
        backgroundText.text = $"{npcData.Background}";
        npcDataPanel.SetActive(true); // 데이터 패널을 표시합니다.
        npcMenuPanel.SetActive(false); // 메뉴 패널을 숨깁니다.
    }

    private void HideNPCData()
    {
        npcDataPanel.SetActive(false); // 데이터 패널을 숨깁니다.
        npcMenuPanel.SetActive(true); // 메뉴 패널을 표시합니다.
    }

    public List<NPC> GetNPCList()
    {
        return npcList;
    }

    public async Task OnGameHourPassed(DateTime gameTime)
    {
        // NPC 행동 업데이트 로직 추가
        foreach (var npc in npcList)
        {
            if (!npc.IsActing)
            {
                NPCBehaviourManager.NPCBehaviour behaviour = await chatGPT.SetNPCBehaviour(npc, gameTime);
                npc.SetBehaviour(behaviour);

                // NPC 행동 업데이트 로직 추가
                Debug.Log($"{behaviour.NPCName}가 {behaviour.Action} 중입니다.");
            }
        }
    }
}