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
        if (collision.tag == "PlayerBody")
        {
            var player = collision.GetComponentInParent<Player>();
            player.health += health;
            FindObjectOfType<PlayerManager>().HandleCrown();
            if (healthParticle != null)
            {
                Instantiate(healthParticle, player.transform.position, Quaternion.identity, player.gameObject.transform);
                SoundManager.instance.PlayOneShot(FMODEvents.instance.pickupSpawn, transform.position);
            }
            //SFX
            SoundManager.instance.PlayOneShot(FMODEvents.instance.healthPickup, transform.position);

            Destroy(gameObject);
        }
    }
}
