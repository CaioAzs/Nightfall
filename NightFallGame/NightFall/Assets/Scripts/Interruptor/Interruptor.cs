using UnityEngine;
using UnityEngine.Rendering.Universal; // Para o Light2D
using UnityEngine.UI; // Para acessar os elementos da UI

public class Interruptor : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Light2D globalLight; // Referência para a luz global
    [SerializeField] private Light2D playerLight; // Referência para a luz do player
    [SerializeField] private GameObject bateriaBarra; // Referência para a barra de bateria
    [SerializeField] private GameObject bossHealthBar; // Referência para a barra de vida do boss
    
    [Header("Áudio")]
    [SerializeField] private AudioClip musicaBoss; // Música para a luta com o boss
    [Range(0f, 1f)]
    [SerializeField] private float volumeMusicaBoss = 0.5f; // Volume da música do boss
    
    [Header("Configurações do Brilho")]
    [SerializeField] private float intensidadeMin = 0.5f;
    [SerializeField] private float intensidadeMax = 1.5f;
    [SerializeField] private float velocidade = 2f;
    
    [Header("Configurações da Sequência")]
    [SerializeField] private float tempoDesativarControles = 2f; // Tempo para reativar controles do player
    
    private SpriteRenderer spriteRenderer;
    private Collider2D interruptorCollider;
    private Color corOriginal;
    private bool isActivated = false;
    private MusicaAmbiente musicaAmbiente;

    void Start()
    {
        // Busca a referência para o MusicaAmbiente
        musicaAmbiente = FindObjectOfType<MusicaAmbiente>();
        
        // Pega a referência do SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            corOriginal = spriteRenderer.color;
        }
        
        // Pega a referência do Collider2D
        interruptorCollider = GetComponent<Collider2D>();
        
        // Certifique-se de que a barra de vida do boss está desativada no início
        if (bossHealthBar != null)
            bossHealthBar.SetActive(false);
    }

    void Update()
    {
        // Se o interruptor ainda não foi ativado, faz o efeito de brilho
        if (!isActivated && spriteRenderer != null)
        {
            // Efeito de brilho pulsante - igual ao do ItemBateria
            float intensidade = Mathf.Lerp(intensidadeMin, intensidadeMax, Mathf.PingPong(Time.time * velocidade, 1));
            spriteRenderer.color = corOriginal * intensidade;
            
            // Verifica se o jogador clicou no interruptor
            CheckForClick();
        }
    }

    void CheckForClick()
    {
        // Verifica se o botão esquerdo do mouse foi pressionado
        if (Input.GetMouseButtonDown(0))
        {
            // Converte a posição do mouse para coordenadas do mundo
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            // Verifica se o clique acertou este objeto
            Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition);
            
            if (hitCollider != null && hitCollider.gameObject == gameObject)
            {
                // Impede que o evento de clique propague para outros sistemas (como atirar)
                // Importante: Certifique-se de que você adicionou o script PlayerInputManager
                PlayerInputManager.isInputBlocked = true;
                
                // Ativa o interruptor
                ActivateInterruptor();
                
                // Agenda a reativação dos controles após o tempo configurado
                Invoke("ReativarControlesPlayer", tempoDesativarControles);
            }
        }
    }
    
    void ReativarControlesPlayer()
    {
        // Reativa os controles do player após o tempo definido
        PlayerInputManager.isInputBlocked = false;
    }

    void ActivateInterruptor()
    {
        isActivated = true;
        
        // Desativa o collider para evitar que tiros colidam com o interruptor
        if (interruptorCollider != null)
        {
            interruptorCollider.enabled = false;
        }
        
        // Restaura a cor original do sprite (remove o efeito de brilho)
        if (spriteRenderer != null)
        {
            spriteRenderer.color = corOriginal; // Volta à cor normal sem efeito de brilho
        }
        
        // Ativa a luz global
        if (globalLight != null)
            globalLight.intensity = 1f;
        
        // Desativa a luz do player
        if (playerLight != null)
            playerLight.gameObject.SetActive(false);
        
        // Desativa a barra de bateria
        if (bateriaBarra != null)
            bateriaBarra.SetActive(false);
        
        // Ativa a barra de vida do boss
        if (bossHealthBar != null)
            bossHealthBar.SetActive(true);
        
        // Troca a música usando o MusicaAmbiente existente
        if (musicaAmbiente != null && musicaBoss != null)
        {
            // Para a música atual
            musicaAmbiente.PararMusica();
            
            // Troca o clip para a música do boss
            musicaAmbiente.musicaFase = musicaBoss;
            
            // Ajusta o volume se necessário
            musicaAmbiente.AjustarVolume(volumeMusicaBoss);
            
            // Inicia a nova música
            musicaAmbiente.TocarMusica();
        }
        else
        {
            // Se não encontrar o MusicaAmbiente existente, cria um novo para a música do boss
            if (musicaBoss != null)
            {
                GameObject novoMusicaObj = new GameObject("MusicaBoss");
                MusicaAmbiente novaMusicaAmbiente = novoMusicaObj.AddComponent<MusicaAmbiente>();
                novaMusicaAmbiente.musicaFase = musicaBoss;
                novaMusicaAmbiente.volumeMusica = volumeMusicaBoss;
                novaMusicaAmbiente.tocarAutomaticamente = true;
                novaMusicaAmbiente.loopMusica = true;
                novaMusicaAmbiente.TocarMusica();
            }
        }
    }
}