using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;  
    void Start()
    {
        Destroy(gameObject, 2);
    }

    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
