
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
            Debug.Log(attack);
            bool fighted = false;
            for (int i = 0; i < pm && (tile.pos[tile.tileIndex] != goal[team]) && this.alive; i++)
            {
                Tile nextTile = lane.tiles[tile.pos[tile.tileIndex] + direction[team]];
                if (nextTile.creature == null)
                {
                    updateTile(nextTile);
                    yield return MoveToNextTile(nextTile);
                }
                else if (!fighted)
                {
                    fighted = true;
                    yield return Fight(nextTile);
                }

            }

            if (tile.pos[tile.tileIndex] != goal[team] && lane.tiles[tile.pos[tile.tileIndex] + direction[team]].creature != null && this.alive && !fighted)
            {
                yield return Fight(lane.tiles[tile.pos[tile.tileIndex] + direction[team]]);
            }else if (tile.pos[tile.tileIndex] == goal[team] && this.alive && !fighted)
            {
                yield return Fight(lane.baseTiles[baseToAttack[team]]);
            }

        }

        public IEnumerator MoveToNextTile(Tile nextTile)
        {
            float speed = 5.0f;
            float step = speed * Time.deltaTime;
            PlayAnimation("Walk");
            while (transform.position != nextTile.transform.position)
            {
                transform.position = UnityEngine.Vector2.MoveTowards(transform.position, nextTile.transform.position, step);
                yield return null;

            }
        }

        public IEnumerator Fight(Tile nextTile)
        {
            Debug.Log("ok ?");
            Creature ennemy = nextTile.creature;            
            if (ennemy.team != this.team)
            {
                PlayAnimation("Fight");
                yield return new WaitForSeconds(1.1f);
                ennemy.PlayAnimation("Hit");
                yield return new WaitForSeconds(0.5f);
                ennemy.Hit(ennemy.hp - this.atk);
                yield return new WaitForSeconds(1f);
            }
        }
    }
}
