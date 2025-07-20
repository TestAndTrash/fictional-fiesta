using System;
using System.Collections;
using UnityEngine;

namespace Assets.Script.Creatures
{
    public class Base : Creature
    {
        public int move;
        public int health;
        public int attack;
        public int range;

        public static event Action<Base> baseIsKilled;

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
            return null;
        }

        public override void Kill()
        {
            alive = false;
            PlayAnimation("Death");
            tile.GetLane().ClearTiles();
            baseIsKilled?.Invoke(this);
        }

        public override void CheckDead()
        {
            if (hp < 1)
            {
                Kill();
            }
        }
    }
}
