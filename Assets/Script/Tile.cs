using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Script.Creatures;
using UnityEngine;

namespace Assets.Script
{
    public class Tile : MonoBehaviour
    {
        [SerializeField]
        public int[] pos;
        public int laneIndex { get; private set; } = 0;
        public int tileIndex { get; private set; } = 1;


    }
}
