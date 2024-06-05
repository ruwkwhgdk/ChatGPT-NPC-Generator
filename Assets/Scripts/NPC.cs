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

    private const float realTimeToGameTimeFactor = 12f;  // 현실 시간 1초를 게임 시간 5분으로 변환

    private NPCData npcData;
    private Button npcButton;
    private SpriteRenderer spriteRenderer;

    private NPCBehaviourManager.NPCBehaviour currentBehaviour;

    private float travelTimeRemaining;
    private float actionTimeRemaining;
    private float travelSpeed;

    public void SetNPC(NPCData data, Vector2 startPos)
    {
        // NPCData 컴포넌트 추가
        npcData = data;
        Location = startPos;
        TargetPosition = Location;

        // NPCData에서 SpriteName 받아와서 Sprite 변경
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (npcData != null && spriteRenderer != null)
        {
            // SpriteName을 사용하여 SpriteRenderer의 스프라이트를 변경
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
        // NPC 업데이트 코드
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

         // 이동 거리 계산 (x축과 y축 거리 합)
        float totalDistance = Mathf.Abs(TargetPosition.x - Location.x) + Mathf.Abs(TargetPosition.y - Location.y);
        // 속도 = 이동 거리 / 전체 이동 시간
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
            // 남은 시간이 0 이하일 때의 위치 설정
            transform.position = new Vector3(TargetPosition.x, TargetPosition.y, 0) + GridWorldManager.Instance.AddPos;
            Location = TargetPosition;
            IsActing = false; // 행동 완료
            return;
        }

        // 남은 시간 동안 이동할 거리 계산
        float step = travelSpeed * Time.deltaTime * realTimeToGameTimeFactor;

        // travelTimeRemaining 감소
        travelTimeRemaining -= Time.deltaTime * realTimeToGameTimeFactor;

        // x축 또는 y축 방향으로만 이동
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

        // 실제 위치 반영
        transform.position = new Vector3(Location.x, Location.y, 0) + GridWorldManager.Instance.AddPos;
    }
}