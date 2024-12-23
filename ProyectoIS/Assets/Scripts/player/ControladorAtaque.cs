using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Security.Cryptography.X509Certificates;

public class ControladorDeAtaque : Atributos
{
    PS4 controls;
    public LayerMask capaEnemigos;
    public GameObject projectilePrefab;
    public Transform LaunchOffset;
    public float magicCooldown = 0.1f;
    private float lastMagicTime;

    public PolygonCollider2D areaAtaque;
    public Sprite projectileSprite;
    private Vector2 direccionMovimiento;
    public Animator animator;
    private MusicManagement musicManagement;
    public Rigidbody2D rb;
    public Slider sliderVidas;
    public Slider sliderManá;

    protected Flash flash;


    private Color fullHealthColor = Color.green;
    private Color midHealthColor = Color.yellow;
    private Color lowHealthColor = Color.red;
    private bool critical;
    private bool attackMod= false;




    private void Awake()
    {
        controls = new PS4();
        flash = GetComponent<Flash>();

        currentHealth = health;
        currentManá = maná;
        sliderVidas = (Slider)GameObject.FindObjectsOfType(typeof(Slider))[1];
        sliderManá = (Slider)GameObject.FindObjectsOfType(typeof(Slider))[0];
        musicManagement = FindObjectOfType<MusicManagement>();
    }


    void Start()
    {
        controls.Gameplay.Attack.Enable();
        controls.Gameplay.Magic.Enable();
        controls.Gameplay.Dash.Enable();
        areaAtaque.isTrigger = true;
        if (sliderVidas != null)
        {
            sliderVidas.maxValue = health; 
            sliderVidas.value = currentHealth;
            UpdateHealthColor();
        }
        if (sliderManá != null)
        {
            sliderManá.maxValue = maná; 
            sliderManá.value = currentManá;
            StartCoroutine(RegenerateMana());
        }
        
        currentAttack = attack;
    }

    void Update()
    {
        direccionMovimiento = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;

        if ((Input.GetKeyDown(KeyCode.Space) || controls.Gameplay.Attack.triggered) && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 1") == false && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 2") == false && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 3") == false)
        {

            Attack();

        }

        if ((Input.GetKeyDown(KeyCode.Q) || controls.Gameplay.Magic.triggered) && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 1") == false && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 2") == false && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 3") == false && currentManá >= 10 && Time.time > lastMagicTime + magicCooldown)
        {

            Magic();


        }

        ActualizarPuntoAtaque();
    }

    void Magic()
    {
        lastMagicTime = Time.time;
        rb.velocity = Vector2.zero;
        animator.SetTrigger("Magic");
        musicManagement.SeleccionAudio(animator.GetInteger("NumbAtt") - 1, 1f);

        GameObject projectileObject = Instantiate(projectilePrefab, LaunchOffset.position, areaAtaque.transform.rotation);

        SpriteRenderer spriteRenderer = projectileObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = projectileSprite;

        // Get the Collider2D component of the projectile
        CircleCollider2D projectileCollider = projectileObject.GetComponent<CircleCollider2D>();

        // Get the Collider2D component of the player
        BoxCollider2D playerCollider = GetComponent<BoxCollider2D>();

        // Make the projectile ignore the player collider
        Physics2D.IgnoreCollision(projectileCollider, playerCollider);


        Projectile projectileScript = projectileObject.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            int damage = CriticalDamage(currentAttack);
            int trueDamage = damage + Random.Range(-3, 4);
            projectileScript.damage = trueDamage;
        }
        currentManá = currentManá - 10;
        sliderManá.value = currentManá;

    }

    void Attack()
    {
        rb.velocity = Vector2.zero;
        animator.SetTrigger("Attack");
        //musicManagement.SeleccionAudio(animator.GetInteger("NumbAtt")-1, 1f);
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(capaEnemigos);

        List<Collider2D> resultados = new List<Collider2D>();
        areaAtaque.OverlapCollider(filter, resultados);
        foreach (Collider2D enemigo in resultados)
        {
            Enemigo enemigoComponent = enemigo.GetComponent<Enemigo>();
            if (enemigoComponent != null)
            {
                int damage = CriticalDamage(currentAttack);
                int trueDamage = damage + Random.Range(-3, 4);
                enemigoComponent.GetDamaged(trueDamage, critical);
                musicManagement.SeleccionAudio(5, 1f);
            }
        }
        int count = animator.GetInteger("NumbAtt") + 1;
        if (count == 4)
        {
            count = 1;
        }
        animator.SetInteger("NumbAtt", count);

    }
    int CriticalDamage(int attack)
    {
        if (Random.value < critChance)
        {
            int criticalHit = Mathf.RoundToInt(attack * critAttack);
            critical = true;
            return criticalHit;
        }
        else
        {
            critical = false;
            return attack;
        }
    }
    void ActualizarPuntoAtaque()
    {
        if (direccionMovimiento != Vector2.zero)
        {
            Vector3 nuevaPosicion = transform.position + (Vector3)direccionMovimiento * 0.5f;
            areaAtaque.transform.position = nuevaPosicion;

            float angle = Mathf.Atan2(direccionMovimiento.y, direccionMovimiento.x) * Mathf.Rad2Deg;
            areaAtaque.transform.rotation = Quaternion.Euler(0, 0, angle + 90);
        }
    }

    public void GetDamaged(int damage)
    {
        currentHealth -= damage;
        StartCoroutine(flash.FlashRoutine());
        CineMachineMovCmera.Instance.MoverCamara(5, 5, 0.5f);

        sliderVidas.value = currentHealth;
        UpdateHealthColor();

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
            SceneManager.LoadScene(3);
        }
    }

    private void UpdateHealthColor()
    {
        if (currentHealth > health / 2)
        {
            sliderVidas.fillRect.GetComponent<Image>().color = fullHealthColor;
        }
        else if (currentHealth > health / 4)
        {
            sliderVidas.fillRect.GetComponent<Image>().color = midHealthColor;
        }
        else
        {
            sliderVidas.fillRect.GetComponent<Image>().color = lowHealthColor;
        }
    }
    public void AddHealth(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, health);
        sliderVidas.value = currentHealth;
        UpdateHealthColor();
    }

    public void addDamage()
    {
        if(!attackMod){Debug.Log("fuerza");
        currentAttack = currentAttack * 2;
        attackMod= true;
        Debug.Log(currentAttack);
        StartCoroutine(VolverNormal());}

    }



    public IEnumerator VolverNormal()

    {
        Debug.Log("entro");
        yield return new WaitForSeconds(10f);
        currentAttack = currentAttack / 2;
        Debug.Log("salio");
        attackMod= true;

    }



    public void AddManá(int amount)
    {
        currentManá += amount;
        currentManá = Mathf.Clamp(currentManá, 0, maná);
        sliderManá.value = currentManá;
    }
    private IEnumerator RegenerateMana()
    {
        while (true)
        {
            if (currentManá < maná)
            {
                currentManá += 1; // Ajusta la cantidad de regeneración según sea necesario
                sliderManá.value = currentManá;
                yield return new WaitForSeconds(1f); 
            }
            else
            {
                yield return null; // Espera al siguiente frame si el maná está lleno
            }
        }
    }

}
