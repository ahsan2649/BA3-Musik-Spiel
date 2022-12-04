using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector] public float speed;
    // Start is called before the first frame update
    private void Awake()
    {
        
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        transform.position += Vector3.right * Time.deltaTime * speed;
    }

    private void OnApplicationQuit()
    {
        Destroy(gameObject);
    }
}
