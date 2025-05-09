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
            Die();
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
            
            // Desabilitar ações do player
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
            
            // Desativar Rigidbody2D para impedir movimento físico
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.isKinematic = true;
            }
            
            // Desativar todos os MonoBehaviours que têm "Player" no nome
            // exceto PlayerHealth para manter a vida
            MonoBehaviour[] playerComponents = player.GetComponents<MonoBehaviour>();
            foreach (var component in playerComponents)
            {
                string componentName = component.GetType().Name;
                if (componentName.Contains("Player") && componentName != "PlayerHealth")
                {
                    component.enabled = false;
                }
            }
            
            // Opcional: mudar a animação do player para "idle" ou outra apropriada
            Animator playerAnim = player.GetComponent<Animator>();
            if (playerAnim != null)
            {
                // Resetar todos os triggers de animação que você conhece
                playerAnim.ResetTrigger("hurt");
                playerAnim.ResetTrigger("die");
                
                // Tentar ir para uma animação idle
                try {
                    playerAnim.Play("Idle"); // Ou o nome da sua animação idle
                } catch {
                    // Ignora se a animação não existir
                }
            }
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