using UnityEngine;

public class Menu : MonoBehaviour
{
    public void MenuChangeState()
    {
        if (gameObject.activeSelf)
        {
            HidePanel();
        }
        else
        {
            DisplayPanel();
        }
    }

    public void DisplayPanel()
    {
        gameObject.SetActive(true);
    }

    public void HidePanel()
    {
        gameObject.SetActive(false);
    }
    
}
