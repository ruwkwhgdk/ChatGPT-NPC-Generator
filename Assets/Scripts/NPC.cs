using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NPC : MonoBehaviour
{
    public Vector2 Location;
    public Vector2 TargetPosition;

    public bool IsActing{ get; private set; } = false;

    private const float realTimeToGameTimeFactor = 12f;  // ���� �ð� 1�ʸ� ���� �ð� 5������ ��ȯ

    private NPCData npcData;
    private Button npcButton;
    private SpriteRenderer spriteRenderer;

    private NPCBehaviourManager.NPCBehaviour currentBehaviour;

    private float travelTimeRemaining;
    private float actionTimeRemaining;
    private float travelSpeed;

    public void SetNPC(NPCData data, Vector2 startPos)
    {
        // NPCData ������Ʈ �߰�
        npcData = data;
        Location = startPos;
        TargetPosition = Location;

        // NPCData���� SpriteName �޾ƿͼ� Sprite ����
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (npcData != null && spriteRenderer != null)
        {
            // SpriteName�� ����Ͽ� SpriteRenderer�� ��������Ʈ�� ����
            Sprite npcSprite = Resources.Load<Sprite>(npcData.Sprite);

            if (npcSprite != null)
            {
                spriteRenderer.sprite = npcSprite;
            }
            else
            {
                Debug.LogError($"Sprite '{npcData.Sprite}' not found in Resources folder.");
            }
        }
        else
        {
            Debug.LogError("NPCData or SpriteRenderer component is missing.");
        }
    }

    public NPCData GetNPCData()
    {
        return npcData;
    }

    private void OnMouseDown()
    {
        if (!NPCManager.Instance.IsOpenNPCUI)
        {
            ShowNPCData();
        }
    }

    private void ShowNPCData()
    {
        NPCManager.Instance.ShowNPCMenu(this);
    }

    private void Update()
    {
        // NPC ������Ʈ �ڵ�
        if (!IsActing || !TimeManager.Instance.GetIsActive())
        {
            return;
        }

        if (travelTimeRemaining > 0)
        {
            MoveTowardsTarget();
        }
        else
        {
            PerformBehaviour();
        }
    }

    public void SetBehaviour(NPCBehaviourManager.NPCBehaviour behaviour)
    {
        currentBehaviour = behaviour;
        TargetPosition = new Vector2((int)behaviour.Location.x, (int)behaviour.Location.y);
        travelTimeRemaining = behaviour.TravelTime;
        actionTimeRemaining = behaviour.ActionTime;
        IsActing = true;

         // �̵� �Ÿ� ��� (x��� y�� �Ÿ� ��)
        float totalDistance = Mathf.Abs(TargetPosition.x - Location.x) + Mathf.Abs(TargetPosition.y - Location.y);
        // �ӵ� = �̵� �Ÿ� / ��ü �̵� �ð�
        travelSpeed = totalDistance / behaviour.TravelTime;
    }

    private void PerformBehaviour()
    {
        if (actionTimeRemaining > 0)
        {
            actionTimeRemaining -= (Time.deltaTime * realTimeToGameTimeFactor);
            if (actionTimeRemaining <= 0)
            {
                transform.position = new Vector3(TargetPosition.x, TargetPosition.y, 0) + GridWorldManager.Instance.AddPos;

                actionTimeRemaining = 0;
                IsActing = false;
            }
        }
    }

    private void MoveTowardsTarget()
    {
        if (travelTimeRemaining <= 0)
        {
            // ���� �ð��� 0 ������ ���� ��ġ ����
            transform.position = new Vector3(TargetPosition.x, TargetPosition.y, 0) + GridWorldManager.Instance.AddPos;
            Location = TargetPosition;
            IsActing = false; // �ൿ �Ϸ�
            return;
        }

        // ���� �ð� ���� �̵��� �Ÿ� ���
        float step = travelSpeed * Time.deltaTime * realTimeToGameTimeFactor;

        // travelTimeRemaining ����
        travelTimeRemaining -= Time.deltaTime * realTimeToGameTimeFactor;

        // x�� �Ǵ� y�� �������θ� �̵�
        if (Mathf.Abs(TargetPosition.x - Location.x) > Mathf.Epsilon)
        {
            float move = Mathf.Sign(TargetPosition.x - Location.x) * step;
            if (Mathf.Abs(move) > Mathf.Abs(TargetPosition.x - Location.x))
            {
                Location.x = TargetPosition.x;
            }
            else
            {
                Location.x += move;
            }
        }
        else if (Mathf.Abs(TargetPosition.y - Location.y) > Mathf.Epsilon)
        {
            float move = Mathf.Sign(TargetPosition.y - Location.y) * step;
            if (Mathf.Abs(move) > Mathf.Abs(TargetPosition.y - Location.y))
            {
                Location.y = TargetPosition.y;
            }
            else
            {
                Location.y += move;
            }
        }

        // ���� ��ġ �ݿ�
        transform.position = new Vector3(Location.x, Location.y, 0) + GridWorldManager.Instance.AddPos;
    }
}