using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header ("Health")]
    [SerializeField] private float startingHealth = 4f;
    public float currentHealth {get; private set; }
    private Animator anim;
    public bool dead;

    [Header("iFrames")]
    [SerializeField] private float iFramesDuration = 1f;
    [SerializeField] private int numberOfFlashes = 5;
    private SpriteRenderer spriteRend;
    
    [Header("Efeitos de Som")]
    public AudioClip somMorte; 
    public AudioClip somDano;
    [Range(0f, 1f)]
    public float volumeSom = 1.0f; 
    
    [Header("Configurações")]
    [SerializeField] private float tempoAntesDeCarregarCena = 2.0f;
    [SerializeField] private bool depurarResets = false; // Para logs detalhados

    private bool invulnerable;
    private bool sequenciaMorteEmAndamento = false;
    private Rigidbody2D rb;
    private AudioSource audioSource;

    public delegate void PlayerDeathEvent(bool isDead);
    public static event PlayerDeathEvent OnPlayerDeath;

    // Propriedade para acesso à saúde máxima externamente
    public float MaxHealth => startingHealth;

    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        
        // Criar AudioSource se não existir
        if (audioSource == null && (somMorte != null || somDano != null))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    private void OnEnable()
    {
        // Se o player está sendo habilitado novamente e está morto, resetamos
        if (dead)
        {
            if (depurarResets) Debug.Log("Player habilitado enquanto morto, resetando...");
            ResetPlayer();
        }
    }

    public void TakeDamage(float _damage)
    {
        // Não permitir dano se estiver em um destes estados
        if (invulnerable || dead || sequenciaMorteEmAndamento) return;
        
        // Guardar o valor anterior para debug
        float healthBefore = currentHealth;
        
        // Aplicar dano com segurança
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);
        
        // Log se ativado
        if (depurarResets) Debug.Log($"Player tomou dano: {_damage}, saúde: {healthBefore} -> {currentHealth}");

        if (currentHealth > 0)
        {
            // Jogador machucado mas não morto
            if (anim != null && anim.gameObject.activeInHierarchy)
                anim.SetTrigger("hurt");
                
            // Tocar som de dano se disponível
            if (audioSource != null && somDano != null)
            {
                audioSource.clip = somDano;
                audioSource.volume = volumeSom;
                audioSource.Play();
            }
                
            StartCoroutine(Invunerability());
        }
        else
        {
            StartCoroutine(SequenciaMorte());
        }
        
        // Atualizar GameManager, se existir
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AtualizarBarras();
        }
    }
    
    // Método público para curar o jogador (pode ser chamado por power-ups, checkpoints, etc.)
    public void Heal(float amount)
    {
        if (dead || sequenciaMorteEmAndamento) return;
        
        float healthBefore = currentHealth;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, startingHealth);
        
        if (depurarResets) Debug.Log($"Player curado: {amount}, saúde: {healthBefore} -> {currentHealth}");
        
        // Atualizar GameManager, se existir
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AtualizarBarras();
        }
    }
    
    // Método para curar completamente o jogador
    public void HealFull()
    {
        Heal(startingHealth - currentHealth);
    }

    private IEnumerator Invunerability()
    {
        invulnerable = true;
        Physics2D.IgnoreLayerCollision(10, 11, true);
        
        for (int i = 0; i < numberOfFlashes; i++)
        {
            if (spriteRend != null)
            {
                spriteRend.color = new Color(1, 0, 0, 0.5f);
                yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
                spriteRend.color = Color.white;
                yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            }
            else
            {
                // Se não tiver SpriteRenderer, apenas esperar o tempo total
                yield return new WaitForSeconds(iFramesDuration / numberOfFlashes);
            }
        }
        
        Physics2D.IgnoreLayerCollision(10, 11, false);
        invulnerable = false;
    }
        
    private IEnumerator SequenciaMorte()
    {
        sequenciaMorteEmAndamento = true;
        
        if (depurarResets) Debug.Log("Iniciando sequência de morte do player");
        
        // Chama o método que desativa todos os controles do player
        PlayerDead();
        
        // Tocar som de morte se disponível
        if (audioSource != null && somMorte != null)
        {
            audioSource.clip = somMorte;
            audioSource.volume = volumeSom;
            audioSource.Play();
        }

        // Aguarda o tempo antes de carregar a cena de derrota
        yield return new WaitForSeconds(tempoAntesDeCarregarCena);

        // Carrega a cena de derrota
        if (SceneController.instance != null)
        {
            if (depurarResets) Debug.Log("Carregando cena de derrota após morte do player");
            SceneController.instance.LoadDialogueLoseGame();  // Cena de derrota
        }
        else
        {
            Debug.LogWarning("SceneController não encontrado, não foi possível carregar cena de derrota");
        }

        sequenciaMorteEmAndamento = false;
    }

    public bool IsDead()
    {
        return dead;
    }

    public void ResetPlayer()
    {
        if (depurarResets) Debug.Log("Iniciando reset completo do player");
        
        // Interromper qualquer coroutine em execução
        StopAllCoroutines();
        
        // Reseta flags de estado
        dead = false;
        sequenciaMorteEmAndamento = false;
        invulnerable = false;
        currentHealth = startingHealth;
        
        // Restaura a cor normal do sprite
        if (spriteRend != null)
        {
            spriteRend.color = Color.white;
        }
        
        // Reativa o PlayerMovement
        var playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
            if (depurarResets) Debug.Log("PlayerMovement reativado");
        }
        
        // Reativa o PlayerAttack
        var playerAttack = GetComponent<PlayerAttack>();
        if (playerAttack != null)
        {
            playerAttack.enabled = true;
            if (depurarResets) Debug.Log("PlayerAttack reativado");
        }
        
        // Restaura o Rigidbody para estado normal
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.gravityScale = 3.5f; // Restaura a gravidade
            rb.linearVelocity = Vector2.zero; // Use velocity em vez de linearVelocity
            if (depurarResets) Debug.Log("Rigidbody restaurado");
        }
        
        // Reativa todos os scripts que foram desativados
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        int scriptsReativados = 0;
        
        foreach (var script in scripts)
        {
            // Reativa scripts de movimento, ataque, controle ou input
            if (script != null && !script.enabled && (
                script.GetType().Name.Contains("Movement") || 
                script.GetType().Name.Contains("Attack") || 
                script.GetType().Name.Contains("Control") || 
                script.GetType().Name.Contains("Input")))
            {
                script.enabled = true;
                scriptsReativados++;
            }
        }
        
        if (depurarResets) Debug.Log($"{scriptsReativados} scripts adicionais reativados");
        
        // Desbloqueia inputs globalmente
        try {
            PlayerInputManager.isInputBlocked = false;
            if (depurarResets) Debug.Log("PlayerInputManager desbloqueado");
        } catch (System.Exception) {
            // Silenciosamente ignora se o PlayerInputManager não existir
        }
        
        // Restaura as colisões entre layers (que podem ter sido desativadas durante iFrames)
        Physics2D.IgnoreLayerCollision(10, 11, false);
        
        // Verifica se há animações pendentes e as reseta
        if (anim != null)
        {
            anim.ResetTrigger("die");
            anim.ResetTrigger("hurt");
            
            // Verificar se o objeto está ativo antes de tentar reproduzir animações
            if (anim.gameObject.activeInHierarchy)
            {
                anim.Play("Idle"); // Volta para a animação padrão
            }
            
            if (depurarResets) Debug.Log("Animator resetado");
        }
        
        // Dispara o evento OnPlayerDeath (com false para indicar que não está morto)
        if (OnPlayerDeath != null)
        {
            OnPlayerDeath(false);
            if (depurarResets) Debug.Log("Evento OnPlayerDeath(false) disparado");
        }
        
        // Notificar o GameManager para atualizar a UI
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AtualizarBarras();
            if (depurarResets) Debug.Log("Barras do GameManager atualizadas após reset");
        }
        
        Debug.Log("Player resetado completamente");
    }

    public void PlayerDead()
    {
        if (!dead)
        {
            if (depurarResets) Debug.Log("Executando PlayerDead()");
            
            // Ativa a animação de morte
            if (anim != null && anim.gameObject.activeInHierarchy)
            {
                anim.SetTrigger("die");
                if (depurarResets) Debug.Log("Animação de morte ativada");
            }

            // Desativa explicitamente o PlayerMovement
            var playerMovement = GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
                if (depurarResets) Debug.Log("PlayerMovement desativado");
            }
            
            // Desativa explicitamente o PlayerAttack
            var playerAttack = GetComponent<PlayerAttack>();
            if (playerAttack != null)
            {
                playerAttack.enabled = false;
                if (depurarResets) Debug.Log("PlayerAttack desativado");
            }
            
            // Congela o Rigidbody
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero; 
                rb.isKinematic = true;
                if (depurarResets) Debug.Log("Rigidbody congelado");
            }
            
            // Desativa todos os scripts relacionados a movimento/controle
            MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
            int scriptsDesativados = 0;
            
            foreach (var script in scripts)
            {
                if (script != null && script != this && script.enabled && (
                    script.GetType().Name.Contains("Movement") || 
                    script.GetType().Name.Contains("Attack") || 
                    script.GetType().Name.Contains("Control") || 
                    script.GetType().Name.Contains("Input")))
                {
                    script.enabled = false;
                    scriptsDesativados++;
                }
            }
            
            if (depurarResets) Debug.Log($"{scriptsDesativados} scripts adicionais desativados");
            
            // Bloqueia inputs globalmente se o sistema existir
            try {
                PlayerInputManager.isInputBlocked = true;
                if (depurarResets) Debug.Log("PlayerInputManager bloqueado");
            } catch (System.Exception) {
                // Silenciosamente ignora se o PlayerInputManager não existir
            }
            
            // Define a variável dead
            dead = true;
            
            // Dispara o evento OnPlayerDeath
            if (OnPlayerDeath != null)
            {
                OnPlayerDeath(dead);
                if (depurarResets) Debug.Log("Evento OnPlayerDeath(true) disparado");
            }
        }
    }
    
    // Método para aplicar configurações do GameManager, se necessário
    public void AplicarConfiguracoesGameManager()
    {
        if (GameManager.Instance != null)
        {
            // Aplicar saúde salva, se existir
            if (GameManager.Instance.playerSavedHealth > 0)
            {
                float saudeSalva = GameManager.Instance.playerSavedHealth;
                if (depurarResets) Debug.Log($"Aplicando saúde salva do GameManager: {saudeSalva}");
                currentHealth = Mathf.Clamp(saudeSalva, 0, startingHealth);
            }
            
            // Atualizar barras
            GameManager.Instance.AtualizarBarras();
        }
    }
    
    // Para testes via Inspector ou outros scripts
    public void DefinirSaudeAtual(float novoValor)
    {
        float valorAnterior = currentHealth;
        currentHealth = Mathf.Clamp(novoValor, 0, startingHealth);
        
        if (depurarResets) Debug.Log($"Saúde definida manualmente: {valorAnterior} -> {currentHealth}");
        
        // Verificar se o player morreu com esta mudança
        if (currentHealth <= 0 && valorAnterior > 0)
        {
            StartCoroutine(SequenciaMorte());
        }
        
        // Atualizar GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AtualizarBarras();
        }
    }
}