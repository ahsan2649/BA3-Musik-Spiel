using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed;
    public float damage;
    public Character shooter;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.right * bulletSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Character")
        {
            collision.GetComponentInParent<Character>().health -= damage;
            Destroy(gameObject);
        }

        if (collision.tag == "Wall")
        {
            //Debug.Log("Wall");
            Destroy(gameObject);
        }
    }
}
