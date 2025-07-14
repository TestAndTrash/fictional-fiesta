using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script.HUD
{
    [RequireComponent(typeof(Button))]
    
    internal class PassTurn : MonoBehaviour
    {
        [SerializeField]
        private Button button;
        public static event Action playerPassedTurn;

        public void LaunchRound()
        {
            playerPassedTurn?.Invoke();
        }

//handle clickability
        private IEnumerator RoundCoroutine()
        {
            button.enabled = false;
            Board board = GameObject.Find("Board").GetComponent<Board>();
            if (board != null)
            {
            }
            button.enabled = true;

            yield return null;
        }


    }
}
