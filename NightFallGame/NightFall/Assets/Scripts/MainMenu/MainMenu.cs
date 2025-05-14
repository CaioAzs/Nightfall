using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame(){
        SceneController.instance.LoadHistoryScene();
    }

    public void QuitGame(){
        Application.Quit();
    }
}
