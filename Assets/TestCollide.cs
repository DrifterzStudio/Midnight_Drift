using UnityEngine;

public class TestCollide : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnCollisionStay(Collision col)
    {
        Debug.Log("(sol) Touching: " + col.gameObject.name);
    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("TRIGGER: " + other.name);
    }
}
