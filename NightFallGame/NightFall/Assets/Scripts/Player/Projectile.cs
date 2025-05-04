using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
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
        //Debug.Log("Colisão detectada com: " + collision.gameObject.name);
        hit = true;
        anim.SetTrigger("explode");
        boxCollider.enabled = false;
        
        // if (collision.tag == "Enemy")
            //collision.GetComponent<Health>().TakeDamage(1);
        
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
        //Debug.Log("Entrou no Deactivate ");
        gameObject.SetActive(false);
    }
}