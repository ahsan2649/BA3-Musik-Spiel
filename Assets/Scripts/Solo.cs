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
    
    //Cached Components
    Rigidbody2D rb;
    SpriteRenderer sr;

    //State
    public int currentHealth;
    int currentMaxDmgPlayer;
    int currentMovePoint;
    List<int> damageList = new List<int>();
    Vector2 startposition;
    float timer;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        currentHealth = maxHealth;
        startposition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Follow Path
        timer += moveSpeed * Time.deltaTime;
        Vector2 newPosition = Vector2.MoveTowards(startposition, movePoints[currentMovePoint].position, timer);
        rb.MovePosition(newPosition);
        if(transform.position == movePoints[currentMovePoint].position)
        {
            if(currentMovePoint != movePoints.Count - 1)
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }


    public void Damage(int damage, int playerNumber)
    {
        
        damageList[playerNumber] += damage;
        //If first time hitting orb
        if(currentHealth == 0)
        {
            currentMaxDmgPlayer = playerNumber;
        }
        //Check which is the most damage dealing player
        else if (damageList[playerNumber] >= damageList[currentMaxDmgPlayer])
        {
            currentMaxDmgPlayer = playerNumber;
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
        sr.color = colorList[currentMaxDmgPlayer];
        //Add bar for damage display
    }

    void Destroy()
    {
        //Activate the S.O.L.O Tracks
    }
}
