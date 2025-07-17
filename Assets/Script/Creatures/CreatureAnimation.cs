using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

namespace Assets.Script.Creatures
{
    public class CreatureAnimation : MonoBehaviour
    {

        [SerializeField]
        private Animator animator;
        private int team;

        public void SelectTeam(int team)
        {
            this.team = team;
            if (team == 0)
            {
                animator.SetLayerWeight(0, 1);
                animator.SetLayerWeight(1, 0);
            }
            else
            {
                animator.SetLayerWeight(0, 0);
                animator.SetLayerWeight(1, 1);
            }
            
        }

        public void PlayAnimation(string name)
        {
            if (animator.HasState(team, Animator.StringToHash(name)))
            {
                animator.Play(name, team);
            }   
        }






    }
}
