using TMPro;
using UnityEngine;
using DG.Tweening;
using System;

public class OpponentCardDisplay : MonoBehaviour
{
    private TextMeshPro nameDisplay = null;
    private TextMeshPro deckDisplay = null;
    private TextMeshPro givenCardDisplay = null;
    private TextMeshPro coinDisplay = null;
    private SpriteRenderer portrait = null;

    public bool traveling = false;

    Vector3 originalPos;
    bool setOnce = false;

    public static event Action<OpponentCardDisplay> OnCardDisplayClicked;

    public Opponent opponentData;

    void Start()
    {
        InitDisplay();
    }

    public void InitDisplay()
    {
        if (portrait != null) return;
        portrait = gameObject.transform.Find("Image").gameObject.GetComponent<SpriteRenderer>();
        nameDisplay = gameObject.transform.Find("Name").gameObject.GetComponent<TextMeshPro>();
        deckDisplay = gameObject.transform.Find("DeckCount").gameObject.GetComponent<TextMeshPro>();
        givenCardDisplay = gameObject.transform.Find("GivenCardNumber").gameObject.GetComponent<TextMeshPro>();
        coinDisplay = gameObject.transform.Find("GivenGold").gameObject.GetComponent<TextMeshPro>();
    }

    public void FillCardData(Opponent opponent)
    {
        Debug.Log(opponent);
        Debug.Log(opponent.opponentSprite);

        InitDisplay();
        opponentData = opponent;
        portrait.sprite = opponent.opponentSprite;
        givenCardDisplay.text = opponent.cardGiven.ToString();
        deckDisplay.text = opponent.deck.Count.ToString();
        nameDisplay.text = opponent.name;
        coinDisplay.text = opponent.goldGiven.ToString();
        ToggleDisplay(true);
    }

    public void ToggleDisplay(bool toggle)
    {
        portrait.enabled = toggle;
        nameDisplay.enabled = toggle;
        deckDisplay.enabled = toggle;
        givenCardDisplay.enabled = toggle;
        coinDisplay.enabled = toggle;
    }

    void OnMouseEnter()
    {
        if (traveling) return;
        transform.DOMoveY(transform.position.y + 0.2f, 0.25f);
        if (setOnce) return;
        originalPos = transform.position;
        setOnce = true;
    }

    void OnMouseExit()
    {
        if (traveling) return;
        transform.DOMoveY(originalPos.y, 0.25f);
    }

    void OnMouseDown()
    {
        OnCardDisplayClicked?.Invoke(this);
    }
}
