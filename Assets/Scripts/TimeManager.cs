using System;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Threading.Tasks; 
using TMPro;

public class TimeManager : MonoBehaviour
{
    public GameObject timeUICanvas; // Time UI
    public TMP_Text timeDisplay;  // 화면 상단에 시간을 표시할 UI 텍스트 요소

    private bool isActive = false; // TimeManager 작동 시작했는지 저장
    private bool isPause = false;

    private float elapsedTime = 0f;  // 경과된 현실 시간
    private DateTime gameTime;  // 게임 시간
    private const float realTimeToGameTimeFactor = 12f;  // 현실 시간 1초를 게임 시간 5분으로 변환

    public static TimeManager Instance;

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

    public void Activate()
    {
        isActive = true;
        timeUICanvas.SetActive(true);

        gameTime = new DateTime(1, 1, 1, 0, 0, 0); // 게임 시간 초기화
        UpdateTimeDisplay();

        // 최초 실행 시 한 번 NPC Manager에게 알림
        NotifyNPCManager();
    }

    public void Pause()
    {
        isPause = true;
    }

    public void Resume()
    {
        isPause = false;
    }

    public bool GetIsActive()
    {
        return isActive && !isPause;
    }

    void Update()
    {
        if (!isActive || isPause)
        {
            return;
        }

        elapsedTime += Time.deltaTime;
        int elapsedMinutes = Mathf.FloorToInt(elapsedTime * realTimeToGameTimeFactor);
        if (elapsedMinutes > 0)
        {
            gameTime = gameTime.AddMinutes(elapsedMinutes);
            elapsedTime = 0f;
            UpdateTimeDisplay();
            NotifyNPCManager();
        }
    }

    void UpdateTimeDisplay()
    {
        timeDisplay.text = gameTime.ToString("dd HH:mm");
    }

    private async Task NotifyNPCManager()
    {
        isActive = false;
        await NPCManager.Instance.OnGameHourPassed(gameTime);  // NPCManager에 알림
        isActive = true;
    }
}
