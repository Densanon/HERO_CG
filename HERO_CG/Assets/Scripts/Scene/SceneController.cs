﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
