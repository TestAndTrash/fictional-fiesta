using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using DG.Tweening;
using System;

public class EnhanceDeck : MonoBehaviour
{
    [SerializeField] private CardData database;
    [SerializeField] private Board board;
    [SerializeField] private DeckManager playerDeck;

    [SerializeField] private SplineContainer splineContainer;

    [SerializeField] private Transform spawnPoint;
    private List<CardEntry> cardsToChooseFrom = new();

    private List<GameObject> physicalCards = new();

    public static event Action<CardManager> playerChoseCard;

    public void DrawRandCards(int nbOfCards, List<int> cardPool)
    {
        EmptyTheDisplay();
        for (int i = 0; i < nbOfCards; i++)
        {
            int randomInt = UnityEngine.Random.Range(0, cardPool.Count);
            cardsToChooseFrom.Add(database.GetCardById(cardPool[randomInt]));
            cardPool.RemoveAt(randomInt);
        }
        DisplayCards();
    }

    public void EmptyTheDisplay()
    {
        foreach (GameObject card in physicalCards)
        {
            Destroy(card);
        }
        physicalCards = new();
        cardsToChooseFrom = new();
    }

    private void DisplayCards()
    {
        foreach (CardEntry card in cardsToChooseFrom)
        {
            GameObject cardObject = Instantiate(card.cardPrefab, spawnPoint.position, spawnPoint.rotation);
            CardManager cardManager = cardObject.GetComponent<CardManager>();
            cardManager.fillCardData(card);
            cardManager.reward = true;
            cardManager.OnCardClicked += CardClicked;
            physicalCards.Add(cardObject);
            board.gameObject.SetActive(false);
            UpdateCardPos(cardObject.GetComponent<CardManager>());
        }
    }

    private void UpdateCardPos(CardManager card)
    {
        if (physicalCards.Count == 0) return;
        float cardSpacing = 1f / 3;
        float firstCardPosition = 0.5f - (physicalCards.Count - 1) * cardSpacing / 2;
        Spline spline = splineContainer.Spline;
        for (int i = 0; i < physicalCards.Count; i++)
        {
            float position = firstCardPosition + i * cardSpacing;
            Vector3 splinePosition = spline.EvaluatePosition(position);
            Vector3 splinePositionWorld = splineContainer.transform.TransformPoint(splinePosition);
            physicalCards[i].transform.DOMove(splinePositionWorld, 0.25f).OnComplete(() =>
            {
                card.traveling = false;
            });
        }
    }

    private void CardClicked(CardManager cardManager)
    {
        playerChoseCard?.Invoke(cardManager);
    }
}
