using UnityEngine;
using UnityEngine.SceneManagement;

public class BossScript : Enemy
{
    public byte knockDownResist;
    private byte currentKnockDownResist;
    public float fallSpeed;
    public bool fallOnHit;
    public GameObject bossDamageSFX;
    public GameObject bossFallSFX;
    private float shineDuration;
    private float nextShine;
    private float shineDurationPerShine = 0.075f;
    private float shineDurationPerHit = 1f;
    private SpriteRenderer bossSprite;
    public SpriteRenderer bossHeadSprite;

    void Start()
    {
        StartEnemy();
        currentKnockDownResist = knockDownResist;
        bossSprite = gameObject.GetComponent<SpriteRenderer>();

    }

    private void FixedUpdate()
    {
        OnAttack();
        OnMove();
        if (transform.position.y > -1)
        {
            //Special Behavior go to certain position and down/up
            body2D.velocity = new Vector2(body2D.velocity.x, -body2D.velocity.y);
        }

        if (Time.time < shineDuration)
        {
            if (Time.time > nextShine)
            {
                nextShine = Time.time + shineDurationPerShine;
                float alpha = bossSprite.color.a;
                alpha = alpha == 0 ? 255 : 0;
                bossSprite.color = new Color(255, 255, 255, alpha);
                bossHeadSprite.color = new Color(255, 255, 255, alpha); 
            }
        }
        else
        {
            bossSprite.color = new Color(255, 255, 255, 255);
            bossHeadSprite.color = new Color(255, 255, 255, 255);
        }
    }

    public override void OnDamage(bool playerAttack)
    {
        Debug.Log(hp);
        if (hp <= 1)
        {
            SceneManager.LoadScene(4);
        }

        Instantiate(bossDamageSFX, transform.position, transform.rotation);
        if (!fallOnHit)
        {
            base.OnDamage(playerAttack);
            shineDuration = Time.time + shineDurationPerHit;
            if (currentKnockDownResist > 1)
            {
                currentKnockDownResist--;
            }
            else
            {
                Instantiate(bossFallSFX, gameObject.transform.position, gameObject.transform.rotation);
                currentKnockDownResist = knockDownResist;
                body2D.velocity = new Vector2(body2D.velocity.x, -fallSpeed);
                fallOnHit = true;
            }
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Walls") && !waitToChange)
        {
            waitToChange = true;
            OnMoveChange();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Walls"))
        {
            waitToChange = false;
        }
    }

    private void OnBecameInvisible()
    {
        if (!waitUpgrade && fallOnHit)
        {
            waitUpgrade = true;
            OnUpgrade();
        }
        fallOnHit = false;
        body2D.velocity = new Vector2(body2D.velocity.x, moveSpeed);
    }

    private void OnBecameVisible()
    {
        waitUpgrade = false;
    }

}
