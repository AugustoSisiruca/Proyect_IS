using System.Collections;
using UnityEngine;

public class Estatua : Enemigo
{
    [SerializeField] private Sprite[] destructionSprites; // Array de sprites para la destrucción
    private SpriteRenderer spriteRenderer;

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateEstatuaSprite();
    }

    protected override void Update()
    {
        UpdateEstatuaSprite();
    }

    protected override void FixedUpdate(){
        
    }
    private void UpdateEstatuaSprite()
    {
        float healthPercentage = (float)vida / vidaMax;

        if (healthPercentage > 0.66f)
        {
            spriteRenderer.sprite = destructionSprites[0]; // Sprite de estado intacto
        }
        else if (healthPercentage > 0.33f)
        {
            spriteRenderer.sprite = destructionSprites[1]; // Sprite de estado medio
        }
        else
        {
            spriteRenderer.sprite = destructionSprites[2]; // Sprite de estado destruido
        }
    }

    protected override IEnumerator OnDieAnimationComplete()
    {
        yield return base.OnDieAnimationComplete();
    }


}