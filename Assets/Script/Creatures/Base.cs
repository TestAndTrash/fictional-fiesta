using System;
using System.Collections;

namespace Assets.Script.Creatures
{
    internal class Base : Creature
    {
        public int move;
        public int health;
        public int attack;
        public int range;

        public static event Action<Base> OnTileClicked;


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
        public override void CheckDead()
        {
            if (hp < 1)
            {
                this.alive = false;
                //TODO
                OnTileClicked?.Invoke(this);
            }
        }
    }
}
