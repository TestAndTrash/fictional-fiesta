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
    }
}
