using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using DG.Tweening;
public class EnhanceDeck : MonoBehaviour
{
    [SerializeField] private CardData database;
    [SerializeField] private Board board;
    [SerializeField] private DeckManager playerDeck;

    [SerializeField] private SplineContainer splineContainer;

    [SerializeField] private Transform spawnPoint;

    private List<int> tempPool = new();
    private List<CardEntry> cardsToChooseFrom = new();

    private List<GameObject> physicalCards = new();

    void Start()
    {
        tempPool.Add(1);
        tempPool.Add(4);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl)) DrawRandCards(2, tempPool);
    }

    void DrawRandCards(int nbOfCards, List<int> cardPool)
    {
        for (int i = 0; i < nbOfCards; i++)
        {
            int randomInt = Random.Range(0, cardPool.Count);
            cardsToChooseFrom.Add(database.GetCardById(cardPool[randomInt]));
            cardPool.RemoveAt(randomInt);
        }
        DisplayCards();
    }

    private void DisplayCards()
    {
        foreach (CardEntry card in cardsToChooseFrom)
        {
            GameObject cardObject = Instantiate(card.cardPrefab, spawnPoint.position, spawnPoint.rotation);
            CardManager cardManager = cardObject.GetComponent<CardManager>();
            cardManager.fillCardData(card);
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
            Vector3 splinePositionTest = splineContainer.transform.TransformPoint(splinePosition);
            physicalCards[i].transform.DOMove(splinePositionTest, 0.25f).OnComplete(() =>
            {
                card.traveling = false;
            });
        }
    }

    private void CardClicked(CardManager cardManager)
    {
        playerDeck.AddCardToDeck(cardManager.card.id);
    }
}
