
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private int damage = 1; // Dano que o projétil causa
    private float direction;
    private bool hit;
    private float lifetime;

    private Animator anim;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }
    
    private void Update()
    {
        if (hit) return;
        float movementSpeed = speed * Time.deltaTime * direction;
        transform.Translate(movementSpeed, 0, 0);

        lifetime += Time.deltaTime;
        if (lifetime > 5) gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(hit) return;
        hit = true;
        anim.SetTrigger("explode");
        boxCollider.enabled = false;
        
        if (collision.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
            
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
            else
            {
                enemyHealth = collision.GetComponentInParent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damage);
                }
                else
                {
                    enemyHealth = collision.GetComponentInChildren<EnemyHealth>();
                    if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage(damage);
                    }
                }
            }
        }
        else if (collision.CompareTag("Boss"))
        {
            BossHealth bossHealth = collision.GetComponent<BossHealth>();
            
            if (bossHealth != null)
            {
                bossHealth.TakeDamage(damage);
            }
            else
            {
                bossHealth = collision.GetComponentInParent<BossHealth>();
                if (bossHealth != null)
                {
                    bossHealth.TakeDamage(damage);
                }
                else
                {
                    bossHealth = collision.GetComponentInChildren<BossHealth>();
                    if (bossHealth != null)
                    {
                        bossHealth.TakeDamage(damage);
                    }
                }
            }
        }
    }
    
    public void SetDirection(float _direction)
    {
        lifetime = 0;
        direction = _direction;
        hit = false;
        
        if (boxCollider != null)
            boxCollider.enabled = true;
        
        if (_direction > 0)
            transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
        else
            transform.localScale = new Vector3(-0.02f, 0.02f, 0.02f);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}