using System;
using System.Collections;
using System.Drawing;
using Assets.Script.Creatures;
using TMPro;
using UnityEngine;

namespace Assets.Script
{
    [RequireComponent(typeof(CreatureAnimation))]
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

        private CardEntry card;

        public CreatureAnimation animations;

        public virtual void Start()
        {
            if (hpDisplay == null || atkDisplay == null)
            {
                InitDisplay();
            }
            
            animations = gameObject.GetComponent<CreatureAnimation>();
            animations.SelectTeam(team);

            PlayAnimation("Spawn");
        }

        public void Initiate(CardEntry cardEntry)
        {
            if (hpDisplay == null || atkDisplay == null)
            {
                InitDisplay();
            }
            card = cardEntry;
            pm = card.pm;
            UpdateHP(card.hp);
            UpdateATK(card.atk);
            rng = card.rng;
        }

        public void InitDisplay()
        {
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
            hp = newHP;
            if (hpDisplay != null)
            {

                hpDisplay.text = hp.ToString();
            }
            CheckDead();
        } 
        
        public void UpdateATK(int newATK)
        {
            atk = newATK;
            if (atkDisplay != null)
            {
                atkDisplay.text = atk.ToString();
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

        public void PlayAnimation(String name)
        {
            if ( animations != null)
            {
                animations.PlayAnimation(name);
            }
        }
        public virtual void Hit(int newHP)
        {
            UpdateHP(newHP);
        }

        public virtual void Kill()
        {
            
            this.alive = false;
            tile.creature = null;

            StartCoroutine(KillAnim("Death"));
        }

        public IEnumerator KillAnim(String name)
        {
            PlayAnimation(name);
            yield return new WaitForSeconds(1f);
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
