using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ControladorDialogo : MonoBehaviour
{
    public Text textoDialogo;
    public GameObject painelDialogo;
    public Button botaoProximo;

    [TextArea(3, 10)]
    public string[] falas;

    private int index = 0;
    private bool digitando = false;

    void Start()
    {
        painelDialogo.SetActive(true);
        botaoProximo.onClick.AddListener(AvancarDialogo);
        StartCoroutine(DigitarTexto(falas[index]));
    }

    void AvancarDialogo()
    {
        if (digitando)
        {
            StopAllCoroutines();
            textoDialogo.text = falas[index];
            digitando = false;
        }
        else
        {
            index++;
            if (index < falas.Length)
            {
                StartCoroutine(DigitarTexto(falas[index]));
            }
            else
            {
                painelDialogo.SetActive(false);
                // Aqui você pode carregar a próxima cena ou iniciar o jogo
            }
        }
    }

    IEnumerator DigitarTexto(string texto)
    {
        digitando = true;
        textoDialogo.text = "";
        foreach (char letra in texto)
        {
            textoDialogo.text += letra;
            yield return new WaitForSeconds(0.08f);
        }
        digitando = false;
    }
}
