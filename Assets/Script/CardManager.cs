using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using Assets.Script;


public class CardManager : MonoBehaviour
{
    Vector3 orignalPos;
    public bool traveling = true;
    public string cardName;
    public int hp;
    public int mobility;
    public int attack;
    public int range;
    bool setOnce = false;

    private HandManager handManager;
    private Board board;

    void OnMouseEnter()
    {
        if (traveling) return;
        transform.DOMoveY(transform.position.y + 0.2f, 0.25f);
        if (setOnce) return;
        orignalPos = transform.position;
        setOnce = true;
    }

    void OnMouseExit()
    {
        if (traveling) return;
        transform.DOMoveY(orignalPos.y, 0.25f);
    }

    void OnMouseDown()
    {
        List<Tile> playerTiles = board.GetPlayerTiles();
    }

    public void fillCardData(CardEntry entry)
    {
        cardName = entry.cardName;
        hp = entry.hp;
        mobility = entry.mobility;
        attack = entry.attack;
        range = entry.range;
    }

    public void fillManagers(HandManager hManager, Board brd)
    {
        handManager = hManager;
        board = brd;
    }
}
