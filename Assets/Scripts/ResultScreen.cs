using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResultScreen : MonoBehaviour
{
    [SerializeField] List<Transform> playerPositions;
    [SerializeField] List<GameObject> soloShines;
    [SerializeField] List<TextMeshProUGUI> player1_texts;
    [SerializeField] List<TextMeshProUGUI> player2_texts;
    [SerializeField] List<TextMeshProUGUI> player3_texts;
    [SerializeField] List<TextMeshProUGUI> player4_texts;

    [SerializeField] Color noSoloShine;

    //PlayerPrefs

    List<string> rawPlayerStats;
    List<int> orderesPlayerList;

    //Player Stats
    List<int> player_solos;
    List<int> player_damage;
    List<int> player_finalblows;
    List<int> player_kicks;
    List<int> player_misses;
    List<int> player_pickups;
    List<int> player_avgSpeed;

    private void Start()
    {
        
    }

    void ParsingStats()
    {
        for (int i = 0; i < PlayerPrefs.GetInt("PlayerCount") - 1; i++)
        {

        }
    }

    public void SoloShine()
    {
        foreach (int i )
    }

    void UpdateStats()
    {
        if (PlayerPrefs.GetInt("PlayerCount") == 1)
        {
            if (player_solos[0] == 1)
            {
                soloShines[0].SetActive(true);
                
            }


        }
    }
}
