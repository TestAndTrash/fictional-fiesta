using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using UnityEditor.Search;
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
        foreach (Creature creature in teams[playerTeamIndex].creatures)
        {
            Tile tile = creature.tile.GetComponent<Tile>();
            Lane lane = lanes[tile.pos[tile.laneIndex]];
            yield return (creature.MoveForward(lane));
        }

        foreach (Creature creature in teams[ennemyTeamIndex].creatures)
        {
            Tile tile = creature.tile.GetComponent<Tile>();
            Lane lane = lanes[tile.pos[tile.laneIndex]];
            yield return (creature.MoveForward(lane));
        }
    }




}
