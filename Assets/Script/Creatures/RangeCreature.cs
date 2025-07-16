using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.Creatures
{
    public class RangeCreature : Creature
    {
        public int move;
        public int health;
        public int attack;
        public int range;

        private int[] goal = { 7, 0 };
        private int[] direction = { 1, -1 };
        private int[] baseToAttack = { 1, 0 };

        public override void Start()
        {
            base.Start();
            pm = move;
            UpdateHP(health);
            UpdateATK(attack);
            rng = range;
        }

        public override IEnumerator MoveForward(Lane lane)
        {
            bool fighted = false;
            for (int i = 0; i < pm && (tile.pos[tile.tileIndex] != goal[team]) && this.alive; i++)
            {
                List<Tile> rangeTiles = GetRangedTiles(lane);
                Tile nextTile = rangeTiles[0];
                
                foreach (Tile rangeTile in rangeTiles)
                {
                    if (rangeTile.creature != null)
                    {
                        yield return Fight(rangeTile);
                        fighted = true;
                        break;
                    }
                }

                if (fighted)
                {
                    break;
                }else if (nextTile.creature == null)
                {
                    updateTile(nextTile);
                    yield return MoveToNextTile(nextTile);
                }
            }

            if (tile.pos[tile.tileIndex] != goal[team] && !fighted && this.alive)
            {
                List<Tile> rangeTiles = GetRangedTiles(lane);
                foreach (Tile rangeTile in rangeTiles)
                {
                    if (rangeTile.creature != null)
                    {
                        yield return Fight(rangeTile);
                        fighted = true;
                        break;
                    }
                }
            }
            else if (tile.pos[tile.tileIndex] == goal[team] && !fighted && this.alive)
            {
                yield return Fight(lane.baseTiles[baseToAttack[team]]);
            }
        }

        private List<Tile> GetRangedTiles(Lane lane)
        {
            int actualPos = tile.pos[tile.tileIndex];
            int sens = direction[team];

            List<Tile> tiles = new List<Tile>();
            for (int i = 1; i <= range; i++)
            {
                if ((actualPos+(i * sens) != goal[team] + sens)) {
                    tiles.Add(lane.tiles[actualPos + (i * sens)]);
                }
                else
                {
                    tiles.Add(lane.baseTiles[baseToAttack[team]]);
                    break;
                }
            }
            return tiles;
        }

        public IEnumerator MoveToNextTile(Tile nextTile)
        {
            float speed = 5.0f;
            float step = speed * Time.deltaTime;
            while (transform.position != nextTile.transform.position)
            {
                transform.position = UnityEngine.Vector2.MoveTowards(transform.position, nextTile.transform.position, step);
                yield return null;
            }
        }

        public IEnumerator Fight(Tile nextTile)
        {
            Creature ennemy = nextTile.creature;
            if (ennemy.team != this.team)
            {
                ennemy.UpdateHP(ennemy.hp - this.atk);
            }
            yield return null;
        }

    }
}
