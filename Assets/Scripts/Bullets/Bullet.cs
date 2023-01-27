using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed;
    public float damage;
    public Character shooter;

    public bool laser = false;
    public LayerMask laserStop;

    private LineRenderer lr;
    // Start is called before the first frame update
    void Start()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, laserStop);
        lr = GetComponentInChildren<LineRenderer>();

        lr.positionCount = 2;
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, hit.point);
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
