using UnityEngine;
using UnityEngine.UI;

public class PainelDocumentoController : MonoBehaviour
{
    public GameObject painelDocumento;
    public Image imagemDocumento;

    public void MostrarDocumento(Sprite imagem)
    {
        imagemDocumento.sprite = imagem;
        painelDocumento.SetActive(true);
        Time.timeScale = 0f; // Pausa o jogo
    }

    public void FecharDocumento()
    {
        painelDocumento.SetActive(false);
        Time.timeScale = 1f; // Retoma o jogo
    }
}
