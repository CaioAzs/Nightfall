using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ControladorDialogoElevador1 : MonoBehaviour
{
    public Text textoDialogo;
    public GameObject painelDialogo;
    
    [Header("Configurações")]
    [TextArea(3, 10)]
    public string[] falas;
    [SerializeField] private float velocidadeTexto = 0.08f;
    
    [Header("Controle de Cena")]
    [SerializeField] private SceneController sceneController;
    [SerializeField] private bool carregarFase1AoFinalizar = true;
    
    // Apenas dois estados: Digitando e Pronto
    private enum EstadoDialogo { Digitando, ProntoParaAvancar }
    private EstadoDialogo estadoAtual = EstadoDialogo.Digitando;
    private Coroutine rotinaDigitacao = null;

    void Start()
    {
        painelDialogo.SetActive(true);
        
        if (sceneController == null)
        {
            sceneController = FindObjectOfType<SceneController>();
            if (sceneController == null)
            {
                Debug.LogWarning("SceneController não encontrado!");
            }
        }
        
        // Inicia a digitação do texto
        IniciarDialogo();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TratarClique();
        }
    }

    void TratarClique()
    {
        Debug.Log("Clique detectado. Estado atual: " + estadoAtual);
        
        switch (estadoAtual)
        {
            case EstadoDialogo.Digitando:
                // Interrompe a digitação, mostra o texto completo e adiciona a mensagem de clique
                CompletarDialogo();
                break;
                
            case EstadoDialogo.ProntoParaAvancar:
                // Finaliza o diálogo e carrega a próxima cena
                FinalizarDialogo();
                break;
        }
    }

    void IniciarDialogo()
    {
        estadoAtual = EstadoDialogo.Digitando;
        
        // Cria um único texto combinando todas as falas
        string textoCompleto = string.Join("\n\n", falas);
        
        // Inicia a digitação
        rotinaDigitacao = StartCoroutine(DigitarTexto(textoCompleto));
    }

    void CompletarDialogo()
    {
        // Interrompe a digitação se estiver em andamento
        if (rotinaDigitacao != null)
        {
            StopCoroutine(rotinaDigitacao);
            rotinaDigitacao = null;
        }
        
        // Combina todas as falas em um único texto
        string textoCompleto = string.Join("\n\n", falas);
        
        // Mostra o texto completo com a mensagem de clique
        textoDialogo.text = textoCompleto + "\n\n<color=#FFFF00>Clique para começar...</color>";
        
        // Muda para o estado de pronto para avançar
        estadoAtual = EstadoDialogo.ProntoParaAvancar;
        Debug.Log("Diálogo completo. Novo estado: ProntoParaAvancar");
    }

    IEnumerator DigitarTexto(string texto)
    {
        textoDialogo.text = "";
        
        foreach (char letra in texto)
        {
            textoDialogo.text += letra;
            yield return new WaitForSeconds(velocidadeTexto);
        }
        
        // Quando termina de digitar automaticamente, adiciona a mensagem de clique
        textoDialogo.text += "\n\n<color=#FFFF00>Clique para começar...</color>";
        
        // Muda o estado para pronto para avançar
        estadoAtual = EstadoDialogo.ProntoParaAvancar;
        rotinaDigitacao = null;
        Debug.Log("Digitação concluída. Novo estado: ProntoParaAvancar");
    }
    
    void FinalizarDialogo()
    {
        Debug.Log("Finalizando diálogo e carregando próxima fase");
        
        if (carregarFase1AoFinalizar && sceneController != null)
        {
            SceneController.instance.LoadLevel2();
        }
        else
        {
            painelDialogo.SetActive(false);
        }
    }
}