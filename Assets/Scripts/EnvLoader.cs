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
        // Unity ������Ʈ ��Ʈ ���丮���� .env ������ �о�ɴϴ�.
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
