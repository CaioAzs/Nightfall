using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header ("Patrol Points")]
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;
    
    [Header("Enemy")]
    [SerializeField] private Transform enemy;
    
    [Header("Movement parameters")]
    [SerializeField] private float speed;
    private Vector3 initScale;
    private bool movingLeft;
    
    [Header("Idle Behaviour")]
    [SerializeField] private float idleDuration;
    private float idleTimer;
    
    [Header("Enemy Animator")]
    [SerializeField] private Animator anim;
    
    private void Awake()
    {
        initScale = enemy.localScale;
    }
    
    private void OnDisable()
    {
        // Verificar se o Animator ainda existe antes de usá-lo
        if (anim != null)
        {
            anim.SetBool("moving", false);
        }
    }
    
    private void Update()
    {
        if (movingLeft)
        {
            if (enemy.position.x >= leftEdge.position.x)
                MoveInDirection(-1);
            else
                DirectionChange();
        }
        else
        {
            if (enemy.position.x <= rightEdge.position.x)
                MoveInDirection(1);
            else
                DirectionChange();
        }
    }
    
    private void DirectionChange()
    {
        // Verificar se o Animator ainda existe
        if (anim != null)
        {
            anim.SetBool("moving", false);
        }
        
        idleTimer += Time.deltaTime;
        if(idleTimer > idleDuration)
        {
            movingLeft = !movingLeft;
            ApplyRotation();
        }
    }
    
    private void MoveInDirection(int _direction)
    {
        idleTimer = 0;
        
        // Verificar se o Animator ainda existe
        if (anim != null)
        {
            anim.SetBool("moving", true);
        }
        
        // Verificar se o transform do inimigo ainda existe
        if (enemy != null)
        {
            enemy.position = new Vector3(enemy.position.x + Time.deltaTime * _direction * speed,
                enemy.position.y, enemy.position.z);
        }
    }
    
    private void ApplyRotation()
    {
        // Verificar se o transform do inimigo ainda existe
        if (enemy != null)
        {
            if (movingLeft)
                enemy.rotation = Quaternion.Euler(0, 180, 0); // Virado para a esquerda
            else
                enemy.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}