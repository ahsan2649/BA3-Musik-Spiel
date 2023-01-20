using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class text_particle : MonoBehaviour
{

    float timer = 0;
    float max_time = 1;
    Animation ani;
    
    // Start is called before the first frame update
    void Start()
    {
        ani = GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!ani.isPlaying)
        {
            Destroy(gameObject);
        }
    }

}
