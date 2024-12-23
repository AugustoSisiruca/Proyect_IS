using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.SceneManagement;

public class Puerta : MonoBehaviour
{

    public int TargetScene;
    public Animator anim;
    PS4 controls;
    public TMP_Text texto;
    [SerializeField]
    private bool Chocando=false;
    private playerMovement player;

    // Start is called before the first frame update

    // Update is called once per frame

    private void Awake()
    {
        controls= new PS4();
    }

    void Start(){
        controls.Gameplay.Attack.Enable();
    }

    void Update()
    {
        IrAmundo();
        anim.SetBool("Opening", Chocando);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if(collision.gameObject.CompareTag("Player"))
        {
            
            texto.gameObject.SetActive(true);

            Chocando = true;
            playerMovement playercomp = collision.gameObject.GetComponent<playerMovement>();
            player = playercomp;
            
            
        }
       
    }


    private void OnCollisionExit2D(Collision2D collision)
    {   
        texto.gameObject.SetActive(false);
        Chocando = false;
    }


    private void IrAmundo()
    {
        if (Chocando==true)
        {
            if (Input.GetKeyDown(KeyCode.Space) || controls.Gameplay.Attack.triggered)
            {
                texto.gameObject.SetActive(false);
                int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
                player.ChangeScene(TargetScene, currentSceneIndex);
                SceneManager.LoadScene(TargetScene);
                DataJuego.data.dinero += 100;
                Debug.Log(DataJuego.data.dinero);

            }
        }  
    }
    
   
}
