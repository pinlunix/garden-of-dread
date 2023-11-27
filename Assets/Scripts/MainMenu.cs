using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class MainMenu : MonoBehaviour
{
    public AudioSource buttonHover;
    public AudioSource buttonClick;
    
    public void PlayGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    public void HoverSound()
    {
        buttonHover.Play();
    }

    public void ClickSound()
    {
        buttonClick.Play();
    }
}
