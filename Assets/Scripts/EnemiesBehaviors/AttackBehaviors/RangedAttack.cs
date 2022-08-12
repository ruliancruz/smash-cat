using System.Collections;
using UnityEngine;

public class RangedAttack : AttackBehavior
{
    [Header("Ranged Settings")]
    public GameObject projectile;
    public float shootForce;
    public float shootForceGrown;
    public float shootForceMax;
    public Transform projectileSpawnPoint;
    public bool shootInPlayerDirection;
    private Transform player;
    [Header("MultiShot Settings")]
    public bool isMultiShoot;
    private int startShootQuantity;
    private int gronwCount = 1;
    public int shootQuantity;
    public float shootQuantityGrown;
    public float delayBetweenShoots;
    public float delayBetweenShootsDegress;
    private int shootActual;
    public float waitAfterShoots;
    public float waitAfterShootsDegress;

    private void Start()
    {
        startShootQuantity = shootQuantity;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate()
    {
        float targetX = player.position.x - gameObject.transform.position.x;
        float targetY = player.position.y - gameObject.transform.position.y;
        float angle = Mathf.Atan2(targetY, targetX) * Mathf.Rad2Deg;
        projectileSpawnPoint.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
    }

    protected override bool DoAttack(Vector2 playerPos, Transform enemyPos)
    {
        if (isMultiShoot)
        {
            shootActual = shootQuantity;
            StartCoroutine(DoMultiShoot(playerPos, enemyPos));
        } else
        {
            DoShootAttack(playerPos, enemyPos);
        }
        return true;
    }

    private IEnumerator DoMultiShoot(Vector2 playerPos, Transform enemyPos)
    {
        DoShootAttack(playerPos, enemyPos);
        shootActual--;
        yield return new WaitForSeconds(delayBetweenShoots);
        Debug.Log("Shoot");
        if (shootActual > 0)
        {
            StartCoroutine(DoMultiShoot(player.position, enemyPos));
        } else
        {
            nextAttack = Time.time + attackDelay + waitAfterShoots;
        }
    }

    private void DoShootAttack(Vector2 playerPos, Transform enemyPos)
    {
        GameObject rock = Instantiate(projectile, projectileSpawnPoint.position, enemyPos.rotation);
        Physics2D.IgnoreCollision(rock.GetComponent<Collider2D>(), enemyPos.gameObject.GetComponent<Collider2D>());

        Rigidbody2D prb = rock.GetComponent<Rigidbody2D>();
        Vector2 enemyPos2D = new Vector2(enemyPos.position.x, enemyPos.position.y);
        Vector2 adjust = shootInPlayerDirection ? (playerPos - enemyPos2D).normalized : new Vector2(0, 1);

        float targetX = playerPos.x - enemyPos.position.x;
        float targetY = playerPos.y - enemyPos.position.y;
        float angle = Mathf.Atan2(targetY, targetX) * Mathf.Rad2Deg;
        rock.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
        prb.velocity = adjust * shootForce;
        nextAttack = Time.time + attackDelay;
        AnimAttack();
    }

    protected override void UpgradeSpecializedAttack()
    {
        gronwCount++;
        shootForce += shootForceGrown;
        shootForce = shootForce > shootForceMax ? shootForceMax : shootForce;
        shootQuantity = Mathf.FloorToInt(startShootQuantity * gronwCount * shootQuantityGrown);
        delayBetweenShoots -= delayBetweenShootsDegress;
        waitAfterShoots -= waitAfterShootsDegress;
    }
}
