using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void LoadHistoryScene()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void LoadLevel1()
    {
        SceneManager.LoadSceneAsync(2);
    }
    public void LoadElevador1_2(){
        SceneManager.LoadSceneAsync(3);
    }

    public void LoadLevel2(){
        SceneManager.LoadSceneAsync(4);
    }

    public void LoadElevador2_3(){
        SceneManager.LoadSceneAsync(5);
    }

    public void LoadLevel3(){
        SceneManager.LoadSceneAsync(6);
    }

    public void LoadDialogueWinGame(){
        SceneManager.LoadSceneAsync(7);
    }

    public void LoadDialogueLoseGame(){
        SceneManager.LoadSceneAsync(8);
    }

    public void QuitGame(){
        Application.Quit();
    }
}
