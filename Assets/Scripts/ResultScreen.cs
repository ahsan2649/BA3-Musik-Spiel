using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using TMPro;

public class ResultScreen : MonoBehaviour
{
    [SerializeField] List<GameObject> soloShines;
    [SerializeField] List<TextMeshProUGUI> player1_texts;
    [SerializeField] List<TextMeshProUGUI> player2_texts;
    [SerializeField] List<TextMeshProUGUI> player3_texts;
    [SerializeField] List<TextMeshProUGUI> player4_texts;

    [SerializeField] List<GameObject> playerModels;
    [SerializeField] List<GameObject> characterSprites;

    [SerializeField] Color noSoloShine;

    //PlayerPrefs

    List<string> rawPlayerStats;

    //Player Stats
    List<int> player_iD;
    List<int> player_solos;
    List<int> player_damage;
    List<int> player_finalblows;
    List<int> player_kicks;
    List<int> player_misses;
    List<int> player_pickups;
    List<int> player_avgSpeed;

    private void Start()
    {
        GetStats();
        UpdateStats();
    }

    void GetStats()
    {
        //Unpack all ints into lists
        for (int i = PlayerPrefs.GetInt("PlayerCount") - 1; i >= 0 ; i++)
        {
            rawPlayerStats[PlayerPrefs.GetInt("PlayerCount") -1 - i] = PlayerPrefs.GetString("Dead" + (PlayerPrefs.GetInt("PlayerCount") - 1 - i).ToString());
            string[] stats = Regex.Split(rawPlayerStats[PlayerPrefs.GetInt("PlayerCount") - 1 - i], @"\D+");
            for (int k = 0; k < stats.Length; k++)
            {
                if (!string.IsNullOrEmpty(stats[k]))
                {
                    int j = int.Parse(stats[k]);
                    
                    if (k == 0) { player_iD[PlayerPrefs.GetInt("PlayerCount") - 1 - i] = j; }
                    if (k == 1) { player_damage[PlayerPrefs.GetInt("PlayerCount") - 1 - i] = j; }
                    if (k == 2) { player_finalblows[PlayerPrefs.GetInt("PlayerCount") - 1 - i] = j; }
                    if (k == 3) { player_misses[PlayerPrefs.GetInt("PlayerCount") - 1 - i] = j; }
                    if (k == 4) { player_kicks[PlayerPrefs.GetInt("PlayerCount") - 1 - i] = j; }
                    if (k == 5) { player_pickups[PlayerPrefs.GetInt("PlayerCount") - 1 - i] = j; }
                    if (k == 6) { player_avgSpeed[PlayerPrefs.GetInt("PlayerCount") - 1 - i] = j; }
                    if (k == 7) { player_solos[PlayerPrefs.GetInt("PlayerCount") - 1 - i] = j; }
                }
            }
        }
    }

    void UpdateStats()
    {
        if (PlayerPrefs.GetInt("PlayerCount") == 1)
        {
            //Set Sprite
            playerModels[0] = characterSprites[player_iD[0]];

            //Set Shine Color
            if (player_solos[0] == 1)
            {
                soloShines[0].SetActive(true);
            }
            else
            {
                soloShines[0].SetActive(true);
                soloShines[0].GetComponent<SpriteRenderer>().color = noSoloShine;
            }
            player1_texts[0].text = player_damage[0].ToString();
            player1_texts[1].text = player_finalblows[0].ToString();
            player1_texts[2].text = player_kicks[0].ToString();
            player1_texts[3].text = player_misses[0].ToString();
            player1_texts[4].text = player_pickups[0].ToString();
            player1_texts[5].text = player_avgSpeed[0].ToString();
        }

        if (PlayerPrefs.GetInt("PlayerCount") == 2)
        {
            //Set Sprite
            playerModels[1] = characterSprites[player_iD[1]];
            //Set Shine Color
            if (player_solos[1] == 1)
            {
                soloShines[1].SetActive(true);
            }
            else
            {
                soloShines[1].SetActive(true);
                soloShines[1].GetComponent<SpriteRenderer>().color = noSoloShine;
            }
            player2_texts[0].text = player_damage[1].ToString();
            player2_texts[1].text = player_finalblows[1].ToString();
            player2_texts[2].text = player_kicks[1].ToString();
            player2_texts[3].text = player_misses[1].ToString();
            player2_texts[4].text = player_pickups[1].ToString();
            player2_texts[5].text = player_avgSpeed[1].ToString();
        }

        if (PlayerPrefs.GetInt("PlayerCount") == 3)
        {
            //Set Sprite
            playerModels[2] = characterSprites[player_iD[2]];
            //Set Shine Color
            if (player_solos[2] == 1)
            {
                soloShines[2].SetActive(true);
            }
            else
            {
                soloShines[2].SetActive(true);
                soloShines[2].GetComponent<SpriteRenderer>().color = noSoloShine;
            }
            player3_texts[0].text = player_damage[2].ToString();
            player3_texts[1].text = player_finalblows[2].ToString();
            player3_texts[2].text = player_kicks[2].ToString();
            player3_texts[3].text = player_misses[2].ToString();
            player3_texts[4].text = player_pickups[2].ToString();
            player3_texts[5].text = player_avgSpeed[2].ToString();
        }

        if (PlayerPrefs.GetInt("PlayerCount") == 4)
        {
            //Set Sprite
            playerModels[3] = characterSprites[player_iD[3]];
            //Set Shine Color
            if (player_solos[3] == 1)
            {
                soloShines[3].SetActive(true);
            }
            else
            {
                soloShines[3].SetActive(true);
                soloShines[3].GetComponent<SpriteRenderer>().color = noSoloShine;
            }
            player4_texts[0].text = player_damage[3].ToString();
            player4_texts[1].text = player_finalblows[3].ToString();
            player4_texts[2].text = player_kicks[3].ToString();
            player4_texts[3].text = player_misses[3].ToString();
            player4_texts[4].text = player_pickups[3].ToString();
            player4_texts[5].text = player_avgSpeed[3].ToString();
        }
    }
}
