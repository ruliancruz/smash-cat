using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasePlayerBehavior : MoveBehavior
{
    protected override void DoMove(Vector2 playerPos, GameObject enemy, Rigidbody2D enemyBody2D)
    {
        float playerDistance = enemy.transform.position.x - playerPos.x;
        if (playerDistance > 0.2f || playerDistance < -0.2f)
        {
            float moveDir = (playerDistance > 0 ? -1 : 1) * moveSpeed;
            enemyBody2D.velocity = new Vector2(moveDir, enemyBody2D.velocity.y);
            enemy.GetComponent<SpriteRenderer>().flipX = moveDir < 0;
        }
    }

    public override void OnMoveUpgrade()
    {
        
    }

    public override void OnMoveChange()
    {

    }

}
