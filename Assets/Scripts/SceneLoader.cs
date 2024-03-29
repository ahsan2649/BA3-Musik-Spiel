using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] string[] scenes;
    [SerializeField] string prefer_scene = "";
    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void LoadLastScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void LoadRandomArena()
    {
        Destroy(menusong.instance.gameObject);
        //Do the randomization here?
        if (prefer_scene != "") SceneManager.LoadScene(prefer_scene);
        else SceneManager.LoadScene(scenes[Random.Range(0, scenes.Length)]);
    }
}
