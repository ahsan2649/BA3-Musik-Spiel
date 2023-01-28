using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CharacterSelectorInput : MonoBehaviour
{
    CharacterSelecting cs;

    public Vector2 stickValue;
    public int selectedChar;
    public int playerNumber;

    public bool characterSubmitted;

    public bool everyoneReady = false;

    // Start is called before the first frame update
    void Start()
    {
        cs = FindObjectOfType<CharacterSelecting>();
        playerNumber = cs.AddPlayer(this);
        selectedChar = playerNumber;
        cs.MoveSelector(selectedChar, playerNumber);
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (characterSubmitted) { return; }
            int tempSelect = selectedChar;
            stickValue = context.ReadValue<Vector2>();
            if (stickValue.x < -0.5f && stickValue.y < -0.5f)
            {
                tempSelect = 2;
            }
            if (stickValue.x > 0.5f && stickValue.y > 0.5f)
            {
                tempSelect = 1;
            }
            if (stickValue.x < -0.5f && stickValue.y > 0.5f)
            {
                tempSelect = 0;
            }
            if (stickValue.x > 0.5f && stickValue.y < -0.5f)
            {
                tempSelect = 3;
            }

            if (tempSelect != selectedChar)
            {
                cs.RemoveVisualizer(playerNumber, selectedChar);
                selectedChar = tempSelect;
                cs.MoveSelector(selectedChar, playerNumber);
            }
        }
        
    }

    public void Select(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!characterSubmitted && cs != null)
            {
                if (cs.CharacterSubmit(selectedChar))
                {
                    characterSubmitted = true;
                }
            }
            else if (everyoneReady) 
            {
                //Start Game
            }
            
        }
        
        
    }

    public void Back(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (characterSubmitted && cs != null)
            {
                characterSubmitted = false;
                cs.DeselectChar(selectedChar);
            }
            else
            {
                //Load Title Screen
                FindObjectOfType<SceneLoader>().LoadLastScene();
            }
        }
        
    }
}
