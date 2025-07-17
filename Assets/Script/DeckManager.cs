using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class DeckManager : MonoBehaviour
{
    [SerializeField] private List<int> deck = new();
    [SerializeField] private List<int> currentDeck = new();

    [SerializeField] private CardData database;

     private TextMeshPro cardNumberDisplay = null;

    public void Start()
    {
        RefillDeck();
        cardNumberDisplay = transform.GetChild(0).gameObject.GetComponent<TextMeshPro>();
        UpdateDisplay();
    }

    public void RefillDeck()
    {
        currentDeck = deck;
    }

    public void UpdateDisplay()
    {
        cardNumberDisplay.text = currentDeck.Count.ToString();
    }

    public CardEntry DrawCardFromDeck()
    {
        if (currentDeck.Count <= 0) return null;
        int randomInt = Random.Range(0, currentDeck.Count);
        CardEntry cardEntry = database.GetCardById(currentDeck[randomInt]);
        currentDeck.RemoveAt(randomInt);
        UpdateDisplay();
        return cardEntry;
    }

    public void AddCardToDeck(int cardID)
    {
        currentDeck.Add(cardID);
        UpdateDisplay();
    }

    public int GetDeckCount()
    {
        return currentDeck.Count;
    }
}
