using System;
using System.Collections.Generic;
using Assets.Script;
using UnityEngine;

public class OpponentCardManager : MonoBehaviour
{
    private string opponentName = "Test Marcus";
    private Team playerTeam;
    private Board board;
    [SerializeField] private DeckManager deck;
    [SerializeField] private OpponentHandManager opponentHandManager;
    private List<CardEntry> hand = new();

    [SerializeField] private int maxHandSize;
    private int mana = 10;
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

    [SerializeField] private int strongAttackerPoint;
    [SerializeField] private int strongDefenserPoint;
    [SerializeField] private int weakHPPoint;
    [SerializeField] private int bigRangePoint;
    [SerializeField] private int highMobilityPoint;
    [SerializeField] private int nexusAlivePoint;
    [SerializeField] private int playerNexusAlivePoint;


    List<(Creature creature, int threatLevel)> creatureThreat = new List<(Creature, int)>();
    List<(CardEntry cardEntry, int cardLevel)> goodCards = new List<(CardEntry, int)>();
    List<(Creature nexus, int hp)> nexusAlive = new List<(Creature, int)>();
    List<(Creature nexus, int hp)> playerNexus = new List<(Creature, int)>();

    public event Action opponentPassedTurn;

    public void DetectThreat()
    {
        foreach (Creature creature in playerTeam.creatures)
        {
            int threatLevel = 0;
            if (creature.atk >= strongAttackerSensitivity) threatLevel += strongAttackerPoint;
            if (creature.hp >= strongDefenserSensitivity) threatLevel += strongDefenserPoint;
            if (creature.pm > highMobilitySensitivity) threatLevel += highMobilityPoint;
            if (creature.hp == weakHPSensitivity) threatLevel += weakHPSensitivity;
            if (creature.rng >= bigRangeSensitivity) threatLevel += bigRangePoint;
            if (creature.tile.GetLane().GetNexusFromTeamID(board.ennemyTeamIndex).alive) threatLevel += nexusAlivePoint;
            if (creature.tile.GetLane().GetNexusFromTeamID(board.playerTeamIndex).alive) threatLevel += playerNexusAlivePoint;
            
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


    public void DetectNexus()
    {
        foreach (Lane lane in board.lanes)
        {
            Creature currentNexus = lane.GetNexusFromTeamID(board.ennemyTeamIndex);
            if (currentNexus.alive)
            {
                nexusAlive.Add((currentNexus, currentNexus.hp));
            }
            Creature plNexus = lane.GetNexusFromTeamID(board.playerTeamIndex);
            if (plNexus.alive)
            {
                playerNexus.Add((plNexus, plNexus.hp));
            }
        }
        nexusAlive.Sort((a, b) => a.hp.CompareTo(b.hp));
        playerNexus.Sort((a, b) => a.hp.CompareTo(b.hp));
    }

    public void ChooseCardToPlay()
    {
        if (creatureThreat.Count <= 0)
        {
            PlayForNexus();
        }
        else
        {
            foreach (var creaThreat in creatureThreat)
            {
                if (remainingMana <= 0) return;
                Tile creatureTile = creaThreat.creature.tile;
                Lane currLane = creatureTile.GetLane();
                Tile firstTile = currLane.tiles[7];
                if (!currLane.GetNexusFromTeamID(board.ennemyTeamIndex).alive) return;
                if (firstTile.creature) return;
                foreach (var goodCard in goodCards)
                {
                    if (goodCard.cardEntry.cost <= remainingMana)
                    {
                        board.InvokCreature(goodCard.cardEntry.creaturePrefab, 1, firstTile);
                        remainingMana -= goodCard.cardEntry.cost;
                        opponentHandManager.DeleteACardFromHand();
                        goodCards.Remove(goodCard);
                        break;
                    }
                }
            }
            if (remainingMana >= 1)
            {
                PlayForNexus();
            }
        }

    }
    public void PlayForNexus()
    {
        List<(Creature nexus, int hp)> concernedNexusList = nexusAlive.Count > 3 ? playerNexus : nexusAlive;
        foreach (var element in concernedNexusList)
        {
            if (remainingMana <= 0) return;
            Tile firstTile = element.nexus.tile.GetLane().tiles[7];
            if (firstTile.creature) return;
            foreach (var goodCard in goodCards)
            {
                if (goodCard.cardEntry.cost <= remainingMana)
                {
                    board.InvokCreature(goodCard.cardEntry.creaturePrefab, 1, firstTile);
                    remainingMana -= goodCard.cardEntry.cost;
                    opponentHandManager.DeleteACardFromHand();
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
        ResetData();
        DetectThreat();
        DetectGoodCards();
        DetectNexus();
        ChooseCardToPlay();

        opponentPassedTurn?.Invoke();
    }

    public void ResetData()
    {
        creatureThreat = new List<(Creature, int)>();
        goodCards = new List<(CardEntry, int)>();
    }

    public void FillBoardInfo(Board brd)
    {
        board = brd;
        playerTeam = brd.GetPlayerTeam();
    }

    public void Draw(int nbOfCards)
    {
        if (hand.Count >= maxHandSize) return;
        for (int i = 0; i < nbOfCards; i++)
        {
            hand.Add(deck.DrawCardFromDeck());
            opponentHandManager.DrawCard();
        }
    }
}
