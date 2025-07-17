
using System.Collections;
using UnityEngine;

namespace Assets.Script.Creatures
{
    internal class MeleCreature : Creature
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
        }

        public override IEnumerator MoveForward(Lane lane)
        {
            for (int i = 0; i < pm && (tile.pos[tile.tileIndex] != goal[team]) && this.alive; i++)
            {
                Tile nextTile = lane.tiles[tile.pos[tile.tileIndex] + direction[team]];
                if (nextTile.creature == null)
                {
                    updateTile(nextTile);
                    yield return MoveToNextTile(nextTile);
                }
                else
                {
                    yield return Fight(nextTile);
                }

            }

            if (tile.pos[tile.tileIndex] != goal[team] && lane.tiles[tile.pos[tile.tileIndex] + direction[team]].creature != null && this.alive)
            {
                yield return Fight(lane.tiles[tile.pos[tile.tileIndex] + direction[team]]);
            }else if (tile.pos[tile.tileIndex] == goal[team] && this.alive)
            {
                yield return Fight(lane.baseTiles[baseToAttack[team]]);
            }

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
                this.UpdateHP(this.hp - ennemy.atk);
            }
            
            yield return null;
        }


    }
}
