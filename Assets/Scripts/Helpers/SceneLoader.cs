using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public Text text;

    private string[] sceneNames;
    private bool mapLoaded = false;

    //Move objects from scene to new scene
    //https://stackoverflow.com/questions/45798666/move-transfer-gameobject-to-another-scene#:~:text=The%20main%20solution%20to%20your,SetActiveScene%20to%20activate%20the%20scene.
    void Start()
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        sceneNames = new string[sceneCount];
        for (int i = 0; i < sceneNames.Length; i++)
        {
            sceneNames[i] = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
        }
        foreach (string s in sceneNames)
        {
            Debug.Log("Scene found: " + s);
        }
    }

    public void LoadLevel(int level)
    {
        StartCoroutine(LoadScene(sceneNames[level]));
    }

    private IEnumerator LoadScene(string levelName)
    {
        yield return null;

        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);
        asyncOp.allowSceneActivation = false;

        while(asyncOp.progress < 0.9f)
        {
            text.text = ((int)(asyncOp.progress * 100f)).ToString() + "%";
        }
        asyncOp.allowSceneActivation = true;

        mapLoaded = true;

        yield return null;
    }

    private void Update()
    {
        if (mapLoaded)
        {
            Scene nextScene = SceneManager.GetSceneByName(sceneNames[0]);
            Scene activeScene = SceneManager.GetActiveScene();
            SceneManager.SetActiveScene(nextScene);
            SceneManager.UnloadSceneAsync(activeScene);
        }
    }
}
