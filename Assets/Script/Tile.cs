using UnityEngine;
using System;

namespace Assets.Script
{
    public class Tile : MonoBehaviour
    {
        [SerializeField]
        public int[] pos;
        public int laneIndex { get; private set; } = 0;
        public int tileIndex { get; private set; } = 1;
        public bool used = false;
        [SerializeField]
        public Creature creature = null;

        private Hover hover;

        public static event Action<Tile> OnTileClicked;

        void Start()
        {
            hover = gameObject.GetComponent<Hover>();
        }

        void OnMouseDown()
        {
            if (hover.isHoverable && !used)
            {
                OnTileClicked?.Invoke(this);
            }
        }

        public Lane GetLane()
        {
            return gameObject.transform.parent.GetComponent<Lane>();
        }

    }
}
