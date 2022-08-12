using UnityEngine;

public class DownEnemy : Enemy
{
    void Start()
    {
        StartEnemy();
    }

    private void FixedUpdate()
    {
        OnAttack();
        OnMove();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Walls"))
        {
            waitToChange = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            OnEnterPlayerArea();
        }
        else if(collision.gameObject.CompareTag("Walls") && !waitToChange)
        {
            waitToChange = true;
            OnMoveChange();
        }
    }
}
