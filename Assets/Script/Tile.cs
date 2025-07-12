using UnityEngine;

namespace Assets.Script
{
    public class Tile : MonoBehaviour
    {
        [SerializeField]
        public int[] pos;
        public int laneIndex { get; private set; } = 0;
        public int tileIndex { get; private set; } = 1;

        public Creature creature { get; set; } = null;

    }
}
