using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MoveBehavior : MonoBehaviour
{
    [Header("Default Settings")]
    public float moveSpeed;
    public float moveSpeedGrown;
    public float moveSpeedMax;

    public void OnMove(Vector2 playerPos, GameObject enemy, Rigidbody2D enemyBody2D)
    {
        moveSpeedMax = moveSpeedMax == 0 ? moveSpeed : moveSpeedMax;
        DoMove(playerPos, enemy, enemyBody2D);
    }

    protected abstract void DoMove(Vector2 playerPos, GameObject enemy, Rigidbody2D enemyBody2D);

    public abstract void OnMoveUpgrade();

    public abstract void OnMoveChange();

}
