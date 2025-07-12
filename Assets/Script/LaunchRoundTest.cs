using UnityEngine;

namespace Assets.Script
{
    internal class LaunchRoundTest : MonoBehaviour
    {
        [SerializeField]
        GameObject board;

        [SerializeField]
        ICreature creature;
        [SerializeField]
        Tile tile1;
        [SerializeField]
        Tile tile2;

        private void Start()
        {
            board.GetComponent<Board>().InvokCreature(creature, 0, tile1);
            board.GetComponent<Board>().InvokCreature(creature, 1, tile2);

        }
    }
}
