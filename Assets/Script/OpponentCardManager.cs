using System.Collections.Generic;
using Assets.Script;
using UnityEngine;

public class OpponentCardManager : MonoBehaviour
{
    private string opponentName = "Test Marcus";
    private Team playerTeam;

    [SerializeField] private DeckManager deck;

    private List<CardEntry> hand;

    private int mana = 0;
    private int remainingMana = 0;

    //SENSITIVITY
    [SerializeField] private int strongAttackerSensitivity;
    [SerializeField] private int strongDefenserSensitivity;
    [SerializeField] private int weakHPSensitivity;
    [SerializeField] private int bigRangeSensitivity;
    [SerializeField] private int highMobilitySensitivity;

    //PRIORITY
    [SerializeField] private int strongAttackerPriority;
    [SerializeField] private int strongDefenserPriority;
    [SerializeField] private int weakHPPriority;
    [SerializeField] private int bigRangePriority;
    [SerializeField] private int highMobilityPriority;
    [SerializeField] private int nexusDangerPriority;

    List<(Creature creature, int threatLevel)> creatureThreat = new List<(Creature, int)>();
    List<(CardEntry cardEntry, int cardLevel)> goodCards = new List<(CardEntry, int)>();


    public void DetectThreat()
    {
        foreach (Creature creature in playerTeam.creatures)
        {
            int threatLevel = 0;
            if (creature.atk >= strongAttackerSensitivity) threatLevel++;
            if (creature.hp >= strongDefenserSensitivity) threatLevel++;
            if (creature.pm > highMobilitySensitivity) threatLevel++;
            if (creature.hp == weakHPSensitivity) threatLevel++;
            if (creature.rng >= bigRangeSensitivity) threatLevel++;
            creatureThreat.Add((creature, threatLevel));
        }
        creatureThreat.Sort((a, b) => b.threatLevel.CompareTo(a.threatLevel));
    }

    public void DetectGoodCards()
    {
        foreach (CardEntry card in hand)
        {
            int cardLevel = 0;
            if (card.atk >= strongAttackerSensitivity) cardLevel++;
            if (card.hp >= strongDefenserSensitivity) cardLevel++;
            if (card.pm > highMobilitySensitivity) cardLevel++;
            if (card.hp == weakHPSensitivity) cardLevel++;
            if (card.rng >= bigRangeSensitivity) cardLevel++;
            goodCards.Add((card, cardLevel));
        }
        goodCards.Sort((a, b) => b.cardLevel.CompareTo(a.cardLevel));
    }


    public void ChooseCardToPlay()
    {
        foreach (var creaThreat in creatureThreat)
        {
            Tile creatureTile = creaThreat.creature.tile;
            //int LaneNb = creatureTile.pos[0];
            Lane l = null;
            if (l.tiles[7].creature) return;
            foreach (var goodCard in goodCards)
            {
                if (goodCard.cardEntry.cost <= remainingMana)
                {
                    //POSE CHANGE CARD ENTRY TO CARDMANAGER
                    goodCards.Remove(goodCard);
                    break;
                }
            }
        }
    }
    public void PlayTurn()
    {
        mana++;
        remainingMana = mana;
        DetectThreat();
        DetectGoodCards();
    }

    public void FillPlayerTeam(Team team)
    {
        playerTeam = team;
    }
    
    public void Draw(int nbOfCards)
    {
        for (int i = 0; i < nbOfCards; i++)
        {
            hand.Add(deck.DrawCardFromDeck());
        }
    }
}
