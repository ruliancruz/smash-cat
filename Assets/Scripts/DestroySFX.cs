using UnityEngine;

public class DestroySFX : MonoBehaviour
{
    public float SFXLenght;

    void Start()
    {
        Destroy(gameObject, SFXLenght);    
    }

}
