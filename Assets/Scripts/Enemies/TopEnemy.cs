using UnityEngine;

public class TopEnemy : Enemy
{
    public float enemyArea;
    public LayerMask enemiesLayer;
    
    public Sprite grabedSpr;
    public Sprite walkingSpr;
    private SpriteRenderer spriteRenderer;
    public bool isHeavy;
    public int piercingProjectiles;

    void Start()
    {
        StartEnemy();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (!isGrabed)
        {
            if (isStuned && Time.time > stunTime)
            {
                isStuned = false;
                spriteRenderer.sprite = walkingSpr;
            }
        }

        if (!isGrabed && !isStuned)
        {
            OnAttack();
            OnMove();
        } else if (isTrowhed)
        {
            Collider2D[] hittedObjects = Physics2D.OverlapCircleAll(transform.position, enemyArea, enemiesLayer);
            foreach (Collider2D hitted in hittedObjects)
            {
                Enemy enemy = hitted.GetComponent<Enemy>();
                if (enemy != null)
                {
                    if (enemy.isHittableEnemy)
                    {
                        enemy.OnDamage(false);
                        Destroy(gameObject);
                    }
                }
            }
        }
    }

    public void OnCollideWithProjectile()
    {
        if ((!isHeavy && spriteRenderer.sprite != walkingSpr) || piercingProjectiles == 0)
        {
            Destroy(gameObject);
        } else
        {
            piercingProjectiles--;
        }
    }

    public bool IsGrablable()
    {
        return isStuned && !isGrabed;
    }

    public bool Grab()
    {
        isGrabed = true;
        body2D.gravityScale = 0;
        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), GameObject.FindGameObjectWithTag("Platform").GetComponent<Collider2D>());
        spriteRenderer.sprite = grabedSpr;
        animator.SetBool("horizontal", false);
        return isHeavy;
    }

    public void Throw(Vector2 throwDir)
    {
        isTrowhed = true;
        transform.position = player.position;
        body2D.velocity = throwDir;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            if (!isInPlayerArea)
            {
                OnEnterPlayerArea();
                spriteRenderer.sprite = walkingSpr;
            }
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, enemyArea);
    }

}
