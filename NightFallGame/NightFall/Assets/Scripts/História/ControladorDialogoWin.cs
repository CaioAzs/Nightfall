using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ControladorDialogoWin : MonoBehaviour
{
    public Text textoDialogo;
    public GameObject painelDialogo;
    
    [Header("Configurações")]
    [TextArea(3, 10)]
    public string[] falas;
    [SerializeField] private float velocidadeTexto = 0.08f;
    
    [Header("Restauração de Estado")]
    [SerializeField] private bool resetarPlayerAoTerminar = true;
    [SerializeField] private bool resetarGameManagerAoTerminar = true;
    [SerializeField] private bool reativarMusicaAoTerminar = true;
    
    private enum EstadoDialogo { Digitando, ProntoParaAvancar }
    private EstadoDialogo estadoAtual = EstadoDialogo.Digitando;
    private Coroutine rotinaDigitacao = null;

    void Start()
    {
        painelDialogo.SetActive(true);
        IniciarDialogo();
        
        // Garantir que a música está tocando durante o diálogo
        if (reativarMusicaAoTerminar)
        {
            MusicaAmbiente musicaAmbiente = FindObjectOfType<MusicaAmbiente>();
            if (musicaAmbiente != null && !musicaAmbiente.GetComponent<AudioSource>().isPlaying)
            {
                musicaAmbiente.TocarMusica();
            }
        }
    }

    void Update()
    {
        // Aceitar qualquer uma destas entradas para avançar o diálogo
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            TratarClique();
        }
    }

    void TratarClique()
    {
        switch (estadoAtual)
        {
            case EstadoDialogo.Digitando:
                CompletarDialogo();
                break;
            case EstadoDialogo.ProntoParaAvancar:
                FinalizarDialogo();
                break;
        }
    }

    void IniciarDialogo()
    {
        estadoAtual = EstadoDialogo.Digitando;
        string textoCompleto = string.Join("\n\n", falas);
        rotinaDigitacao = StartCoroutine(DigitarTexto(textoCompleto));
    }

    void CompletarDialogo()
    {
        if (rotinaDigitacao != null)
        {
            StopCoroutine(rotinaDigitacao);
            rotinaDigitacao = null;
        }

        string textoCompleto = string.Join("\n\n", falas);
        textoDialogo.text = textoCompleto + "\n\n<color=#FFFF00>Clique para continuar...</color>";
        estadoAtual = EstadoDialogo.ProntoParaAvancar;
    }

    IEnumerator DigitarTexto(string texto)
    {
        textoDialogo.text = "";

        foreach (char letra in texto)
        {
            textoDialogo.text += letra;
            yield return new WaitForSeconds(velocidadeTexto);
        }

        textoDialogo.text += "\n\n<color=#FFFF00>Clique para continuar...</color>";
        estadoAtual = EstadoDialogo.ProntoParaAvancar;
        rotinaDigitacao = null;
    }

    void FinalizarDialogo()
    {
        // Resetar o player se necessário antes de mudar de cena
        if (resetarPlayerAoTerminar)
        {
            PrepararPlayerParaReinicio();
        }
        
        // Resetar o GameManager se necessário
        if (resetarGameManagerAoTerminar)
        {
            PrepararGameManagerParaReinicio();
        }
        
        // Garantir que a música esteja tocando
        if (reativarMusicaAoTerminar)
        {
            RestaurarMusica();
        }
        
        // Aguardar um pequeno delay para garantir que tudo foi preparado
        StartCoroutine(MudarParaTelaVitoria());
    }
    
    private IEnumerator MudarParaTelaVitoria()
    {
        // Pequeno delay para garantir que as preparações foram concluídas
        yield return new WaitForSeconds(0.2f);
        
        // Ir para a tela de vitória
        SceneController.instance.LoadYouWin();
    }
    
    private void PrepararPlayerParaReinicio()
    {
        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth != null)
        {
            Debug.Log("Resetando player antes da tela de vitória");
            playerHealth.ResetPlayer();
            
            // Como é uma vitória, podemos adicionalmente restaurar a saúde para o máximo
            if (GameManager.Instance != null)
            {
                float saudeMaxima = GameManager.Instance.playerMaxHealth;
                float diferenca = saudeMaxima - playerHealth.currentHealth;
                if (diferenca > 0)
                {
                    // "Curar" o player sem usar TakeDamage (que aplicaria efeitos de dano)
                    // Aqui assumimos que você tem acesso à variável currentHealth
                    // Se for privada, você precisará adicionar um método público de cura no PlayerHealth
                    var prop = playerHealth.GetType().GetProperty("currentHealth");
                    if (prop != null && prop.CanWrite)
                    {
                        prop.SetValue(playerHealth, saudeMaxima);
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("PlayerHealth não encontrado para reset");
        }
    }
    
    private void PrepararGameManagerParaReinicio()
    {
        if (GameManager.Instance != null)
        {
            Debug.Log("Preparando GameManager para reinício do jogo");
            
            // Restaurar a saúde salva para o máximo (é uma vitória!)
            GameManager.Instance.playerSavedHealth = GameManager.Instance.playerMaxHealth;
            
            // Se tiver, use o método ForcarBuscaReferencias
            // que adicionamos na implementação melhorada do GameManager
            var forcarBuscaMethod = GameManager.Instance.GetType().GetMethod("ForcarBuscaReferencias");
            if (forcarBuscaMethod != null)
            {
                forcarBuscaMethod.Invoke(GameManager.Instance, null);
                Debug.Log("Método ForcarBuscaReferencias encontrado e chamado");
            }
            else
            {
                // Método alternativo 
                Debug.Log("Usando método alternativo para preparar GameManager");
                GameManager.Instance.AtualizarBarras();
            }
        }
        else
        {
            Debug.LogWarning("GameManager não encontrado");
        }
    }
    
    private void RestaurarMusica()
    {
        MusicaAmbiente musicaAmbiente = FindObjectOfType<MusicaAmbiente>();
        if (musicaAmbiente != null)
        {
            AudioSource audioSource = musicaAmbiente.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                if (!audioSource.isPlaying)
                {
                    Debug.Log("Reativando música ambiente para a vitória");
                    musicaAmbiente.TocarMusica();
                }
                else
                {
                    // Se já estiver tocando, garantir volume normal
                    audioSource.volume = musicaAmbiente.volumeMusica;
                }
                
                // Opcional: para a vitória, podemos aumentar um pouco o volume para um efeito triunfante
                // audioSource.volume = Mathf.Min(1.0f, musicaAmbiente.volumeMusica * 1.2f); 
            }
        }
    }
}