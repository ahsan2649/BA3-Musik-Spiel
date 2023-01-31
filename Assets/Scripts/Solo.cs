using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
public class Solo : MonoBehaviour
{
    //Serializable Values
    [SerializeField] int maxHealth = 500;
    [SerializeField] List<Color> colorList = new List<Color>();
    [SerializeField] List<Transform> movePoints = new List<Transform>();
    [SerializeField] float moveSpeed;
    [SerializeField] GameObject innerCircle;
    [SerializeField] float maxSize;

    [SerializeField] Material mat;

    //Cached Components
    Rigidbody2D rb;
    SpriteRenderer sr;

    //State
    public float currentHealth;
    public bool activated = true;
    int currentMaxDmgPlayer;
    Player maxDmgPlayer;
    int currentMovePoint;
    [SerializeField] List<float> damageList;
    Vector2 startposition;
    float timer;
    public bool moves;
    private float minSize;

    private EventInstance soloDestroyed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = innerCircle.GetComponent<SpriteRenderer>();

        currentHealth = maxHealth;
        startposition = transform.position;
        minSize = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (moves)
        {
            //Follow Path
            timer += moveSpeed * Time.deltaTime;
            Vector2 newPosition = Vector2.MoveTowards(startposition, movePoints[currentMovePoint].position, timer);
            rb.MovePosition(newPosition);
            if (transform.position == movePoints[currentMovePoint].position)
            {
                if (currentMovePoint != movePoints.Count - 1)
                {
                    currentMovePoint = currentMovePoint + 1;
                    startposition = transform.position;
                    Debug.Log("Next MovePoint");
                    timer = 0;
                }
                else
                {
                    currentMovePoint = 0;
                    startposition = transform.position;
                    Debug.Log("Reset MovePoint");
                    timer = 0;
                }
            }
        }
        UpdateVisuals();
        
    }

    public void Damage(float damage,int bulletType, Player player)
    {
        
        damageList[bulletType] += damage;
        //If first time hitting orb
        if(currentHealth == maxHealth)
        {
            currentMaxDmgPlayer = bulletType;
        }
        //Check which is the most damage dealing player
        else if (damageList[bulletType] >= damageList[currentMaxDmgPlayer])
        {
            currentMaxDmgPlayer = bulletType;
            maxDmgPlayer = player;
        }
        currentHealth -= damage;

        if(currentHealth <= 0)
        {
            Destroy();
        }


        UpdateVisuals();
    }


    void UpdateVisuals()
    {

        Debug.Log("Update Visuals SOLO");
        //Size up 
        float sizeMultiplier = maxHealth - currentHealth;
        sizeMultiplier = sizeMultiplier / maxHealth;
        float newSize = Mathf.Lerp(minSize, maxSize, sizeMultiplier);
        innerCircle.transform.localScale = new Vector3(newSize, newSize, newSize);
        mat.SetColor("_Color", colorList[currentMaxDmgPlayer]* (1 + (sizeMultiplier * 2)));
    }



    void Destroy()
    {
        GetComponent<CircleCollider2D>().enabled = false;

        //Sound Destroyed (needs to be canceled once SOLO starts)
        soloDestroyed = SoundManager.instance.CreateEventInstance(FMODEvents.instance.soloDestroyed);
        soloDestroyed.start();

        FindObjectOfType<SongManager>().SetSolo(this);
        //Visuals still missing
    }

    public void StopSoloSound()
    {
        soloDestroyed.stop(STOP_MODE.IMMEDIATE);
        //SOLO Start for max dmg player
        maxDmgPlayer.StartSolo();
    }
}
