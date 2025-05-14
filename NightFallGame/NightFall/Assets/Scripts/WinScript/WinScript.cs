using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class WinScript : MonoBehaviour
{
     public GameObject winMenu;

    public void GoToMainMenu(){
        SceneController.instance.LoadMenu();
    }

    public void PlayAgain(){
        SceneController.instance.LoadHistoryScene();
    }


    public void QuitGame(){
        Application.Quit();
    }
}

