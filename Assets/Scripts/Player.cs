using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private Rigidbody2D body2D;
    private float playerMove;
    public float moveSpeed;
    private float jumpForce;
    public float jumpForceSet;
    public float fallMultiplier;
    public float minimumJumpMultiplier;
    private bool isJumping = false;
    private bool requestJump = false;
    public Transform attackAreaStart;
    public Transform attackAreaEnd;
    public float attackRate;
    private float attackRange;
    public float attackRangeSet;
    public LayerMask enemiesLayer;
    private float nextAttack;
    public float attackDuration;
    private float attackTime;
    public Transform holdingArea;
    private Transform oldingEnemy;
    public float throwForce;
    public byte hp;
    public byte maxHp;
    private float invulnerabilityTime;
    public float invulnerabilityDuration;
    public GameObject attackSprite;
    public float shineDurationPerShine;
    private float nextShine;
    private SpriteRenderer playerSprite;
    public GameObject heartObject;
    private List<GameObject> heartsObjects = new List<GameObject>();
    public Transform heartSpawn;
    public float heartDist;
    private bool jumpForceUP = false;
    public float jumpForceBonus;
    public float jumpForceDuration;
    public float gravityReductionBonus;
    public float gravityReductionDuration;
    private bool attackRangeUP = false;
    public float attackRangeBonus;
    public float attackRangeDuration;
    public GameObject playerAttackSFX;
    public GameObject playerDamageSFX;
    public GameObject playerDeathSFX;
    public GameObject lifeUpSFX;
    private bool isDead = false;

    public float heavyDebuffSpeed;
    public float heavyDebuffJump;

    void Start()
    {
        body2D = gameObject.GetComponent<Rigidbody2D>();
        playerSprite = gameObject.GetComponent<SpriteRenderer>();
        UpdatedHeart();
        AttackRangeUpdate();
        JumpForceUpdate();
    }

    void AttackRangeUpdate()
    {
        float attackBonus = attackRangeUP ? attackRangeBonus : 0;
        attackRange = attackRangeSet + attackBonus;
    }

    void JumpForceUpdate()
    {
        float jumpBonus = jumpForceUP ? jumpForceBonus : 0;
        jumpForce = jumpForceSet + jumpBonus;
    }

    void UpdatedHeart()
    {
        for(byte i = 0; i < heartsObjects.Count; i++)
        {
            Destroy(heartsObjects[i]);
        }
        heartsObjects.Clear();
        for (byte i = 0; i < hp; i++)
        {
            Vector3 heartPos = new Vector3(heartSpawn.position.x + i * heartDist, heartSpawn.position.y, heartSpawn.position.z);
            GameObject heart = Instantiate(heartObject, heartPos, Quaternion.identity);
            heartsObjects.Add(heart);
        }
    }

    void Update()
    {
        if (!isDead)
        {
            playerMove = Input.GetAxis("Horizontal");

            if (!isJumping && Input.GetKeyDown(KeyCode.W))
            {
                isJumping = true;
                requestJump = true;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (oldingEnemy == null)
                {
                    if (Time.time > nextAttack)
                    {
                        // Sound here
                        Vector2 playerAttackPosition = new Vector2(transform.position.x + 0.55f, transform.position.y - 0.1f);
                        Instantiate(playerAttackSFX, playerAttackPosition, transform.rotation);
                        nextAttack = Time.time + attackRate;
                        attackTime = Time.time + attackDuration;
                    }
                }
                else
                {
                    Vector2 throwDir = new Vector2(0, -throwForce);
                    TopEnemy enemy = oldingEnemy.GetComponent<TopEnemy>();
                    bool isHeavy = enemy.isHeavy;
                    enemy.Throw(throwDir);
                    oldingEnemy = null;
                    if (isHeavy)
                    {
                        moveSpeed += heavyDebuffSpeed;
                        jumpForce += heavyDebuffJump;
                    }

                }
       
            }
        } else
        {
            moveSpeed = 0f;
        }
    }

    private void FixedUpdate()
    {
        body2D.velocity = new Vector2(playerMove * moveSpeed * Time.deltaTime, body2D.velocity.y);

        if (Time.time < invulnerabilityTime)
        {
            if (Time.time > nextShine)
            {
                nextShine = Time.time + shineDurationPerShine;
                float alpha = playerSprite.color.a;
                alpha = alpha == 0 ? 255 : 0;
                playerSprite.color = new Color(255, 255, 255, alpha);
            }
        } else
        {
            playerSprite.color = new Color(255, 255, 255, 255);
        }

        if (playerMove != 0)
        {
            Vector2 scale = transform.localScale;
            bool changeSpriteDirection = scale.x > 0 != playerMove > 0;

            if (changeSpriteDirection)
            {
                scale.x = playerMove > 0 ? 1 : -1;
                transform.localScale = scale;
            }
        }

        if (oldingEnemy != null)
        {
            oldingEnemy.position = holdingArea.position;
        }

        if (requestJump)
        {
            requestJump = false;
            body2D.velocity = Vector2.up * jumpForce * Time.fixedDeltaTime;
            gameObject.GetComponent<Animator>().SetBool("is_jumping", true);
        }

        if (body2D.velocity.y < 0)
        {
            body2D.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (body2D.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            body2D.velocity += Vector2.up * Physics2D.gravity.y * (minimumJumpMultiplier - 1) * Time.fixedDeltaTime;
        }

        if (IsAttacking())
        {
            Collider2D[] hittedObjects = Physics2D.OverlapAreaAll(attackAreaStart.position, attackAreaEnd.position, enemiesLayer);
            foreach (Collider2D hitted in hittedObjects)
            {
                if (oldingEnemy == null)
                {
                    Enemy enemy = hitted.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.GetComponent<Enemy>().OnDamage(true);
                        TopEnemy topEnemy = enemy.gameObject.GetComponent<TopEnemy>();
                        if (topEnemy != null)
                        {
                            if (topEnemy.IsGrablable())
                            {
                                bool isHeavy = topEnemy.Grab();
                                if (isHeavy)
                                {
                                    moveSpeed -= heavyDebuffSpeed;
                                    jumpForce -= heavyDebuffJump;
                                }
                                oldingEnemy = topEnemy.gameObject.transform;
                                break;
                            }
                        }
                    }
                }
            }
        }
        bool changeSpriteAttackState = attackSprite.activeSelf != IsAttacking() ? true : false;
        if (changeSpriteAttackState)
        {
            attackSprite.SetActive(IsAttacking());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            isJumping = false;
            gameObject.GetComponent<Animator>().SetBool("is_jumping", false);
        }
        if (collision.gameObject.CompareTag("PowerUp"))
        {
            
        }
        if (collision.gameObject.CompareTag("EnemyProjectile"))
        {
            Destroy(collision.gameObject);
            onDamage();
        }
    }

    public void OnCollectPowerUp(PowerUpScript.PowerUps powerUps)
    {
        switch (powerUps)
        {
            case PowerUpScript.PowerUps.HighJump:
                jumpForceUP = true;
                JumpForceUpdate();
                Invoke("HighJumpOff", jumpForceDuration);
                break;
            case PowerUpScript.PowerUps.GravityReduction:
                body2D.gravityScale -= gravityReductionBonus;
                Invoke("GravityReductionOff", gravityReductionDuration);
                break;
            case PowerUpScript.PowerUps.BigAttackRange:
                attackRangeUP = true;
                AttackRangeUpdate();
                Invoke("BigAttackRange", attackRangeDuration);
                break;
            case PowerUpScript.PowerUps.LifeUp:
                if (hp < maxHp)
                {
                    if (!isDead)
                    {
                        hp += 1;
                        Instantiate(lifeUpSFX, gameObject.transform.position, gameObject.transform.rotation);
                        UpdatedHeart();
                    }
                }
                break;
        }
    }

    private void HighJumpOff()
    {
        jumpForceUP = false;
        JumpForceUpdate();
    }

    private void GravityReductionOff()
    {
        body2D.gravityScale += gravityReductionBonus;
    }

    private void BigAttackRange()
    {
        attackRangeUP = false;
        AttackRangeUpdate();
    }

    public void onDamage()
    {
        if (Time.time > invulnerabilityTime)
        {            
            invulnerabilityTime = Time.time + invulnerabilityDuration;
            // Sound here
            Vector2 playerDamagePosition = new Vector2(transform.position.x + 0.045f, transform.position.y + 0.13f);
            if (hp > 1)
            {
                Instantiate(playerDamageSFX, playerDamagePosition, transform.rotation);
                hp -= 1;
                UpdatedHeart();
            }
            else
            {
                Vector2 playerDeathPosition = new Vector2(transform.position.x + 0.045f, transform.position.y + 0.13f);
                Instantiate(playerDeathSFX, playerDeathPosition, transform.rotation);
                hp -= 1;
                UpdatedHeart();
                isDead = true;
                Invoke("GameOver", 2f);
            }
        }
    }

    void GameOver()
    {
        SceneManager.LoadScene(3);
    }

    private bool IsAttacking()
    {
        return Time.time < attackTime;
    }

    private void OnDrawGizmos()
    {
        if (attackAreaStart == null || attackAreaEnd == null)
            return;

        Gizmos.color = IsAttacking() ? Color.red : Color.white;

        Vector3 startPos = attackAreaStart.position;
        Vector3 endPos = attackAreaEnd.position;
        Gizmos.DrawLine(startPos, new Vector3(endPos.x, startPos.y, startPos.z));
        Gizmos.DrawLine(startPos, new Vector3(startPos.x, endPos.y, startPos.z));
        Gizmos.DrawLine(startPos, endPos);
        Gizmos.DrawLine(new Vector3(startPos.x, endPos.y, startPos.z), endPos);
        Gizmos.DrawLine(new Vector3(startPos.x, endPos.y, startPos.z), new Vector3(endPos.x, startPos.y, startPos.z));
        Gizmos.DrawLine(new Vector3(endPos.x, startPos.y, startPos.z), new Vector3(endPos.x, endPos.y, startPos.z));
    }

}
