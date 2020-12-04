using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameMenuManager : MonoBehaviour
{
    public static IngameMenuManager instance;

    public GameObject ingameMenu;
    public Text healthText;
    public Text networkStatusText;
    public Text fpsCounterText;
    public Text pingText;
    public Text bulletText;

    public GameObject hitmarker;

    public GameObject settingsMenu;
    public InputField mouseSensitivityInput;

    private LocalPlayerController localPlayer = null;
    private bool settingsEnabled = false;

    public float fpsRefreshRate = 1.0f;
    private float fpsTimer;
    private float fps = 60.0f;

    public GameObject scoreboard;
    public Text[] scoreboardRows;
    private float scoreboardTimer;
    private float scoreboardRefreshRate = 1.0f;

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object.");
            Destroy(this);
        }
    }

    private void Start()
    {
        hitmarker.SetActive(false);
    }

    private void Update()
    {
        if (localPlayer)
        {
            healthText.text = Mathf.CeilToInt(localPlayer.health).ToString();
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleSettingsMenu();
            }
            else if (!settingsEnabled && Input.GetKey(KeyCode.Tab))
            {
                ShowScoreboard(true);
            } else
            {
                ShowScoreboard(false);
            }
        }
        if (Time.unscaledTime > fpsTimer)
        {
            float curFps = 1f / Time.unscaledDeltaTime;
            fps = fps * 0.8f + curFps * 0.2f;
            fpsCounterText.text = ((int)Mathf.Round(fps)).ToString();
            fpsTimer = Time.unscaledTime + fpsRefreshRate;
        }
        if (Time.time > scoreboardTimer)
        {
            UpdateScoreboard();
            scoreboardTimer = Time.time + scoreboardRefreshRate;
        }
    }

    private void ShowScoreboard(bool show)
    {
        if (show)
        {
            scoreboard.SetActive(true);
            ingameMenu.SetActive(false);
        } else if (scoreboard.activeInHierarchy)
        {
            scoreboard.SetActive(false);
            ingameMenu.SetActive(true);
        }
    }

    private void UpdateScoreboard()
    {
        for (int j = 0; j < scoreboardRows.Length; j++)
        {
            scoreboardRows[j].text = "";
        }
        int i = 0;
        foreach (PlayerController p in GameManager.players.Values)
        {
            if (p != null)
            {
                scoreboardRows[i].text = p.GetScore();
            }
            i++;
            if (i >= 8) break;
        }
    }

    private void ToggleSettingsMenu()
    {
        settingsEnabled = !settingsEnabled;
        settingsMenu.SetActive(settingsEnabled);
        ingameMenu.SetActive(!settingsEnabled);
        localPlayer.cameraController.ToggleCursorMode(!settingsEnabled);
    }

    public void SetMouseSensitivity()
    {
        if (float.TryParse(mouseSensitivityInput.text, out float sens))
        {
            localPlayer.cameraController.SetMouseSensitivity(sens);
        }
    }

    public void SetLocalPlayer(LocalPlayerController lp)
    {
        localPlayer = lp;
        UpdateSettings();
    }

    public void UpdateSettings()
    {
        mouseSensitivityInput.text = "1,0";
        SetMouseSensitivity();
    }

    public void DisableIsConnecting()
    {
        networkStatusText.text = "";
        //TODO StartCourontine isConnected
    }

    public void HitMark()
    {
        if (hitmarker.activeInHierarchy) return;
        hitmarker.SetActive(true);
        StartCoroutine(HitmarkerCooldown());
    }

    public void SetRTT(float srtt)
    {
        pingText.text = ((int)Mathf.Round(srtt * 500.0f)).ToString();
    }

    public void SetBullets(int bullets, int maxBullets)
    {
        bulletText.text = $"{bullets}/{maxBullets}";
    }

    private IEnumerator HitmarkerCooldown()
    {
        yield return new WaitForSeconds(0.1f);

        hitmarker.SetActive(false);
    }
}
