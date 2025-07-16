using UnityEngine;
using DG.Tweening;
using System;

public class CardManager : MonoBehaviour
{
    Vector3 orignalPos;
    public CardEntry card;
    public bool traveling = true;
    public string cardName;
    bool setOnce = false;
    public event Action<CardManager> OnCardClicked;
    private HandManager handManager;

    void OnMouseEnter()
    {
        if (traveling || !handManager.canPlay) return;
        transform.DOMoveY(transform.position.y + 0.2f, 0.25f);
        if (setOnce) return;
        orignalPos = transform.position;
        setOnce = true;
    }

    void OnMouseExit()
    {
        if (traveling || !handManager.canPlay) return;
        transform.DOMoveY(orignalPos.y, 0.25f);
    }

    void OnMouseDown()
    {
        OnCardClicked?.Invoke(this);
    }

    public void UseCard()
    {
        handManager.DeleteFromHand(gameObject);
        Destroy(gameObject);
    }

    public void fillCardData(CardEntry entry)
    {
        card = entry;
        cardName = entry.cardName;
    }

    public void fillHandManager(HandManager hManager)
    {
        handManager = hManager;
    }
}
