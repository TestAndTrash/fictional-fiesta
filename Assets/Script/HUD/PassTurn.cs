using System.Collections;
using UnityEngine;
using UnityEngine.Splines.ExtrusionShapes;
using UnityEngine.UI;

namespace Assets.Script.HUD
{
    [RequireComponent(typeof(Button))]
    internal class PassTurn : MonoBehaviour
    {
        [SerializeField]
        private Button button;

        public void LaunchRound()
        {
            StartCoroutine(RoundCoroutine());
        }

        private IEnumerator RoundCoroutine()
        {
            button.enabled = false;
            Board board = GameObject.Find("Board").GetComponent<Board>();
            if (board != null)
            {
                yield return board.RunAction();
            }
            button.enabled = true;

            yield return null;
        }


    }
}
