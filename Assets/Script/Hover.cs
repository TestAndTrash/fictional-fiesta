using UnityEngine;

namespace Assets.Script
{
    public class Hover : MonoBehaviour
    {

        private bool isHoverable;

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


        public void ColorMe(Color color)
        {
            gameObject.GetComponent<SpriteRenderer>().color = color;
        }
    }
}
