using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Splines;
using DG.Tweening;
using System.Collections;
using System;

public class HandManager : MonoBehaviour
{
    [SerializeField] private int maxHandSize;
    [SerializeField] private SplineContainer splineContainer;

    [SerializeField] private Transform spawnPoint;

    [SerializeField] private DeckManager deckManager;
    [SerializeField] private Board board;

    private List<GameObject> handCards = new();

    public bool canPlay = false;

    public static event Action playerCanPlay;
    public event Action playerDeckIsEmpty;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) DrawCard();
    }

    public void DrawCard()
    {
        if (handCards.Count >= maxHandSize) return;
        CardEntry drawnCard = deckManager.DrawCardFromDeck();
        if (drawnCard == null)
        {
            if(handCards.Count <= 0) playerDeckIsEmpty?.Invoke();
            return;
        }
        GameObject cardObject = Instantiate(drawnCard.cardPrefab, spawnPoint.position, spawnPoint.rotation);
        CardManager cardManager = cardObject.GetComponent<CardManager>();
        cardManager.fillCardData(drawnCard);
        cardManager.fillHandManager(this);
        cardManager.OnCardClicked += CardClicked;
        handCards.Add(cardObject);
        UpdateCardPos(cardObject.GetComponent<CardManager>());
    }
    public IEnumerator DrawFirstHand(int nbOfCards)
    {
        canPlay = false;
        for (int i = 0; i < nbOfCards; i++)
        {
            DrawCard();
            yield return new WaitForSeconds(0.3f);
        }
        canPlay = true;
    }

    private void UpdateCardPos(CardManager card)
    {
        canPlay = false;
        if (handCards.Count == 0) return;
        float cardSpacing = 1f / 15;
        float firstCardPosition = 0.5f - (handCards.Count - 1) * cardSpacing / 2;
        Spline spline = splineContainer.Spline;
        for (int i = 0; i < handCards.Count; i++)
        {
            float position = firstCardPosition + i * cardSpacing;
            Vector3 splinePosition = spline.EvaluatePosition(position);
            Vector3 forward = spline.EvaluateTangent(position);
            Vector3 up = spline.EvaluateUpVector(position);
            Quaternion rotation = Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);
            handCards[i].transform.DOLocalRotateQuaternion(rotation, 0.25f);
            handCards[i].transform.DOMove(splinePosition, 0.25f).OnComplete(() =>
            {
                card.traveling = false;
                canPlay = true;
            });
        }
        
    }

    private void ArrangeCardPos()
    {
        canPlay = false;
        if (handCards.Count == 0) return;
        float cardSpacing = 1f / 15;
        float firstCardPosition = 0.5f - (handCards.Count - 1) * cardSpacing / 2;
        Spline spline = splineContainer.Spline;
        for (int i = 0; i < handCards.Count; i++)
        {
            float position = firstCardPosition + i * cardSpacing;
            Vector3 splinePosition = spline.EvaluatePosition(position);
            Vector3 forward = spline.EvaluateTangent(position);
            Vector3 up = spline.EvaluateUpVector(position);
            Quaternion rotation = Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);
            handCards[i].transform.DOLocalRotateQuaternion(rotation, 0.25f);
            handCards[i].transform.DOMove(splinePosition, 0.25f).OnComplete(()=>
            {
                canPlay = true;
            });
        }
    }

    public void DeleteFromHand(GameObject card)
    {
        handCards.Remove(card);
        ArrangeCardPos();
    }

    private void CardClicked(CardManager cardManager)
    {
        if (!canPlay) return;
        board.SetPlaceCardMode(cardManager.card, cardManager);
    }

    public void ActivatePlay(bool active)
    {
        canPlay = active;
        if(active) playerCanPlay?.Invoke();
    }
}
