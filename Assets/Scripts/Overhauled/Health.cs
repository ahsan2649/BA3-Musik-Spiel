using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] float health;
    [SerializeField] GameObject healthParticle;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            var player = collision.GetComponentInParent<Player>();
            player.health += health;
            if (healthParticle != null)
            {
                Instantiate(healthParticle, player.body.transform.position + player.body.transform.up * 2f, Quaternion.identity);

            }
            Destroy(gameObject);
        }
    }
}
