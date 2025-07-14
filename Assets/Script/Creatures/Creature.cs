using System;
using System.Collections;
using System.Drawing;
using TMPro;
using UnityEngine;

namespace Assets.Script
{
    public class Creature : MonoBehaviour
    {
        public int team { get; set; } = -1;
        public Tile tile { get; set; } = null;

        [SerializeField]
        public int pm { get; set; } = -1;
        public int hp { get; private set; } = -1;
        public int atk { get; private set; } = -1;
        public int rng { get; set; } = -1;
        public bool alive { get; set; } = true;

        private TextMeshPro hpDisplay = null;
        private TextMeshPro atkDisplay = null;


        public virtual void Start() {
            Transform display = gameObject.transform.Find("CreatureUI/HP_UI/HP");
            if (display != null)
            {
                hpDisplay = gameObject.transform.Find("CreatureUI/HP_UI/HP").gameObject.GetComponent<TextMeshPro>();
            }

            
            display = gameObject.transform.Find("CreatureUI/ATK_UI/ATK");
            if (display != null)
            {
                atkDisplay = gameObject.transform.Find("CreatureUI/ATK_UI/ATK").gameObject.GetComponent<TextMeshPro>();
            }

        }

        public void UpdateHP(int newHP)
        {
            this.hp = newHP;
            if (hpDisplay != null)
            {
                hpDisplay.text = this.hp.ToString();
            }
            CheckDead();
        } 
        
        public void UpdateATK(int newATK)
        {
            this.atk = newATK;
            if (atkDisplay != null)
            {
                atkDisplay.text = this.atk.ToString();
            }
        }
        
        public virtual IEnumerator MoveForward(Lane lane)
        {
            throw new NotImplementedException();

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

        public virtual void Kill()
        {
            this.alive = false;
            tile.creature = null;
            this.gameObject.SetActive(false);
        }

        public virtual void CheckDead()
        {
            if (hp < 1)
            {
                Kill();
            }
        }

        void OnDestroy()
        {
            tile.creature = null;
        }
    }
}
