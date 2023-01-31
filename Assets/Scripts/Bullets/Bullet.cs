using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed;
    public float damage;
    public Player shooter;

    [SerializeField] int bulletType;
    public bool synth = false;
    [SerializeField] float synthSizeFactor = 2f;
    public bool laser = false;
    private Vector2 maxLaserLength;
    private Vector2 laserLength;
    public LayerMask laserStop;
    [SerializeField] float laserDestroyDelay = 0.2f;
    public LineRenderer[] laserLr;
    // Start is called before the first frame update
    void Start()
    {
        if (laser)
        {
            laserLength = transform.position;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right,Mathf.Infinity, laserStop);
            foreach (LineRenderer i in laserLr)
            {
                i.positionCount = 2;
                i.SetPosition(0, transform.position);
                i.SetPosition(1, transform.position);
            }
            maxLaserLength = hit.point;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (laser)
        {
            if (laserLength != maxLaserLength)
            {
                laserLength = new Vector2(Mathf.MoveTowards(laserLength.x, maxLaserLength.x, (laserLength.x+ bulletSpeed) * Time.deltaTime), Mathf.MoveTowards(laserLength.y, maxLaserLength.y, (laserLength.y + bulletSpeed) * Time.deltaTime));
                foreach (LineRenderer i in laserLr)
                {
                    i.SetPosition(1, laserLength);
                }
                

                var col = GetComponent<CapsuleCollider2D>();
                col.offset = new Vector2(laserLr[0].GetPosition(1).x / 2, 0);
                col.size = new Vector2(laserLr[0].GetPosition(1).x, laserLr[0].startWidth);
            }
            else
            {
                StartCoroutine(DelayedDestroy());
            }
            
        }
        else
        {
            transform.position += transform.right * bulletSpeed * Time.deltaTime;
        }

        //Destroys bullet after some time
        if (synthSizeFactor > 1 && synth)
        {
            synthSizeFactor += Time.deltaTime;
            transform.localScale += new Vector3(synthSizeFactor * Time.deltaTime, synthSizeFactor * Time.deltaTime, synthSizeFactor * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PlayerBody")
        {
            if (collision.GetComponentInParent<Player>() == shooter)
            {
                if (!laser && !synth)
                {
                    Destroy(gameObject);
                }
                return;
            }
            collision.GetComponentInParent<Player>().health -= damage;
            //SFX
            SoundManager.instance.PlayOneShot(FMODEvents.instance.hit, transform.position);

            
            if (collision.GetComponentInParent<Player>().health - damage <= 0)
            {
                shooter.finalBlows += 1;
            }
            shooter.damageDealt += damage;
            if (!laser && !synth)
            {
                Destroy(gameObject);
            }
        }

        if (collision.tag == "Surface")
        {
            //Debug.Log("Wall");
            if (!laser)
            {
                Destroy(gameObject);
            }
        }

        if (collision.tag == "SOLO")
        {
            collision.GetComponent<Solo>().Damage(damage, bulletType);
            if (!laser && !synth) { Destroy(gameObject); }
        }
    }

    private IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(laserDestroyDelay);
        Destroy(gameObject);
    }
}
