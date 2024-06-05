using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviourManager : MonoBehaviour
{
    public struct NPCBehaviour
    {
        public string NPCName;
        public int CurrentTime;
        public Vector2Int Location;
        public int TravelTime;
        public int ActionTime;
        public string Action;
        public string Details;

        public NPCBehaviour(string name, int currentTime, Vector2Int location, int travelTime, int actionTime, string action, string details)
        {
            NPCName = name;
            CurrentTime = currentTime;
            Location = location;
            TravelTime = travelTime;
            ActionTime = actionTime;
            Action = action;
            Details = details;
        }

        public void DebugNPCBehaviour()
        {
            Debug.Log("NPCName : " + NPCName + "\nCurrentTime : " + CurrentTime + "\nLocation : " + Location + "\nTravelTime : " +
                TravelTime + "\nActionTime : " + ActionTime + "\nAction : " + Action + "\nDetails : " + Details);
        }

        public string GetString()
        {
            var npcBehaviourStr = "NPC Name : " + NPCName + " / Current TIme : " + CurrentTime + " / Location : " + Location.ToString() +
                " / TravelTime : " + TravelTime + " / ActionTime : " + ActionTime + " / Action : " + Action + " / Details : " + Details;
            return npcBehaviourStr;
        }
    }

    // NPC �ൿ���� �����ϴ� ����Ʈ
    private List<NPCBehaviour> npcBehaviours = new List<NPCBehaviour>();

    // �޼��带 �߰��Ͽ� NPC �ൿ�� ������ �� �ֽ��ϴ�.
    public void AddNPCBehaviour(NPCBehaviour behaviour)
    {
        npcBehaviours.Add(behaviour);
    }

    public void RemoveNPCBehaviour(NPCBehaviour behaviour)
    {
        npcBehaviours.Remove(behaviour);
    }
}