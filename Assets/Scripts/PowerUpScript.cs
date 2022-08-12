using UnityEngine;

public class PowerUpScript : MonoBehaviour
{
    public enum PowerUps
    {
        HighJump,
        GravityReduction,
        BigAttackRange,
        LifeUp
    };

    public PowerUps powerUp;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            Destroy(gameObject);
        } else if (collision.gameObject.CompareTag("Player"))
        {
            collision.GetComponent<Player>().OnCollectPowerUp(powerUp);
            Destroy(gameObject);
        }
    }

}
