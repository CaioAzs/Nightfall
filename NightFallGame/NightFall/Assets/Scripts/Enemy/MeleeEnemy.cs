using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private float range;
    [SerializeField] private int damage;
    [SerializeField] private float colliderDistance;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask playerLayer;
    private float cooldownTimer = Mathf.Infinity;
    private Animator anim;
    private PlayerHealth playerHealth; // Modificado para usar PlayerHealth
    [SerializeField] private EnemyPatrol enemyPatrol;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        
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

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        if (PlayerInSight())
        {
            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;
                anim.SetTrigger("meleeAttack");
            }
        }

        if (enemyPatrol != null)
            enemyPatrol.enabled = !PlayerInSight();
    }

    private bool PlayerInSight()
    {
        RaycastHit2D hit = 
            Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.left, 0, playerLayer);

        if (hit.collider != null){
            // Modificado para usar PlayerHealth em vez de Health
            PlayerHealth detectedHealth = hit.transform.GetComponent<PlayerHealth>();

            if(detectedHealth != null && detectedHealth.currentHealth > 0){
                playerHealth = detectedHealth;
                return true;
            } else{
                return false;
            }
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
    }

    private void DamagePlayer()
    {
        if(PlayerInSight() && playerHealth != null && playerHealth.currentHealth > 0){
            playerHealth.TakeDamage(damage);
        }
    }
    
    // Verificar se o inimigo está morto (útil para outros scripts)
    public bool IsEnemyDead()
    {
        EnemyHealth health = GetComponent<EnemyHealth>();
        if (health != null)
        {
            return health.IsDead();
        }
        return false;
    }
}