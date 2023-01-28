using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelecting : MonoBehaviour
{
    [Header("Selected Character Images")]
    [SerializeField] GameObject[] selectedImage;

    [Header("Player Colors")]
    [SerializeField] Color[] playerCol;

    [Header("Astroboy")]
    [SerializeField] Transform[] astroPos;

    [Header("Demongirl")]
    [SerializeField] Transform[] demonPos;

    [Header("Angelgirl")]
    [SerializeField] Transform[] angelPos;

    [Header("Sharkboy")]
    [SerializeField] Transform[] sharkPos;

    public bool[] characters = new bool[3];
    public List<CharacterSelectorInput> players = new List<CharacterSelectorInput>();

    [SerializeField] GameObject readyBanner;

    List<int> playerVisualizers = new List<int>();
    Transform[] visualizers;
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("Player1Char", 0);
        PlayerPrefs.SetInt("Player2Char", 1);
        PlayerPrefs.SetInt("Player3Char", 2);
        PlayerPrefs.SetInt("Player4Char", 3);
    }

    // Update is called once per frame
    void Update()
    {
        bool allPlayersSubmitted = false;
        //Check for all players have character submitted
        foreach (CharacterSelectorInput i in players)
        {
            if (i.characterSubmitted)
            {
                allPlayersSubmitted = true;
            }
            else
            {
                allPlayersSubmitted = false;
            }
        }
        if (allPlayersSubmitted)
        {
            //Activate Banner for ready play
            readyBanner.SetActive(true);
            foreach (CharacterSelectorInput i in players)
            {
                i.everyoneReady = true;
            }
        }
        else
        {
            readyBanner.SetActive(false);
            foreach (CharacterSelectorInput i in players)
            {
                i.everyoneReady = false;
            }
        }
    }

    public int AddPlayer(CharacterSelectorInput player)
    {
        players.Add(player);
        playerVisualizers.Add(players.IndexOf(player));
        return players.IndexOf(player);
    }

    public bool CharacterSubmit(int characterInt)
    {
        if (characters[characterInt] == false)
        {
            characters[characterInt] = true;
            selectedImage[characterInt].SetActive(true);
            return true;            
        }
        else
        {
            return false;
        }
    }

    public void MoveSelector(int characterInt, int playerNumber)
    {
        if (characterInt == 0) { visualizers = astroPos; }
        if (characterInt == 1) { visualizers = demonPos; }
        if (characterInt == 2) { visualizers = angelPos; }
        if (characterInt == 3) { visualizers = sharkPos; }

        //Sets new visualizers
        for (int i = 0; i < 4; i++)
        {
            if (!visualizers[i].gameObject.activeSelf)
            {
                visualizers[i].gameObject.SetActive(true);
                visualizers[i].GetComponent<Image>().color = playerCol[playerNumber];
                playerVisualizers[playerNumber] = i;
                i = 4;
            }
        }
    }
    
    public void RemoveVisualizer(int playerNumber, int characterInt)
    {
        if (characterInt == 0) { visualizers = astroPos; }
        if (characterInt == 1) { visualizers = demonPos; }
        if (characterInt == 2) { visualizers = angelPos; }
        if (characterInt == 3) { visualizers = sharkPos; }

        visualizers[playerVisualizers[playerNumber]].gameObject.SetActive(false);
    }

    public void DeselectChar(int selectedChar)
    {
        characters[selectedChar] = false;
        selectedImage[selectedChar].SetActive(false);
    }

    public void StartGame()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (i == 0) { PlayerPrefs.SetInt("Player0Char", players[i].selectedChar); }
            if (i == 1) { PlayerPrefs.SetInt("Player1Char", players[i].selectedChar); }
            if (i == 2) { PlayerPrefs.SetInt("Player2Char", players[i].selectedChar); }
            if (i == 3) { PlayerPrefs.SetInt("Player3Char", players[i].selectedChar); }
        }
    }
}
