using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ControladorDialogo : MonoBehaviour
{
    public Text textoDialogo;
    public GameObject painelDialogo;
    
    [Header("Configurações")]
    [TextArea(3, 10)]
    public string[] falas;
    [SerializeField] private float velocidadeTexto = 0.08f;
    
    [Header("Controle de Cena")]
    [SerializeField] private SceneController sceneController; // Referência para o SceneController
    [SerializeField] private bool carregarFase1AoFinalizar = true; // Opção para carregar automaticamente a Fase 1
    
    private int index = 0;
    private bool digitando = false;
    private bool dialogoCompleto = false;

    void Start()
    {
        painelDialogo.SetActive(true);
        
        // Tenta encontrar o SceneController se não for atribuído
        if (sceneController == null)
        {
            sceneController = FindObjectOfType<SceneController>();
            if (sceneController == null)
            {
                Debug.LogWarning("SceneController não encontrado! Certifique-se de adicioná-lo à cena.");
            }
        }
        
        StartCoroutine(DigitarTexto(falas[index]));
    }

    void Update()
    {
        // Verifica se o botão do mouse foi pressionado
        if (Input.GetMouseButtonDown(0))
        {
            AvancarDialogo();
        }
    }

    void AvancarDialogo()
    {
        if (dialogoCompleto)
        {
            // Se o diálogo estiver completo, carrega a próxima cena
            FinalizarDialogo();
            return;
        }
        
        if (digitando)
        {
            // Se estiver digitando, acelera para mostrar todo o texto
            StopAllCoroutines();
            textoDialogo.text = falas[index];
            digitando = false;
        }
        else
        {
            // Avança para a próxima fala
            index++;
            if (index < falas.Length)
            {
                StartCoroutine(DigitarTexto(falas[index]));
            }
            else
            {
                // Chegou na última fala
                dialogoCompleto = true;
                
                // Opcional: Mostrar alguma indicação visual de que o diálogo terminou
                textoDialogo.text += "\n\n<color=#FFFF00>Clique para começar...</color>";
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
            yield return new WaitForSeconds(velocidadeTexto);
        }
        digitando = false;
    }
    
    void FinalizarDialogo()
    {
        if (carregarFase1AoFinalizar && sceneController != null)
        {
            // Usa o SceneController para carregar a Fase 1
            sceneController.LoadCena1();
        }
        else
        {
            // Apenas fecha o painel de diálogo se não quiser carregar a próxima cena
            painelDialogo.SetActive(false);
        }
    }
}