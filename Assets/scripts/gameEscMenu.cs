using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gameEscMenu : MonoBehaviour
{

    public GameObject escMenu;

    public AudioSource hover;
    public AudioSource click;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible= false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            escMenu.SetActive(true);
            Cursor.visible = true;
        }
    }

    public void Continue()
    {
        click.Play();
        escMenu.SetActive(false);
        Cursor.visible = false;
    }

    public void ToMenu()
    {
        click.Play();
        SceneManager.LoadScene("menu");
    }

    public void Hover()
    {
        hover.Play();
    }

}
