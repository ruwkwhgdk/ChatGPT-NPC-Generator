using System;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Threading.Tasks; 
using TMPro;

public class TimeManager : MonoBehaviour
{
    public GameObject timeUICanvas; // Time UI
    public TMP_Text timeDisplay;  // ȭ�� ��ܿ� �ð��� ǥ���� UI �ؽ�Ʈ ���

    private bool isActive = false; // TimeManager �۵� �����ߴ��� ����
    private bool isPause = false;

    private float elapsedTime = 0f;  // ����� ���� �ð�
    private DateTime gameTime;  // ���� �ð�
    private const float realTimeToGameTimeFactor = 12f;  // ���� �ð� 1�ʸ� ���� �ð� 5������ ��ȯ

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

        gameTime = new DateTime(1, 1, 1, 0, 0, 0); // ���� �ð� �ʱ�ȭ
        UpdateTimeDisplay();

        // ���� ���� �� �� �� NPC Manager���� �˸�
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
        await NPCManager.Instance.OnGameHourPassed(gameTime);  // NPCManager�� �˸�
        isActive = true;
    }
}
