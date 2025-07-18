using UnityEngine;

namespace Assets.Script
{
    internal class LaunchRoundTest : MonoBehaviour
    {
        [SerializeField]
        GameObject board;

        [SerializeField]
        Creature[] creature;
        
        [SerializeField]
        Tile[] tile;

        private void Start()
        {
            /*board.GetComponent<Board>().InvokCreature(creature[0], 0, tile[1]);
            board.GetComponent<Board>().InvokCreature(creature[1], 0, tile[0]);
            board.GetComponent<Board>().InvokCreature(creature[2], 0, tile[3]);
            board.GetComponent<Board>().InvokCreature(creature[3], 0, tile[2]);
            board.GetComponent<Board>().InvokCreature(creature[0], 1, tile[4]);
            board.GetComponent<Board>().InvokCreature(creature[1], 1, tile[5]);
            board.GetComponent<Board>().InvokCreature(creature[2], 1, tile[6]);
            board.GetComponent<Board>().InvokCreature(creature[3], 1, tile[7]);*/

        }
    }
}
