using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Solo : MonoBehaviour
{
    //Serializable Values
    [SerializeField] int maxHealth = 500;
    [SerializeField] List<Color> colorList = new List<Color>();
    [SerializeField] List<Transform> movePoints = new List<Transform>();
    [SerializeField] float moveSpeed;
    [SerializeField] GameObject innerCircle;
    [SerializeField] float maxSize;
    
    //Cached Components
    Rigidbody2D rb;
    SpriteRenderer sr;

    //State
    public int currentHealth;
    public bool activated = true;
    int currentMaxDmgPlayer;
    int currentMovePoint;
    List<float> damageList = new List<float>();
    Vector2 startposition;
    float timer;
    public bool moves;
    private float minSize;

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

    public void Damage(float damage,int bulletType)
    {
        
        damageList[bulletType] += damage;
        //If first time hitting orb
        if(currentHealth == 0)
        {
            currentMaxDmgPlayer = bulletType;
        }
        //Check which is the most damage dealing player
        else if (damageList[bulletType] >= damageList[currentMaxDmgPlayer])
        {
            currentMaxDmgPlayer = bulletType;
        }
        currentHealth -= bulletType;

        if(currentHealth <= 0)
        {
            Destroy();
        }


        UpdateVisuals();
    }


    void UpdateVisuals()
    {

        //Size up 
        float sizeMultiplier = maxHealth - currentHealth;
        sizeMultiplier = sizeMultiplier / maxHealth;
        float newSize = Mathf.Lerp(minSize, maxSize, sizeMultiplier);
        innerCircle.transform.localScale = new Vector3(newSize, newSize, newSize);
        sr.material.SetColor("Color", colorList[currentMaxDmgPlayer]* (sizeMultiplier * 2));
    }

    void Destroy()
    {
        
    }
}
