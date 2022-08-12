using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Default Settings")]
    protected Transform player;
    protected Rigidbody2D body2D;
    public byte hp;
    public float gravity;
    protected bool isInPlayerArea = false;
    public bool isHittableEnemy;
    protected bool waitUpgrade = false;
    protected Animator animator;

    [Header("Attack Settings")]
    //Attaques que os inimigos terão
    public AttackBehavior attacksBehavior;
    public bool attackOutisdePlayerArea;

    [Header("Move Settings")]
    public MoveBehavior moveBehavior;
    public float moveSpeed;
    public float moveSpeedGrown;
    public float moveSpeedMax;
    public bool moveOutsidePlayerArea;
    protected bool waitToChange = false;

    public enum MoveVerticalyDir
    {
        Climb,
        Fall
    };
    public MoveVerticalyDir moveDirection = MoveVerticalyDir.Climb;
    
    [Header("On Death Settings")]
    public bool explodeOnDeath;
    public GameObject explosion;

    //Special behaviors, grabable
    public bool isGrabableEnemy;
    protected bool isStuned = false;
    protected float stunTime;
    protected bool isGrabed = false;
    public float stunDuration;
    protected bool isTrowhed = false;
    public GameObject enemyDamageSFX;
    public GameObject enemyDeathSFX;

    protected void StartEnemy()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        body2D = gameObject.GetComponent<Rigidbody2D>();
        body2D.velocity = new Vector2(0, moveDirection == MoveVerticalyDir.Climb ? moveSpeed : -moveSpeed);
        Physics2D.IgnoreCollision(player.gameObject.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>());
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Physics2D.IgnoreCollision(enemy.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>());
        }
        moveSpeedMax = moveSpeedMax == 0 ? moveSpeed : moveSpeedMax;
        animator = GetComponent<Animator>();
    }

    public virtual void OnDamage(bool playerAttack)
    {
        if (isGrabableEnemy)
        {
            if (!isGrabed)
            {
                Instantiate(enemyDamageSFX, transform.position, transform.rotation);
                isStuned = true;
                stunTime = Time.time + stunDuration;
                body2D.velocity = Vector2.zero;
                body2D.gravityScale = gravity;
                Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), GameObject.FindGameObjectWithTag("Platform").GetComponent<Collider2D>(), false);
            }
        } else
        {
            if (hp > 1)
            {
                hp -= 1;
            }
            else
            {
                Instantiate(enemyDeathSFX, transform.position, transform.rotation);
                if (explodeOnDeath)
                {
                    if (playerAttack)
                    {
                        attackOutisdePlayerArea = true;
                        OnAttack();
                    }
                    Instantiate(explosion, gameObject.transform.position, Quaternion.identity);
                }
                Destroy(gameObject);
            }
        }
    }

    protected bool OnAttack()
    {
        if (isInPlayerArea || attackOutisdePlayerArea)
        {
            return attacksBehavior.OnAttack(player.position, gameObject);
        }
        return false;
    }

    public void OnMove()
    {
        if (isInPlayerArea || moveOutsidePlayerArea)
        {
            if (moveBehavior != null)
            {
                moveBehavior.OnMove(player.position, gameObject, body2D);
            }
        }
    }

    protected void OnMoveChange()
    {
        if (moveBehavior != null)
        {
            moveBehavior.OnMoveChange();
        }
    }

    protected void OnUpgrade()
    {
        if (moveBehavior != null)
        {
            moveBehavior.OnMoveUpgrade();
        }
        waitUpgrade = true;
        moveSpeed += moveSpeedGrown;
        moveSpeed = moveSpeed > moveSpeedMax ? moveSpeedMax : moveSpeed;
        attacksBehavior.OnAttackUpgrade();
    }

    protected void OnEnterPlayerArea()
    {
        isInPlayerArea = true;
        body2D.gravityScale = gravity;
        animator.SetBool("horizontal", true);
    }
}
