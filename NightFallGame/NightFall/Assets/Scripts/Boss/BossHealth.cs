using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    public int health = 12;
    public int enrageThreshold = 6;
    public bool isInvulnerable = false;
    
    [Header("Efeitos de Som")]
    public AudioClip somMorte; // Som quando o boss morre
    [Range(0f, 1f)]
    public float volumeSom = 1.0f; // Volume do som
    
    [Header("Configurações de Vitória")]
    [SerializeField] private float tempoAntesDeCarregarCena = 3.0f; // Tempo antes de carregar a cena de vitória
    
    private bool isEnraged = false;
    private bool isDead = false;
    private bool isHurt = false; // Controla se o boss está atualmente na animação de dano
    private bool playerDead = false;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    
    private void OnEnable()
    {
        // Inscrever no evento de morte do player
        PlayerHealth.OnPlayerDeath += VerifyPlayerLife;
    }
    
    private void OnDisable()
    {
        // Cancelar inscrição para evitar memory leaks
        PlayerHealth.OnPlayerDeath -= VerifyPlayerLife;
    }

    public void TakeDamage(int damage)
    {
        if (isInvulnerable || isDead)
            return;

        health -= damage;
        
        if (health <= enrageThreshold && !isEnraged)
        {
            isEnraged = true;
            animator.SetBool("IsEnraged", true);
        }
        
        // Se não estiver enraivecido e não estiver no meio de uma animação de dano
        if (!isEnraged && !isHurt)
        {
            isHurt = true;
            animator.SetTrigger("Hurt");
            isHurt = false;
        }

        if (health <= 0 && !isDead)
        {
            StartCoroutine(DieSoonThenLoadVictoryScene());
        }
    }

    private IEnumerator DieSoonThenLoadVictoryScene()
    {
        // Aciona o método de morte
        Die();
        
        // Espera um tempo para mostrar a animação de morte
        yield return new WaitForSeconds(tempoAntesDeCarregarCena);
        
        // Carrega a cena de vitória
        if (SceneController.instance != null)
        {
            SceneController.instance.LoadDialogueWinGame();
            Debug.Log("Carregando cena de vitória...");
        }
        else
        {
            Debug.LogWarning("SceneController não encontrado! Não foi possível carregar a cena de vitória.");
        }
    }

    void Die()
    {
        if(!playerDead){
            isDead = true;
            
            // Parar a música ambiente
            MusicaAmbiente musicaAmbiente = FindObjectOfType<MusicaAmbiente>();
            if (musicaAmbiente != null)
            {
                musicaAmbiente.PararMusica();
            }
            
            // Tocar som de morte do boss
            if (somMorte != null)
            {
                AudioSource.PlayClipAtPoint(somMorte, transform.position, volumeSom);
            }
        
            // Desativar componentes de comportamento do boss
            var bossComponents = GetComponents<MonoBehaviour>();
            foreach (var component in bossComponents)
            {
                if (component != this)
                    component.enabled = false;
            }
            
            // Desativar colliders
            Collider2D[] colliders = GetComponents<Collider2D>();
            foreach (Collider2D collider in colliders)
            {
                collider.enabled = false;
            }
            
            // Acionar animação de morte
            animator.SetTrigger("Die");
            
            // Desabilitar ações do player temporariamente para evitar interações durante a sequência
            DesabilitarAcoesDoPlayer();
        }
             
    }
    
    // Método para desabilitar todas as ações do player
    private void DesabilitarAcoesDoPlayer()
    {
        // Encontrar o player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null)
        {
            // Desativar movimentos do player
            var playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
                playerMovement.enabled = false;
            
            // Opcional: desativar outros componentes do player
            // mas manter o PlayerHealth ativo
        }
    }

    public bool IsEnraged()
    {
        return isEnraged;
    }
    
    // Método que será chamado quando o player morrer
    public void VerifyPlayerLife(bool dead)
    {
        playerDead = true;

        if(dead && !isDead)
        {
            animator.SetTrigger("PlayerDead");
        }
    }

    public void BossInraged(){
        StartCoroutine(MakeInvulnerable(3,2));
    }

    // Efeito visual de invulnerabilidade
    public IEnumerator MakeInvulnerable(float duration, int flashes)
    {
        isInvulnerable = true;
        
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            Color flashColor = new Color(1, 0.5f, 0.5f, 0.7f);
            
            for (int i = 0; i < flashes; i++)
            {
                spriteRenderer.color = flashColor;
                yield return new WaitForSeconds(duration / (flashes * 2));
                spriteRenderer.color = originalColor;
                yield return new WaitForSeconds(duration / (flashes * 2));
            }
        }
        else
        {
            yield return new WaitForSeconds(duration);
        }
        
        isInvulnerable = false;
    }
}