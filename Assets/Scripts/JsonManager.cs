using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using NPCFeature;

// ������ NPCData ����Ʈ
public class NPCDataList
{
    public List<NPCData> List;
}

public class JsonManager : MonoBehaviour
{
    public NPCDataList DataList;

    // ���� ���
    private string jsonPath = "Assets/Data";

    // NPC ������ ���ϸ�
    private string jsonName = "NPCData.json";

    public static JsonManager Instance;

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

    // Json ���� �޾Ƽ� �����ͷ� ����
    public void ImportJson()
    {
        string path = Path.Combine(jsonPath, jsonName);
        string allText = File.ReadAllText(path);
       
        DataList = JsonUtility.FromJson<NPCDataList>(allText);
    }

    // �����͸� Json ���� ���·� ����
    public void ExportJson()
    {
        var jsonData = JsonUtility.ToJson(DataList, true);

        string path = Path.Combine(jsonPath, jsonName);
        File.WriteAllText(path, jsonData);
    }

    // ������ ����Ʈ�� �־��� NPCData�� ����
    public void AddNPCData(NPCData data)
    {
        DataList.List.Add(data);
    }

    // NPC ������ ����
    public NPCDataList GetNPCDataList()
    {
        return DataList;
    }
}