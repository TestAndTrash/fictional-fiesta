using System;
using System.Collections;
using UnityEngine;

namespace Assets.Script
{
    public class Creature : MonoBehaviour
    {
        public int team { get; set; } = -1;
        private int[] goal = { 7, 0 };
        private int[] direction = { 1, -1 };
        public Tile tile { get; set; } = null;

        [SerializeField]
        public int pm { get; set; } = -1;
        public int hp { get; set; } = -1;
        public int atk { get; set; } = -1;
        public int rng { get; set; } = -1;

        public virtual void Start() {}

        public virtual void DoAction()
        {
            throw new NotImplementedException();
        }
        
        public virtual IEnumerator MoveForward(Lane lane)
        {
            for (int i = 0; i < pm && (tile.pos[tile.tileIndex] != goal[team]); i++)
            {
                Tile nextTile = lane.tiles[tile.pos[tile.tileIndex]+ direction[team]];
                if (nextTile.creature == null)
                {
                    updateTile(nextTile);
                    yield return MoveToNextTile(nextTile);
                }else
                {
                    yield return Fight(nextTile);
                }
                
            }

        }

        public virtual IEnumerator MoveToNextTile(Tile nextTile)
        {
            float speed = 5.0f;
            float step = speed * Time.deltaTime;
            while (transform.position != nextTile.transform.position)
            {
                transform.position = UnityEngine.Vector2.MoveTowards(transform.position, nextTile.transform.position, step);
                yield return null;

            }
        }

        public void updateTile(Tile newTile)
        {
            if (tile != null)
            {
                tile.creature = null;
            }
            tile = newTile;
            newTile.creature = this;

        } 

        public virtual IEnumerator Fight(Tile nextTile)
        {
            throw new NotImplementedException();
        }

        public virtual void CheckDead()
        {
            if (hp < 1)
            {
                //tile.creature = null;
                
                //this.gameObject.SetActive(false);
                Destroy(this.gameObject);
            }
        }

        void OnDestroy()
        {
            tile.creature = null;
        }
    }
}
