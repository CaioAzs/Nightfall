using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header ("Health")]
    [SerializeField] private float startingHealth;
    public float currentHealth {get; private set; }
    private Animator anim;
    public bool dead;

    [Header("iFrames")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private int numberOfFlashes;
    private SpriteRenderer spriteRend;
    
    [Header("Efeitos de Som")]
    public AudioClip somMorte; // Som quando o player morre
    [Range(0f, 1f)]
    public float volumeSom = 1.0f; // Volume do som

    private bool invulnerable;
    private bool sequenciaMorteEmAndamento = false;

    // Evento que será chamado quando o player morrer
    public delegate void PlayerDeathEvent(bool isDead);
    public static event PlayerDeathEvent OnPlayerDeath;

    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(float _damage)
    {
        if (invulnerable || dead || sequenciaMorteEmAndamento) return;
        
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            // Player foi atingido, mas ainda está vivo
            anim.SetTrigger("hurt");
            StartCoroutine(Invunerability());
        }
        else
        {
            // Iniciar sequência de morte
            SequenciaMorte();
        }
    }

    private IEnumerator Invunerability()
    {
        invulnerable = true;
        Physics2D.IgnoreLayerCollision(10, 11, true);
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            spriteRend.color = Color.white;
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
        }
        Physics2D.IgnoreLayerCollision(10, 11, false);
        invulnerable = false;
    }
    
    // Sequência de morte do player
    private void SequenciaMorte()
    {
        sequenciaMorteEmAndamento = true;
        
        // 1. Reproduzir o som de morte
        if (somMorte != null)
        {
            AudioSource.PlayClipAtPoint(somMorte, transform.position, volumeSom);
        }
        
        // 2. Acionar a morte do player
        PlayerDead();
        
        sequenciaMorteEmAndamento = false;
    }

    public bool IsDead()
    {
        return dead;
    }
    
    // Método para redefinir player após game over
    public void ResetPlayer()
    {
        dead = false;
        sequenciaMorteEmAndamento = false;
        currentHealth = startingHealth;
        
        // Reativa o componente de movimento
        if(GetComponent<PlayerMovement>() != null)
            GetComponent<PlayerMovement>().enabled = true;
    }

    public void PlayerDead(){
        if (!dead)
        {
            anim.SetTrigger("die");
            
            // Desabilitar movimento do player
            if(GetComponent<PlayerMovement>() != null)
                GetComponent<PlayerMovement>().enabled = false;
            
            dead = true;
            
            // Dispara o evento de morte do player
            // Qualquer objeto (incluindo o boss) que estiver "ouvindo" este evento será notificado
            if (OnPlayerDeath != null)
            {
                OnPlayerDeath(dead);
            }
        }
    }
}