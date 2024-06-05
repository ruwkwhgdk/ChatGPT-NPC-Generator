using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LogManager : MonoBehaviour
{
    public Button LogOpenButton;
    public GameObject LogPanel;
    public Transform LogPanelContent;
    public Button BackButton;

    public GameObject LogPrefab;

    private List<GameObject> logObjectList;
    private List<NPCBehaviourManager.NPCBehaviour> npcBehaviourList;

    public static LogManager Instance;

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
        logObjectList = new List<GameObject>();
        npcBehaviourList = new List<NPCBehaviourManager.NPCBehaviour>();

        LogOpenButton.onClick.AddListener(ShowPanel);
        BackButton.onClick.AddListener(HidePanel);
    }

    public void SetActiveOpenUI()
    {
        LogOpenButton.transform.parent.gameObject.SetActive(true);
    }

    public void AddLog(NPCBehaviourManager.NPCBehaviour npcBehaviour)
    {
        npcBehaviourList.Add(npcBehaviour);
    }

    private void ShowPanel()
    {
        TimeManager.Instance.Pause();

        LogPanel.SetActive(true);
        LogOpenButton.gameObject.SetActive(false);

        for (int i = 0; i < npcBehaviourList.Count; i++)
        {
            var logObj = Instantiate(LogPrefab, LogPanelContent);
            var textUI = logObj.GetComponentInChildren<TextMeshProUGUI>();
            textUI.text = npcBehaviourList[i].GetString();

            logObjectList.Add(logObj);
        }
    }

    private void HidePanel()
    {
        TimeManager.Instance.Resume();

        LogPanel.SetActive(false);
        LogOpenButton.gameObject.SetActive(true);

        for (int i = 0; i < logObjectList.Count; i++)
        {
            Destroy(logObjectList[i]);
        }

        logObjectList.Clear();
    }
}
