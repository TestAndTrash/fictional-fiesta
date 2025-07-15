using System;

using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSelector : MonoBehaviour
{
    public static void PlayScene(String scene)
    {
        SceneManager.LoadScene(scene);
    }
}
