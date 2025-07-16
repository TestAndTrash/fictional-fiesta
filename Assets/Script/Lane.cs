using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Script
{
    public class Lane : MonoBehaviour
    {
        [SerializeField]
        public List<Tile> tiles;
        [SerializeField]
        public List<Tile> baseTiles;
        public int playerBaseIndex { get; private set; } = 0;
        public int ennemyBaseIndex { get; private set; } = 1;

        [SerializeField]
        private Creature basePrefab;
        [SerializeField]
        public List<Creature> bases;

        private void Start()
        {
            InvokBase(playerBaseIndex);
            InvokBase(ennemyBaseIndex);
        }

        private void InvokBase(int team)
        {
            Creature newBase = Instantiate(basePrefab, transform);

            newBase.GetComponent<Creature>().team = team;
            newBase.updateTile(baseTiles[team]);
            newBase.transform.position = baseTiles[team].transform.position;

            bases.Add(newBase);
        }

        public Creature GetNexusFromTeamID(int index)
        {
           return bases[index];
        }

        public void ClearTiles()
        {
            foreach (Tile tile in tiles)
            {
                if (tile.creature != null)
                {
                    tile.creature.Kill();
                }
            }
        }

    }
}
