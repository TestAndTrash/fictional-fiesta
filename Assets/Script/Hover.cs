using UnityEngine;

namespace Assets.Script
{
    public class Hover : MonoBehaviour
    {

        public bool isHoverable;

        private Color initialColor;
        private Color hoverColor = Color.azure;
        private Color hoverableColor = Color.blueViolet;
        void Start()
        {
            initialColor = gameObject.GetComponent<SpriteRenderer>().color;
        }

        void OnMouseEnter()
        {
            if (!isHoverable) return;
            ColorMe(hoverColor);
        }

        void OnMouseExit()
        {
            if (!isHoverable) return;
            ColorMe(hoverableColor);
        }

        public void MakeHoverable()
        {
            isHoverable = true;
            ColorMe(hoverableColor);
        }

        public void DisableHoverable()
        {
            isHoverable = false;
            ColorMe(initialColor);
        }


        public void ColorMe(Color color)
        {
            gameObject.GetComponent<SpriteRenderer>().color = color;
        }
    }
}
