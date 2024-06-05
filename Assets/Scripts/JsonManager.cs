using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using NPCFeature;

// 관리할 NPCData 리스트
public class NPCDataList
{
    public List<NPCData> List;
}

public class JsonManager : MonoBehaviour
{
    public NPCDataList DataList;

    // 저장 경로
    private string jsonPath = "Assets/Data";

    // NPC 데이터 파일명
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

    // Json 파일 받아서 데이터로 저장
    public void ImportJson()
    {
        string path = Path.Combine(jsonPath, jsonName);
        string allText = File.ReadAllText(path);
       
        DataList = JsonUtility.FromJson<NPCDataList>(allText);
    }

    // 데이터를 Json 파일 형태로 추출
    public void ExportJson()
    {
        var jsonData = JsonUtility.ToJson(DataList, true);

        string path = Path.Combine(jsonPath, jsonName);
        File.WriteAllText(path, jsonData);
    }

    // 데이터 리스트에 주어진 NPCData를 저장
    public void AddNPCData(NPCData data)
    {
        DataList.List.Add(data);
    }

    // NPC 데이터 리턴
    public NPCDataList GetNPCDataList()
    {
        return DataList;
    }
}