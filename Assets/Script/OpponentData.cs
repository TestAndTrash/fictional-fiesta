using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Opponent
{
    public int id;
    public GameObject opponentPrefab;
    public string name;
    public int goldGiven;
    public int cardGiven;
    public int cardInDeck;
    public List<int> deck;
    public Sprite opponentSprite;
}

public class OpponentData : MonoBehaviour
{
    public List<Opponent> entries;

    public Opponent GetOpponentById(int id)
    {
        foreach (Opponent entry in entries)
        {
            if (entry.id == id)
                return entry;
        }

        Debug.LogWarning($"GameObject avec l'ID {id} non trouvé !");
        return null;
    }
}