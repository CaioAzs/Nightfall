using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void LoadCena1()
    {
        SceneManager.LoadScene("Level1");
    }

    public void LoadCena2()
    {
        SceneManager.LoadScene("Level2");
    }
}
