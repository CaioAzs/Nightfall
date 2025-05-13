using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;
    private Animator anim;
    private PlayerMovement playerMovement;
    private float cooldownTimer;

    private void Awake(){
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update(){
        if (PainelDocumentoController.documentoAberto)
            return;

        if(Input.GetMouseButton(0) && cooldownTimer > attackCooldown && playerMovement.canAttack())
            Attack();


        cooldownTimer += Time.deltaTime;
    }

    private void Attack()
    {
        anim.SetTrigger("attack");
        cooldownTimer = 0;

        if (projectilePrefab == null || firePoint == null)
            return;
            
        GameObject newProjectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Projectile projectileComponent = newProjectile.GetComponent<Projectile>();
        
        if (projectileComponent != null)
        {
            float dir = transform.eulerAngles.y == 0 ? 1 : -1;
            projectileComponent.SetDirection(dir);
        }
    }




}
