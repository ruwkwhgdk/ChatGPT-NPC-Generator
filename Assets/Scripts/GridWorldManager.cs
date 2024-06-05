using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridWorldManager : MonoBehaviour
{
    public GameObject gridPrefab;

    private const int gridX = 8;
    private const int gridY = 8;

    private List<GameObject> gridList;

    public Vector3 AddPos;

    public static GridWorldManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            AddPos = new Vector3(-gridX / 2 + 0.5f, -gridY / 2 + 1.5f, 0);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitGridWorld()
    {
        gridList = new List<GameObject>();

        var addPos = new Vector3(-gridX / 2 + 0.5f, -gridY / 2 + 1.5f, 0);

        for (int i = 0; i < gridX; i++)
        {
            for (int j = 0; j < gridY; j++)
            {
                GameObject gridObject = Instantiate(gridPrefab, new Vector3(i, j, 0) + addPos, Quaternion.identity, transform);
                gridList.Add(gridObject);
            }
        }
    }
}
