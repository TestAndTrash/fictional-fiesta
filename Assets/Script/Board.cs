using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField]
    List<Lane> lanes;
    [SerializeField]
    List<Team> teams;
    public int playerTeamIndex { get; private set; } = 0;
    public int ennemyTeamIndex { get; private set; } = 1;



    public void InvokCreature(Creature prefab, int team, Tile tile)
    {
        Creature newCreature = Instantiate(prefab, teams[team].transform);
        newCreature.GetComponent<Creature>().team = team;
        newCreature.updateTile(tile);
        newCreature.transform.position = tile.transform.position;

        this.teams[team].creatures.Add(newCreature);
        if (newCreature.team == ennemyTeamIndex)
        {
            newCreature.GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    public void RunAction()
    {
        StartCoroutine(CoroutineActions());
    }

    public IEnumerator CoroutineActions()
    {
        //foreach (Creature creature in teams[playerTeamIndex].creatures)
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

        //foreach (Creature creature in teams[ennemyTeamIndex].creatures)
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

    public List<Tile> GetPlayerTiles()
    {
        List<Tile> playerTiles = new();
        foreach (Lane lane in lanes)
        {
            playerTiles.Add(lane.tiles[0]);
            lane.tiles[0].gameObject.GetComponent<Hover>().MakeHoverable();
        }
        return playerTiles;
    }
}
