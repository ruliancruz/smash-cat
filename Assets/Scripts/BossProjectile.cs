using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public GameObject explosion;
    public GameObject bossShotSFX;
    public SpawnController spawncontroller;
    
    private void Start()
    {
        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), GameObject.FindGameObjectWithTag("Platform").gameObject.GetComponent<Collider2D>());
        Vector2 shotPosition = new Vector2(gameObject.GetComponent<Transform>().position.x, gameObject.GetComponent<Transform>().position.y - 0.2f);
        Instantiate(bossShotSFX, gameObject.GetComponent<Transform>().position, gameObject.GetComponent<Transform>().rotation);
    }
    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TopEnemy topEnemy = collision.gameObject.GetComponent<TopEnemy>();
            if (topEnemy != null)
            {
                topEnemy.OnCollideWithProjectile();
            } else
            {
                collision.gameObject.GetComponent<Enemy>().OnDamage(false);
            }
        }
        Instantiate(explosion, gameObject.GetComponent<Transform>().position, gameObject.GetComponent<Transform>().rotation);
        Destroy(gameObject);
    }
}
