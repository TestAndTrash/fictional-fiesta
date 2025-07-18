using System;
using System.Collections;
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
    public List<CardEntry> hand = new();

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
    public event Action opponentDeckIsEmpty;

    public void DetectThreat()
    {
        foreach (Creature creature in playerTeam.creatures)
        {
            int threatLevel = 0;
            if (creature.atk >= strongAttackerSensitivity) threatLevel += strongAttackerPoint;
            if (creature.hp >= strongDefenserSensitivity) threatLevel += strongDefenserPoint;
            if (creature.pm > highMobilitySensitivity) threatLevel += highMobilityPoint;
            if (creature.hp == weakHPSensitivity) threatLevel += weakHPPoint;
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

    public void PlayTurn()
    {
        mana++;
        remainingMana = mana;
        ResetData();
        DetectThreat();
        DetectGoodCards();
        DetectNexus();
        StartCoroutine(PlayTurnRoutine());
    }

    private IEnumerator PlayTurnRoutine()
    {
        List<(CardEntry, Tile)> cardsToPlay = new();

        if (creatureThreat.Count > 0)
        {
            cardsToPlay.AddRange(GetCardsToPlayForThreats());
        }

        if (remainingMana >= 1)
        {
            cardsToPlay.AddRange(GetCardsToPlayForNexus());
        }

        yield return StartCoroutine(PlayCardsSequentially(cardsToPlay));

        opponentPassedTurn?.Invoke();
    }

    private List<(CardEntry, Tile)> GetCardsToPlayForThreats()
    {
        List<(CardEntry, Tile)> selected = new();

        foreach (var creaThreat in creatureThreat)
        {
            if (remainingMana <= 0) break;
            Tile firstTile = creaThreat.creature.tile.GetLane().tiles[7];
            if (!firstTile.creature && creaThreat.creature.tile.GetLane().GetNexusFromTeamID(board.ennemyTeamIndex).alive)
            {
                foreach (var goodCard in goodCards)
                {
                    if (goodCard.cardEntry.cost <= remainingMana)
                    {
                        selected.Add((goodCard.cardEntry, firstTile));
                        remainingMana -= goodCard.cardEntry.cost;
                        goodCards.RemoveAll(entry => entry.cardEntry == goodCard.cardEntry);
                        break;
                    }
                }
            }
        }

        return selected;
    }

private List<(CardEntry, Tile)> GetCardsToPlayForNexus()
{
    List<(CardEntry, Tile)> selected = new();
    List<(Creature, int)> concernedNexusList = nexusAlive.Count > 3 ? playerNexus : nexusAlive;

    foreach (var (nexus, hp) in concernedNexusList)
    {
        if (remainingMana <= 0) break;
        Tile firstTile = nexus.tile.GetLane().tiles[7];
        if (!firstTile.creature)
        {
            foreach (var goodCard in goodCards)
            {
                if (goodCard.cardEntry.cost <= remainingMana)
                {
                    selected.Add((goodCard.cardEntry, firstTile));
                    remainingMana -= goodCard.cardEntry.cost;
                    goodCards.RemoveAll(entry => entry.cardEntry == goodCard.cardEntry);
                    break;
                }
            }
        }
    }

    return selected;
}

    private IEnumerator PlayCardsSequentially(List<(CardEntry cardEntry, Tile tile)> cardsToPlay)
    {
        foreach (var (card, tile) in cardsToPlay)
        {
            yield return StartCoroutine(PlayTheCardRoutine(card, tile));
        }
    }

    private IEnumerator PlayTheCardRoutine(CardEntry cardEntry, Tile tile)
    {
        opponentHandManager.PlayTheCard(cardEntry); // animation
        yield return new WaitForSeconds(2f); // attendre 2 secondes

        board.InvokCreature(cardEntry.creaturePrefab, 1, tile, cardEntry);
        hand.Remove(cardEntry);
    }

    public void ResetData()
    {
        creatureThreat = new List<(Creature, int)>();
        goodCards = new List<(CardEntry, int)>();
        playerNexus = new List<(Creature, int)>();
        nexusAlive = new List<(Creature, int)>();
    }

    public void FillBoardInfo(Board brd)
    {
        board = brd;
        playerTeam = brd.GetPlayerTeam();
    }

    public IEnumerator Draw(int nbOfCards)
    {
        if (deck.GetDeckCount() <= 0 && hand.Count <= 0)
        {
            opponentDeckIsEmpty?.Invoke();
        }
        if (hand.Count >= maxHandSize) yield return null;
        for (int i = 0; i < nbOfCards; i++)
        {
            hand.Add(deck.DrawCardFromDeck());
            opponentHandManager.DrawCard();
            yield return new WaitForSeconds(0.3f);
        }
    }
}
