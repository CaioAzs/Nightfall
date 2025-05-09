using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void LoadCena1()
    {
        SceneManager.LoadScene("Fase1_Spaw");
    }

    public void LoadCena2()
    {
        SceneManager.LoadScene("História");
    }
}
