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
    private Health playerHealth;
    //private EnemyPatrol enemyPatrol;
    [SerializeField] private EnemyPatrol enemyPatrol; // Adicionado

    private void Awake()
    {
        anim = GetComponent<Animator>();
        //enemyPatrol = GetComponentInParent<EnemyPatrol>(); // ajustar aqui
        
        if (enemyPatrol == null)
        {
            GameObject patrolObj = GameObject.FindGameObjectWithTag("EnemyPatrol");
            if (patrolObj != null)
                enemyPatrol = patrolObj.GetComponent<EnemyPatrol>();
        }
        
        // Configura a referência no componente Health, se existir
        Health myHealth = GetComponent<Health>();
        if (myHealth != null && enemyPatrol != null)
        {
            myHealth.SetEnemyPatrol(enemyPatrol);
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
            Health detectedHealth = hit.transform.GetComponent<Health>();

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
}