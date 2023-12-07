using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour

{
    public static bool GameEnd = false;

    public GameObject endGameUI;
    public GameObject Player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            EndScreen();
            
        }
           
    }

    public void EndScreen()
    {
        endGameUI.SetActive(true);
        Time.timeScale = 0f;
        Player.SetActive(false);
        GameEnd = true;
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("StartScreen");
    }

}