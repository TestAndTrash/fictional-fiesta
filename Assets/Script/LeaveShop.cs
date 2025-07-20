using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script.HUD
{
    [RequireComponent(typeof(Button))]

    public class LeaveShop : MonoBehaviour
    {
        [SerializeField]
        private Button button;
        public static event Action playerLeaveShop;

        void Start()
        {
            GameManager.shopOpen += onShopOpen;
            GameManager.closeShop += onShopClose;
            gameObject.SetActive(false);
        }

        public void Leave()
        {
            playerLeaveShop?.Invoke();
            gameObject.SetActive(false);
        }

        public void onShopClose()
        {
            gameObject.SetActive(false);
        }

        public void onShopOpen()
        {
            gameObject.SetActive(true);
        }
    }
}
