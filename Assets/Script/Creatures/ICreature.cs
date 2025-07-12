using System;
using System.Collections;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Assets.Script
{
    public class ICreature : MonoBehaviour
    {
        public int team { get; set; } = -1;
        public int[] goal = { 7, 0 };
        public int[] direction = { 1, -1 };
        public Tile tile { get; set; }

        [SerializeField]
        public int pm { get; set; } = -1;
        public int hp { get; set; } = -1;
        public int atk { get; set; } = -1;
        public int range { get; set; } = -1;

        private void Start()
        {
            pm = 3;
            hp = 3;
            atk = 3;
            range = 1;
        }

        public virtual void DoAction()
        {
            throw new NotImplementedException();
        }
        
        public virtual IEnumerator MoveForward(Lane lane)
        {
            for (int i = 0; i < pm && (tile.pos[tile.tileIndex] != goal[team]); i++)
            {
                Tile nextTile = lane.tiles[tile.pos[tile.tileIndex]+ direction[team]];
                this.tile = nextTile;
                yield return MoveToNextTile(nextTile);
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
    }
}
