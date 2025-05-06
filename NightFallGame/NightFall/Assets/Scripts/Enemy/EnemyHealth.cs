using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header ("Health")]
    [SerializeField] private float startingHealth;
    public float currentHealth {get; private set; }
    private Animator anim;
    private bool dead;

    [Header("iFrames")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private int numberOfFlashes;
    private SpriteRenderer spriteRend;
    [SerializeField] private EnemyPatrol enemyPatrol;

    private bool invulnerable;

    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
        
        // Tenta obter o componente EnemyPatrol se não foi atribuído no inspector
        if (enemyPatrol == null)
        {
            // Tenta encontrar no próprio objeto
            enemyPatrol = GetComponent<EnemyPatrol>();
            
            // Tenta encontrar no objeto pai
            if (enemyPatrol == null && transform.parent != null)
            {
                enemyPatrol = transform.parent.GetComponent<EnemyPatrol>();
            }
            
            // Tenta encontrar por tag (como último recurso)
            if (enemyPatrol == null)
            {
                GameObject patrolObj = GameObject.FindGameObjectWithTag("EnemyPatrol");
                if (patrolObj != null)
                    enemyPatrol = patrolObj.GetComponent<EnemyPatrol>();
            }
        }
    }

    public void TakeDamage(float _damage)
    {
        if (invulnerable || dead) return;
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            // Inimigo foi atingido, mas ainda está vivo
            anim.SetTrigger("hurt");
            StartCoroutine(Invunerability());
        }
        else
        {
            if (!dead)
            {
                // Inimigo morreu
                anim.SetTrigger("die");
                
                // Desabilitar componentes de IA e comportamento
                if(enemyPatrol != null) 
                    enemyPatrol.enabled = false;

                if(GetComponent<MeleeEnemy>() != null)
                    GetComponent<MeleeEnemy>().enabled = false;
                
                // Desativar todos os colliders
                DisableColliders();
                
                // Opcional: adicionar pontuação, spawnar item, etc.
                
                dead = true;
            }
        }
    }

    private void DisableColliders()
    {
        // Desativar todos os colliders do inimigo
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }
    }

    private IEnumerator Invunerability()
    {
        invulnerable = true;
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            spriteRend.color = Color.white;
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
        }
        invulnerable = false;
    }

    public void SetEnemyPatrol(EnemyPatrol patrol)
    {
        enemyPatrol = patrol;
    }

    public bool IsDead()
    {
        return dead;
    }
    
    
    // // Se precisar destruir o inimigo após algum tempo
    // public void DestroyAfterDelay(float delay)
    // {
    //     if (dead)
    //     {
    //         Destroy(gameObject, delay);
    //     }
    // }
}