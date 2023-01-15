using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] FMODUnity.EventReference song;
    List<Character> characters= new List<Character>();
    List<Gun> guns= new List<Gun>();

    private void Awake()
    {
        characters = FindObjectsOfType<Character>().ToList();
        guns = FindObjectsOfType<Gun>().ToList();
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
