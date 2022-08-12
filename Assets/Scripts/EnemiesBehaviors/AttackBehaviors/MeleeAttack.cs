using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : AttackBehavior
{
    //TODO OnDeath Behavior
    [Header("Melee Settings")]
    public float attackRange;
    public bool damageOnAttack;
    protected override bool DoAttack(Vector2 playerPos, Transform enemyPos)
    {
        if (Vector2.Distance(playerPos, enemyPos.position) < attackRange)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().onDamage();
            AnimAttack();
            if(damageOnAttack)
            {
                enemyPos.gameObject.GetComponent<Enemy>().OnDamage(false);
            }
            return true;
        }
        return false;
    }

    protected override void UpgradeSpecializedAttack()
    {
        
    }

}
