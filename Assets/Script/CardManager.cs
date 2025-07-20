using UnityEngine;
using DG.Tweening;
using System;
using TMPro;

public class CardManager : MonoBehaviour
{
    Vector3 orignalPos;
    public CardEntry card;
    public bool traveling = true;
    public string cardName;
    bool setOnce = false;
    public event Action<CardManager> OnCardClicked;
    private HandManager handManager = null;

    private SpriteRenderer spriteRenderer;

    private TextMeshPro goldNumber = null;
    private SpriteRenderer goldSprite = null;
    private int originalSortingOrder;
    private bool chosen = false;

    public bool reward = false;

    public bool sell = false;
    public int price = 0;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSortingOrder = spriteRenderer.sortingOrder;
        if (goldNumber != null) return;
        InitDisplay();
        goldSprite.enabled = false;
        goldNumber.enabled = false;
    }

    public void InitDisplay()
    {
        goldNumber = gameObject.transform.Find("GoldNumber").gameObject.GetComponent<TextMeshPro>();
        goldSprite = gameObject.transform.Find("GoldSprite").gameObject.GetComponent<SpriteRenderer>();
    }

    public void ChangeSortingOrder(int order)
    {
        if(spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = order;
        originalSortingOrder = spriteRenderer.sortingOrder;
    }

    void OnMouseEnter()
    {
        if ((handManager == null || !handManager.canPlay || chosen) && !reward) return;
        if (traveling) return;
        transform.DOMoveY(transform.position.y + 0.2f, 0.25f);
        spriteRenderer.sortingOrder = 12;
        if (setOnce) return;
        orignalPos = transform.position;
        setOnce = true;
    }

    void OnMouseExit()
    {
        if ((handManager == null || !handManager.canPlay || chosen) && !reward) return;
        if (traveling) return;
        transform.DOMoveY(orignalPos.y, 0.25f);
        spriteRenderer.sortingOrder = originalSortingOrder;
    }

    void OnMouseDown()
    {
        if (chosen && !reward && !sell) return;
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

    public void DisplayGold()
    {
        if (card.price == 0) return;
        if (goldSprite == null) InitDisplay();
        goldSprite.enabled = true;
        goldNumber.enabled = true;
        goldNumber.text = price.ToString();
    }
}
