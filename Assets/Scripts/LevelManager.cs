using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager levelManager;
    public enum Phase
    {
        Starting, Playing, Result
    };

    public Phase phase;

    private void Awake()
    {
        if(levelManager != null && levelManager != this)
        {
            Destroy(this);
        }
        else
        {
            levelManager = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
