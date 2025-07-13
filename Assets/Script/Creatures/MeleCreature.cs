
using System.Collections;

namespace Assets.Script.Creatures
{
    internal class MeleCreature : Creature
    {
        public int move; 
        public int health; 
        public int attack; 
        public int range; 

        public override void Start()
        {
            pm = move;
            hp = health;
            atk = attack;
            rng = range;
        }

        public override IEnumerator Fight(Tile nextTile)
        {

            Creature ennemy = nextTile.creature;            
            ennemy.hp -= this.atk;
            this.hp -= ennemy.atk;
            ennemy.CheckDead();
            this.CheckDead();

            yield return null;
        }


    }
}
