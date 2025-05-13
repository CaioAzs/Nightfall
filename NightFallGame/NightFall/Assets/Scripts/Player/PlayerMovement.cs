using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{

    Rigidbody2D rbPlayer;
    [SerializeField] float speed = 5f;
    
    [SerializeField] float jumpForce = 15f;
    [SerializeField] bool isJump;
    [SerializeField] bool inFloor = true;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;

    Animator animPlayer;

    //[SerializeField] bool dead = false;
    CapsuleCollider2D playerCollider;

    private void Awake(){
        animPlayer = GetComponent<Animator>();
        rbPlayer = GetComponent<Rigidbody2D>();
        //playerCollider = GetComponent<CapsuleCollider2D>();
    }

    private void Start(){
        //dead = false;
    }

    private void Update(){
        //if(dead) return;
        if (PainelDocumentoController.documentoAberto)
            return;

        inFloor = Physics2D.Linecast(transform.position, groundCheck.position, groundLayer);
        Debug.DrawLine(transform.position, groundCheck.position, Color.blue);

        animPlayer.SetBool("jump", !inFloor);

        if(Input.GetButtonDown("Jump") && inFloor){
            isJump = true;
        } else if(Input.GetButtonUp("Jump") && rbPlayer.linearVelocity.y > 0){
            rbPlayer.linearVelocity = new Vector2(rbPlayer.linearVelocity.x, rbPlayer.linearVelocity.y * 0.5f);
        }

    }

    private void FixedUpdate(){
        Move();
        JumpPlayer();
    }
    void Move(){
        //if(dead) return;

        float xMove = Input.GetAxis("Horizontal");
        rbPlayer.linearVelocity = new Vector2(xMove*speed, rbPlayer.linearVelocity.y);

        animPlayer.SetFloat("speed", Mathf.Abs(xMove));

        if(xMove > 0){
            transform.eulerAngles = new Vector2(0,0);
        } else if(xMove < 0){
            transform.eulerAngles = new Vector2(0,180);
        }
    }

    void JumpPlayer(){
        //if(dead) return;

        if(isJump){
            rbPlayer.linearVelocity = Vector2.up * jumpForce;
            isJump = false;
        }
    }

    public bool canAttack(){
        float xMove = Input.GetAxis("Horizontal");
        return inFloor && xMove == 0;
    }

    /*public void Death(){
        StartCoroutine(DeathCorotine());
    }

    IEnumerator DeathCorotine(){
        if(!dead){
            dead = true;
            animPlayer.SetTrigger("Death");
            yield return new WaitForSeconds(0.5f);
            rbPlayer.linearVelocity = Vector2.zero;
            rbPlayer.AddForce(Vector2.up * 15f, ForceMode2D.Impulse);
            playerCollider.enabled = false;
            Invoke("RestartGame", 2.5f);
        }
    }

    void RestartGame(){
        GameManager.Instance.ResetScores();
        SceneManager.LoadScene(2);
    }*/
}
