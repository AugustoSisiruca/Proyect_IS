using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemigo : MonoBehaviour
{
    public int vidaMax;
    protected int vida;
    public int maxAttack;
    public int attack;
    public int defensaMax;
    protected int defensa;
    protected int netDamage;
    public bool gettingKnockedBack { get; private set; }
    [SerializeField] private float knockBackTime = .2f;
    private Rigidbody2D rb;
    private MusicManagement musicManagement;
    [SerializeField] protected float speed;
    [SerializeField] protected LayerMask layerMask;
    protected GameObject player;
    public GameObject Character;
    protected bool hasLineOfSight = false;
    private bool isFacingRight = true; // Assume the enemy is facing right initially
    public Animator animator;
    protected Flash flash;
    public delegate void EnemyKilledHandler(Enemigo enemy);
    public event EnemyKilledHandler OnEnemyKilled;




    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        musicManagement = FindObjectOfType<MusicManagement>();
        flash = GetComponent<Flash>();
    }

    protected virtual void Start(){
        
        vida = vidaMax;
        attack = maxAttack;
        defensa = defensaMax;
        player = GameObject.FindGameObjectWithTag("Player");
    }
    protected virtual void Update()
        {
            //LÓGICA PARA QUE EL ENEMIGO SIEMPRE MIRE AL PERSONAJE PRINCIPAL
            if (player.transform.position.x < transform.position.x && isFacingRight)
        {
            Flip();
        }
            else if (player.transform.position.x > transform.position.x &&!isFacingRight)
        {
            Flip();
        }
        if (hasLineOfSight && !animator.GetBool("Death"))
            {
                transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
                Ataque();
            }
            else
            {
                animator.SetBool("Attack", false);
            }
        if(vida<=0){

            animator.SetBool("Death", true);
            StartCoroutine(OnDieAnimationComplete());
            // 
            // SceneManager.LoadScene(2);
        }
        }

    void Flip()
    {
        isFacingRight =!isFacingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }

    protected virtual void FixedUpdate()
    {
        RaycastHit2D ray = Physics2D.Raycast(transform.position, player.transform.position - transform.position, Mathf.Infinity, ~layerMask);
        if (ray.collider != null)
        {
            hasLineOfSight = ray.collider.CompareTag("Player");
            if (hasLineOfSight)
            {
                Debug.DrawRay(transform.position, player.transform.position - transform.position, Color.green);
            }
            else
            {
                if (!animator.GetBool("Death"))
                {
                    Debug.DrawRay(transform.position, player.transform.position - transform.position, Color.red);
                    // animator.SetBool("Attack", false);
                    
                }
            }
        }
        // if(vida<=0){

        //     animator.SetBool("Death", true);
        //     StartCoroutine(OnDieAnimationComplete());
        //     // musicManagement.SeleccionAudio(4, 1f);
        //     // SceneManager.LoadScene(2);
        // }
    }

    void Ataque(){
        animator.SetBool("Attack", true);
    }

    
    public virtual void GetDamaged(int damage){
        GetKnockedBackUwu(playerMovement.Instance.transform, 15f);
        musicManagement.SeleccionAudio(4, 1f);
        StartCoroutine(flash.FlashRoutine());
        
        

        netDamage = damage-defensa;
        if(netDamage>0){
            vida -= netDamage;
            animator.SetInteger("life", vida);
            animator.SetBool("Attack", false);
            Debug.Log(vida );
            }
    }

    protected virtual  IEnumerator OnDieAnimationComplete(){
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
         Die();
    }

    protected void Die()
    {
        if (OnEnemyKilled != null)
        {
            OnEnemyKilled(this);
        }
        Destroy(gameObject);
    }

    // void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         Debug.Log("hola");
    //         ControladorDeAtaque jugador = other.GetComponent<ControladorDeAtaque>();
    //         if (jugador != null)
    //         {
    //             jugador.GetDamaged(attack);
    //             Debug.Log("damge doned");
    //         }
    //     }
    // }


    public void GetKnockedBackUwu(Transform damageSource, float knockBackThrust)
   
    {
        
        gettingKnockedBack = true;
        
        Vector2 diference = (transform.position - damageSource.position).normalized * knockBackThrust * rb.mass;
        rb.AddForce(diference, ForceMode2D.Impulse);
        StartCoroutine(KnockRoutine());
    }

    private IEnumerator KnockRoutine()
    {
        yield return new WaitForSeconds(knockBackTime);
        rb.velocity = Vector2.zero;
        gettingKnockedBack = false;
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision) //Probando, para que Minotauro no herede onCollisionEnter2D()
    {
        ControladorDeAtaque jugador = collision.gameObject.GetComponent<ControladorDeAtaque>();
        if (jugador != null)
        {
            jugador.GetDamaged(attack);
        }
    }
}
