using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenuSc : MonoBehaviour
{

    public AudioSource hover;
    public AudioSource click;

    public void PlayGame()
    {
        click.Play();
        SceneManager.LoadScene("game");
    }

    public void Exit()
    {
        click.Play();
        Application.Quit();
    }

    public void Hover()
    {
        hover.Play();
    }

}
