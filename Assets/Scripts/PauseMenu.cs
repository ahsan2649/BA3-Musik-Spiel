using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [Header("ButtonImages")]
    [SerializeField] List<GameObject> buttonImages;
    [SerializeField] List<GameObject> buttonImagesSel;



    //State
    int currentSelection = 0;
    public bool active = false;

    public void MoveSelectionUp()
    {
        if (!active) { return; }
        if (currentSelection < 2) { currentSelection++; }
        else { currentSelection = 0; }
        UpdateVisuals();
        SoundManager.instance.PlayOneShot(FMODEvents.instance.move, transform.position);
    }
    public void MoveSelectionDown()
    {
        if (!active) { return; }
        if (currentSelection > 0) { currentSelection--; }
        else { currentSelection = 2; }
        UpdateVisuals();
        SoundManager.instance.PlayOneShot(FMODEvents.instance.move, transform.position);
    }


    public void Select()
    {
        if (!active) { return; }
        SoundManager.instance.PlayOneShot(FMODEvents.instance.select, transform.position);
        if (currentSelection == 0)
        {
            //ResumePlay
        }

        if (currentSelection == 1)
        {
            //Open Options
        }

        if (currentSelection == 2)
        {
            //Go to Title Screen
            FindObjectOfType<SceneLoader>().LoadScene(0);
        }
    }

    public void Back()
    {
        if (!active) { return; }
        SoundManager.instance.PlayOneShot(FMODEvents.instance.back, transform.position);
        //Resume Play

    }

    void UpdateVisuals()
    {
        foreach (GameObject button in buttonImages)
        {
            if (button != buttonImages[currentSelection])
            {
                button.SetActive(true);
            }
            else
            {
                button.SetActive(false);
            }
        }
        foreach (GameObject button in buttonImagesSel)
        {
            if (button == buttonImagesSel[currentSelection])
            {
                button.SetActive(true);
            }
            else
            {
                button.SetActive(false);
            }
        }
    }
}
