using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{

    EdgeCollider2D ec;
    public Transform c;
    public GameObject test_sphere;
    Vector2 closest_point;

    // Start is called before the first frame update
    void Start()
    {
        ec = GetComponent<EdgeCollider2D>();
        Debug.Log(ec.edgeCount);
        Debug.Log(ec.pointCount);
        Debug.Log(ec.ClosestPoint(new Vector2(c.position.x,c.position.y)));
    }

    // Update is called once per frame
    void Update()
    {
        closest_point = ec.ClosestPoint(new Vector2(c.position.x, c.position.y));
        Instantiate(test_sphere, new Vector3(closest_point.x, closest_point.y, 0), Quaternion.identity);
    }
}
