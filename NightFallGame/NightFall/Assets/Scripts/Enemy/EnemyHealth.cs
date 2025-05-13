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
    private bool playerIsDead = false;

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
    
    private void OnEnable()
    {
        // Inscrever no evento de morte do player
        PlayerHealth.OnPlayerDeath += OnPlayerDeath;
    }
    
    private void OnDisable()
    {
        // Cancelar inscrição para evitar memory leaks
        PlayerHealth.OnPlayerDeath -= OnPlayerDeath;
    }

    public void TakeDamage(float _damage)
    {
        if (invulnerable || dead || playerIsDead) return;
        
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
    
    // Método que será chamado quando o player morrer
    private void OnPlayerDeath(bool isDead)
    {
        if (isDead && !this.dead) // Se o player morreu e este inimigo não está morto
        {
            playerIsDead = true;
            
            // Desativa o patrol do inimigo
            if (enemyPatrol != null)
                enemyPatrol.enabled = false;
            
            // Desativa o componente MeleeEnemy para parar ataques
            if (GetComponent<MeleeEnemy>() != null)
                GetComponent<MeleeEnemy>().enabled = false;
            
            // Reseta todos os triggers e ativa a animação de idle
            if (anim != null)
            {
                // Reseta todos os triggers que possam estar ativos
                anim.ResetTrigger("hurt");
                anim.ResetTrigger("meleeAttack");
                
                // Força a transição para idle
                anim.SetBool("moving", false);
                
                // Se o animator tiver um parâmetro "idle", ative-o
                // Isso depende da estrutura do seu Animator
                try 
                {
                    anim.Play("Idle"); // Tenta tocar diretamente a animação Idle
                }
                catch
                {
                    // Ignora se a animação não existir
                    Debug.LogWarning("Animação 'Idle' não encontrada para o inimigo: " + gameObject.name);
                }
            }
            
            // Desabilita outros componentes que possam afetar o movimento
            MonoBehaviour[] components = GetComponents<MonoBehaviour>();
            foreach (var component in components)
            {
                string componentName = component.GetType().Name;
                // Desativa componentes relacionados a comportamento, exceto este script
                if ((componentName.Contains("Enemy") && componentName != "EnemyHealth") || 
                     componentName.Contains("AI") || 
                     componentName.Contains("Controller"))
                {
                    component.enabled = false;
                }
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
        
        // Também desativa colliders nos filhos
        foreach (Collider2D collider in GetComponentsInChildren<Collider2D>())
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
}