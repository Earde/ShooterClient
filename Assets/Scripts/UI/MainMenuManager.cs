using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager instance;

    public GameObject startMenu;
    public InputField usernameField;

    public GameObject progressMenu;

    public SceneLoader sceneLoader;

    private void Start()
    {
        startMenu.SetActive(true);
        progressMenu.SetActive(false);
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object.");
            Destroy(this);
        }
    }

    public void PlayGame(int level)
    {
        StaticCrossSceneData.Name = usernameField.text;
        startMenu.SetActive(false);
        progressMenu.SetActive(true);
        usernameField.interactable = false;

        sceneLoader.LoadLevel(level);
    }
}
