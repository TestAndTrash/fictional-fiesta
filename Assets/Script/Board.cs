using System;
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



    public void InvokCreature(ICreature prefab, int team, Tile tile)
    {
        ICreature newCreature = Instantiate(prefab, teams[team].transform);
        newCreature.GetComponent<ICreature>().team = team;
        newCreature.GetComponent<ICreature>().tile = tile;
        newCreature.transform.position = tile.transform.position;

        this.teams[team].creatures.Add(newCreature);
        if (newCreature.team == ennemyTeamIndex)
        {
            newCreature.GetComponent<SpriteRenderer>().flipX = true;
        }


    }

    public void RunAction()
    {
        foreach (Team team in teams)
        {
            foreach (ICreature creature in team.creatures)
            {
                Tile tile = creature.tile.GetComponent<Tile>();
                Lane lane = lanes[tile.pos[tile.laneIndex]];
                StartCoroutine(creature.MoveForward(lane));
            }
        }
    }
}
