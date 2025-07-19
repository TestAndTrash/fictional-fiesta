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

    private SpriteRenderer spriteRenderer;
    private int originalSortingOrder;
    private bool chosen = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSortingOrder = spriteRenderer.sortingOrder;
    }

    void OnMouseEnter()
    {
        if (traveling || !handManager.canPlay || chosen) return;
        transform.DOMoveY(transform.position.y + 0.2f, 0.25f);
        spriteRenderer.sortingOrder = 10;
        if (setOnce) return;
        orignalPos = transform.position;
        setOnce = true;
    }

    void OnMouseExit()
    {
        if (traveling || !handManager.canPlay || chosen) return;
        transform.DOMoveY(orignalPos.y, 0.25f);
        spriteRenderer.sortingOrder = originalSortingOrder;
    }

    void OnMouseDown()
    {
        if (chosen) return;
        OnCardClicked?.Invoke(this);
    }

    public void Up(bool goingUp)
    {
        if (goingUp)
        {
            chosen = true;
            Vector3 destination = transform.position + new Vector3(0, 1.5f, 0);
            transform.DOMove(destination, 0.25f);
        }
        else
        {
            Vector3 destination = transform.position + new Vector3(0, 1.5f, 0);
            transform.DOMove(destination, 0.25f).OnComplete(() =>
            {
                chosen = false;
            });
        }
    }

    public void UseCard(int mana)
    {
        handManager.PayMana(mana);
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
