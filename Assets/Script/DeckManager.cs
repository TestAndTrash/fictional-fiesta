using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class DeckManager : MonoBehaviour
{
    [SerializeField] private List<int> deck = new();
    [SerializeField] private List<int> currentDeck = new();

    [SerializeField] private CardData database;

    public TextMeshPro cardNumberDisplay = null;
    public SpriteRenderer cardBackSprite = null;
    public void Start()
    {
        RefillDeck();
        cardNumberDisplay = gameObject.transform.Find("Number").gameObject.GetComponent<TextMeshPro>();
        cardBackSprite = gameObject.GetComponent<SpriteRenderer>();
        UpdateDisplay();
    }

    public void RefillDeck()
    {
        currentDeck = new List<int>(deck);
        UpdateDisplay();
    }

    public void ReplaceDeck(List<int> newDeck)
    {
        deck = newDeck;
        currentDeck = new List<int>(deck);
    }

    public void UpdateDisplay()
    {
        if (cardNumberDisplay == null)
        {
            cardNumberDisplay = gameObject.transform.Find("Number").gameObject.GetComponent<TextMeshPro>();
        }
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
        deck.Add(cardID);
        currentDeck = new List<int>(deck);
        UpdateDisplay();
    }

    public int GetDeckCount()
    {
        return currentDeck.Count;
    }
}
