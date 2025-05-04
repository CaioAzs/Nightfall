using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
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

    private bool invulnerable;

    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(float _damage)
    {
        if (invulnerable) return;
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            anim.SetTrigger("hurt");
            StartCoroutine(Invunerability());
        }
        else
        {
            if (!dead)
            {
                anim.SetTrigger("die");
                GetComponent<PlayerMovement>().enabled = false;
                dead = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Enemy"){
            TakeDamage(1);
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
}
