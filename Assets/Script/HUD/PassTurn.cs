using System;
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

        void Start()
        {
            HandManager.playerCanPlay += onEnnemyPassed;
            CardGameManager.playerLostGame += EndBattle;
            CardGameManager.playerWonGame += EndBattle;
            CardGameManager.battleStart += StartBattle;
            gameObject.SetActive(false);
        }

        public void LaunchRound()
        {
            playerPassedTurn?.Invoke();
            button.enabled = false;
        }

        public void onEnnemyPassed()
        {
            button.enabled = true;
        }

        public void EndBattle() {
            gameObject.SetActive(false);
        }

        public void StartBattle() {
            gameObject.SetActive(true);
        }
    }
}
