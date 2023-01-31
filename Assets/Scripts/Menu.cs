using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Menu : MonoBehaviour
{
    public float sceneTransitionTime = 3.5f;
    public int sceneToLoad;

    bool started = false;

    public void StartGame(InputAction.CallbackContext context)
    {
        if (!context.performed || started) { return; }
        else
        {

            SoundManager.instance.PlayOneShot(FMODEvents.instance.crown, transform.position);
            StartCoroutine(WaitUntilNextScene());
            started = true;
        }

    }

    IEnumerator WaitUntilNextScene()
    {
        yield return new WaitForSeconds(sceneTransitionTime);
        FindObjectOfType<SceneLoader>().LoadScene(sceneToLoad);
    }
}
