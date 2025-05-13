using UnityEngine;
using UnityEngine.UI;

public class PainelDocumentoController : MonoBehaviour
{
    public GameObject painelDocumento;
    public Image imagemDocumento;
    public static bool documentoAberto = false;
    public void MostrarDocumento(Sprite imagem)
    {
        imagemDocumento.sprite = imagem;
        painelDocumento.SetActive(true);
        Time.timeScale = 0f; // Pausa o jogo
        documentoAberto = true;
    }

    public void FecharDocumento()
    {
        painelDocumento.SetActive(false);
        Time.timeScale = 1f; // Retoma o jogo
        documentoAberto = false;
    }
}
