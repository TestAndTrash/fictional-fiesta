using System.Collections.Generic;
using Assets.Script;
using UnityEngine;

[System.Serializable]
public class CardEntry
{
    public int id;
    public GameObject cardPrefab;
    public Creature creaturePrefab;
    public int cost;
    public string cardName;
    public int pm;
    public int hp;
    public int atk;
    public int rng;
}

public class CardData : MonoBehaviour
{
    public List<CardEntry> entries;

    public CardEntry GetCardById(int id)
    {
        foreach (CardEntry entry in entries)
        {
            if (entry.id == id)
                return entry;
        }

        Debug.LogWarning($"GameObject avec l'ID {id} non trouvé !");
        return null;
    }
}
