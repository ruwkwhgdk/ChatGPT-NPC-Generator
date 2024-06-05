using System;
using System.IO;
using UnityEngine;

public class EnvLoader : MonoBehaviour
{
    void Awake()
    {
        LoadEnv();
    }

    void LoadEnv()
    {
        // Unity 프로젝트 루트 디렉토리에서 .env 파일을 읽어옵니다.
        string envFilePath = Path.Combine(Application.dataPath, "../.env");
        if (!File.Exists(envFilePath))
        {
            Debug.LogError(".env file not found at: " + envFilePath);
            return;
        }

        foreach (var line in File.ReadAllLines(envFilePath))
        {
            var parts = line.Split('=', 2);
            if (parts.Length == 2)
            {
                Environment.SetEnvironmentVariable(parts[0], parts[1]);
            }
        }
    }
}
