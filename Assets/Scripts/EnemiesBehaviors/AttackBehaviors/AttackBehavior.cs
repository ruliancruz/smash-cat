using UnityEngine;

public abstract class AttackBehavior : MonoBehaviour
{
    [Header("Default Settings")]
    protected float nextAttack;
    public float attackDelay;
    public float attackDelayDegress;
    public float attackDelayMin;
    public bool deathOnAttack;
    public Animator animator;

    public bool OnAttack(Vector2 playerPos, GameObject enemy)
    {
        bool isAttacking = false;
        if (Time.time > nextAttack)
        {
            nextAttack = Time.time + attackDelay;
            isAttacking = DoAttack(playerPos, enemy.transform);
            if (deathOnAttack)
            {
                Destroy(enemy);
            }
        }
        return isAttacking;
    }
    protected abstract bool DoAttack(Vector2 playerPos, Transform enemyPos);

    public void AnimAttack()
    {
        if (animator != null)
        {
            animator.Play("boss_spider_attack", -1, 0);
        }
    }

    public void OnAttackUpgrade()
    {
        UpgradeSpecializedAttack();
        attackDelay -= attackDelayDegress;
        attackDelay = attackDelay < attackDelayMin ? attackDelayMin : attackDelay;
    }
    protected abstract void UpgradeSpecializedAttack();

}
