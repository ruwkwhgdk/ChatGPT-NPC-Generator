using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NPCFeature
{
    public enum NPCAlignment
    {
        LawfulGood = 0,
        NeutralGood = 1,
        ChaoticGood = 2,
        LawfulNeutral = 3,
        TrueNeutral = 4,
        ChaoticNeutral = 5,
        LawfulEvil = 6,
        NeutralEvil = 7,
        ChaoticEvil = 8
    }

    public enum NPCSpecies
    {
        Human = 0,
        Halfling = 1,
        Dwarf = 2,
        Goblin = 3,
        Gnome = 4,
        Nymph = 5,
        Orc = 6,
        Elf = 7,
        Lizardman = 8,
        Kobold = 9,
        Angel = 10,
        Devil = 11,
        Dragon = 12,
        Android = 13,
        Ogre = 14,
        Ghoul = 15,
        Skeleton = 16,
        Vampire = 17,
        Etc = 18
    }
}

[System.Serializable]
public class NPCData
{
    // NPC 이름
    public string Name;

    // NPC가 사용할 스프라이트
    public string Sprite;

    // NPC 성향
    public NPCFeature.NPCAlignment Alignment;

    // NPC 종족
    public NPCFeature.NPCSpecies Species;

    // NPC 설정
    public string Background;

    // json Text를 받아서 NPCData로 변환하여 리턴
    public static NPCData SetNPCData(string allText)
    {
        return JsonUtility.FromJson<NPCData>(allText);
    }
}