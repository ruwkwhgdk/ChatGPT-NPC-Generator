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
    // NPC �̸�
    public string Name;

    // NPC�� ����� ��������Ʈ
    public string Sprite;

    // NPC ����
    public NPCFeature.NPCAlignment Alignment;

    // NPC ����
    public NPCFeature.NPCSpecies Species;

    // NPC ����
    public string Background;

    // json Text�� �޾Ƽ� NPCData�� ��ȯ�Ͽ� ����
    public static NPCData SetNPCData(string allText)
    {
        return JsonUtility.FromJson<NPCData>(allText);
    }
}