using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField]
    List<Lane> lanes;
    [SerializeField]
    public List<Team> teams;
    public int playerTeamIndex { get; private set; } = 0;
    public int ennemyTeamIndex { get; private set; } = 1;

    private List<Tile> playerTiles = new();

    private CardEntry currentCardEntry;
    private CardManager currentCardManager;

    public event Action<bool> CaptureLane;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt)) CaptureLane?.Invoke(true);
    }
    private void OnEnable()
    {
        Tile.OnTileClicked += HandleTileClick;
    }

    private void OnDisable()
    {
        Tile.OnTileClicked -= HandleTileClick;
    }
    public void InvokCreature(Creature prefab, int team, Tile tile)
    {
        Creature newCreature = Instantiate(prefab, teams[team].transform);
        newCreature.GetComponent<Creature>().team = team;
        newCreature.updateTile(tile);
        newCreature.transform.position = tile.transform.position;

        teams[team].creatures.Add(newCreature);
        if (newCreature.team == ennemyTeamIndex)
        {
            newCreature.GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    public void RunOpponentActions()
    {
        StartCoroutine(CoroutineOpponentActions());
    }

    public IEnumerator CoroutinePlayerActions()
    {
        for (int i = teams[playerTeamIndex].creatures.Count - 1; i >= 0; i--)
        {
            Creature creature = teams[playerTeamIndex].creatures[i];
            if (creature.gameObject.activeSelf)
            {
                Tile tile = creature.tile.GetComponent<Tile>();
                Lane lane = lanes[tile.pos[tile.laneIndex]];
                yield return (creature.MoveForward(lane));
            }
            else
            {
                teams[playerTeamIndex].creatures.Remove(creature);
                Destroy(creature.gameObject);
            }
        }
    }

    public IEnumerator CoroutineOpponentActions()
    {
        for (int i = teams[ennemyTeamIndex].creatures.Count - 1; i >= 0; i--)
        {
            Creature creature = teams[ennemyTeamIndex].creatures[i];
            if (creature.gameObject.activeSelf)
            {
                Tile tile = creature.tile.GetComponent<Tile>();
                Lane lane = lanes[tile.pos[tile.laneIndex]];
                yield return (creature.MoveForward(lane));
            }
            else
            {
                teams[ennemyTeamIndex].creatures.Remove(creature);
                Destroy(creature.gameObject);
            }
        }
    }

    public void SetPlayerTiles()
    {
        foreach (Lane lane in lanes)
        {
            playerTiles.Add(lane.tiles[0]);
            lane.tiles[0].gameObject.GetComponent<Hover>().MakeHoverable();
        }
    }

    public void DisablePlaceCardMode()
    {
        foreach (Tile tile in playerTiles)
        {
            tile.gameObject.GetComponent<Hover>().DisableHoverable();
        }
    }

    public void SetPlaceCardMode(CardEntry cardEntry, CardManager cardManager)
    {
        currentCardEntry = cardEntry;
        currentCardManager = cardManager;
        SetPlayerTiles();
    }


    public void HandleTileClick(Tile clickedTile)
    {
        if (currentCardEntry != null)
        {
            InvokCreature(currentCardEntry.creaturePrefab, 0, clickedTile);
            currentCardEntry = null;
            currentCardManager.UseCard();
            DisablePlaceCardMode();
        }
    }

    public Team GetPlayerTeam()
    {
        return teams[playerTeamIndex];
    }
}
