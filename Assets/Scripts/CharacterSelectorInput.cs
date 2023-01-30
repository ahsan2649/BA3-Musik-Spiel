using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CharacterSelectorInput : MonoBehaviour
{
    [SerializeField] float stickSensitivity = 0.1f;
    CharacterSelecting cs;

    public Vector2 stickValue;
    public int selectedChar;
    public int playerNumber;

    public bool characterSubmitted = false;

    public bool everyoneReady = false;

    public bool firstInput = false;

    public int gamepadID;
    // Start is called before the first frame update
    void Start()
    {
        gamepadID = GetComponent<PlayerInput>().devices[0].deviceId;
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
            if (stickValue.x < -stickSensitivity && stickValue.y < -stickSensitivity)
            {
                tempSelect = 2;
            }
            if (stickValue.x > stickSensitivity && stickValue.y > stickSensitivity)
            {
                tempSelect = 1;
            }
            if (stickValue.x < -stickSensitivity && stickValue.y > stickSensitivity)
            {
                tempSelect = 0;
            }
            if (stickValue.x > stickSensitivity && stickValue.y < -stickSensitivity)
            {
                tempSelect = 3;
            }

            if (tempSelect != selectedChar)
            {
                if (cs != null)
                {
                    cs.RemoveVisualizer(playerNumber, selectedChar);
                    selectedChar = tempSelect;
                    cs.MoveSelector(selectedChar, playerNumber);
                }
            }
        }
    }

    public void Select(InputAction.CallbackContext context)
    {
        if (context.performed && firstInput)
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
                cs.StartGame();
            }
            
        }
        firstInput = true;
        
        
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
            else if(firstInput)
            {
                //Load Title Screen
                FindObjectOfType<SceneLoader>().LoadLastScene();
            }
            firstInput = true;
        }
    }

    public void SetFirstInput()
    {
        firstInput = true;
    }
}
