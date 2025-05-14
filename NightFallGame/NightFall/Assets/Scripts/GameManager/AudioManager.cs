using UnityEngine;

public class MusicaAmbiente : MonoBehaviour
{
    [Header("Configurações de Música")]
    public AudioClip musicaFase; // Música de fundo da fase
    [Range(0f, 1f)]
    public float volumeMusica = 0.5f; // Volume da música
    public bool tocarAutomaticamente = true; // Iniciar automaticamente
    public bool loopMusica = true; // Repetir quando terminar
    public bool manterEntreScenas = true; // Manter o objeto entre cenas
    public bool pararQuandoPlayerMorrer = true; // Parar a música quando o player morrer
    
    private AudioSource audioSource;
    
    // Singleton para garantir apenas uma instância
    private static MusicaAmbiente instancia;

    void Awake()
    {
        // Implementação simples de Singleton
        if (manterEntreScenas)
        {
            if (instancia == null)
            {
                instancia = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }
        
        // Configurar o componente de áudio
        ConfigurarAudioSource();
    }
    
    void OnEnable()
    {
        // Inscrever no evento de morte do player
        if (pararQuandoPlayerMorrer)
        {
            PlayerHealth.OnPlayerDeath += OnPlayerMorreu;
        }
    }
    
    void OnDisable()
    {
        // Cancelar inscrição no evento
        if (pararQuandoPlayerMorrer)
        {
            PlayerHealth.OnPlayerDeath -= OnPlayerMorreu;
        }
    }
    
    void Start()
    {
        if (musicaFase != null && audioSource != null && !audioSource.isPlaying)
        {
            if (tocarAutomaticamente)
            {
                TocarMusica();
            }
        }
    }


    
    private void ConfigurarAudioSource()
    {
        // Obter ou adicionar um AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Configurar o AudioSource
        audioSource.playOnAwake = false;
        audioSource.loop = loopMusica;
        audioSource.volume = volumeMusica;
        audioSource.priority = 0; // Alta prioridade
    }
    
    // Método chamado quando o player morre
    private void OnPlayerMorreu(bool isDead)
    {
        if (isDead)
        {
            PararMusica();
        }
    }
    
    // Métodos públicos para controlar a música
    
    public void TocarMusica()
    {
        if (musicaFase != null && audioSource != null)
        {
            audioSource.clip = musicaFase;
            audioSource.Play();
        }
    }
    
    public void PausarMusica()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }
    
    public void RetomarMusica()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.UnPause();
        }
    }
    
    public void PararMusica()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
    
    // Ajustar volume durante o jogo
    public void AjustarVolume(float novoVolume)
    {
        volumeMusica = Mathf.Clamp01(novoVolume);
        if (audioSource != null)
        {
            audioSource.volume = volumeMusica;
        }
    }
}