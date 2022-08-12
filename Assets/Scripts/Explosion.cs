using UnityEngine;

public class Explosion : MonoBehaviour
{
    public GameObject explosionSFX;

    void Start()
    {
        {
            Instantiate(explosionSFX, transform.position, transform.rotation);
            Destroy(gameObject, 0.4f);
        }
    }
}
