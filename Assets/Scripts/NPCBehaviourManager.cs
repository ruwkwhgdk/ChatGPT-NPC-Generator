using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviourManager : MonoBehaviour
{
    public struct NPCBehaviour
    {
        public string NPCName;
        public string CurrentTime;
        public Vector2Int Location;
        public int TravelTime;
        public int ActionTime;
        public string Action;
        public string Details;

        public NPCBehaviour(string name, string currentTime, Vector2Int location, int travelTime, int actionTime, string action, string details)
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
            var npcBehaviourStr = "NPC Name : " + NPCName + "\nCurrent TIme : " + CurrentTime + "\nLocation : " + Location.ToString() +
                "\nTravelTime : " + TravelTime + " Minute\nActionTime : " + ActionTime + " Minute\nAction : " + Action + "\nDetails : " + Details;
            return npcBehaviourStr;
        }
    }

    // NPC 행동들을 관리하는 리스트
    private List<NPCBehaviour> npcBehaviours = new List<NPCBehaviour>();

    // 메서드를 추가하여 NPC 행동을 관리할 수 있습니다.
    public void AddNPCBehaviour(NPCBehaviour behaviour)
    {
        npcBehaviours.Add(behaviour);
    }

    public void RemoveNPCBehaviour(NPCBehaviour behaviour)
    {
        npcBehaviours.Remove(behaviour);
    }
}
