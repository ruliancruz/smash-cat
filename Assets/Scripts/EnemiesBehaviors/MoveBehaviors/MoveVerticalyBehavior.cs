using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveVerticalyBehavior : MoveBehavior
{
    protected override void DoMove(Vector2 playerPos, GameObject enemy, Rigidbody2D enemyBody2D)
    {
        enemyBody2D.velocity = new Vector2(moveSpeed, enemyBody2D.velocity.y);
    }

    public override void OnMoveUpgrade()
    {
        moveSpeed += moveSpeed > 0 ? moveSpeedGrown : -moveSpeedGrown;
        moveSpeed = moveSpeed > moveSpeedMax || moveSpeed < -moveSpeedMax ? 
            moveSpeed > 0 ? moveSpeedMax : -moveSpeedMax : moveSpeed;
    }

    public override void OnMoveChange()
    {
        moveSpeed *= -1;
    }

}
