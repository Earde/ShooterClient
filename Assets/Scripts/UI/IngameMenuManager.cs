using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameMenuManager : MonoBehaviour
{
    public static IngameMenuManager instance;

    [Header("Setup")]
    public GameObject ingameMenu;
    public Text healthText;
    public Text networkStatusText;
    public Text fpsCounterText;
    public Text pingText;
    public Text bulletText;

    private LocalPlayerController localPlayer = null;
    private bool settingsEnabled = false;

    [Header("Hitmarker")]
    public GameObject hitmarker;
    public float hitmarkerCooldown = 0.1f;

    [Header("Settings Menu")]
    public GameObject settingsMenu;
    public InputField mouseSensitivityInput;

    [Header("FPS")]
    public float fpsRefreshRate = 1.0f;

    private float fpsTimer;
    private float fps = 60.0f;

    [Header("Scoreboard")]
    public GameObject scoreboard;
    public Text[] scoreboardRows;

    private float scoreboardTimer;
    private float scoreboardRefreshRate = 1.0f;

    //Singleton
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
            // Health
            healthText.text = Mathf.CeilToInt(localPlayer.health).ToString();
            // Settings Menu
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleSettingsMenu();
            }
            // Show Scoreboard
            else if (!settingsEnabled && Input.GetKey(KeyCode.Tab))
            {
                ShowScoreboard(true);
            } else
            {
                ShowScoreboard(false);
            }
        }
        // Frame per second
        if (Time.unscaledTime > fpsTimer)
        {
            float curFps = 1f / Time.unscaledDeltaTime;
            fps = fps * 0.8f + curFps * 0.2f; //smooth fps
            fpsCounterText.text = ((int)Mathf.Round(fps)).ToString();
            fpsTimer = Time.unscaledTime + fpsRefreshRate;
        }
        // Update Score board
        if (Time.time > scoreboardTimer)
        {
            UpdateScoreboard();
            scoreboardTimer = Time.time + scoreboardRefreshRate;
        }
    }

    /// <summary>
    /// Show scoreboard
    /// </summary>
    /// <param name="show"></param>
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

    /// <summary>
    /// Update scoreboard
    /// </summary>
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

    /// <summary>
    /// Enable/disable settings menu
    /// </summary>
    private void ToggleSettingsMenu()
    {
        settingsEnabled = !settingsEnabled;
        settingsMenu.SetActive(settingsEnabled);
        ingameMenu.SetActive(!settingsEnabled);
        localPlayer.cameraController.ToggleCursorMode(!settingsEnabled);
    }

    /// <summary>
    /// Set mouse sensitivity
    /// </summary>
    public void SetMouseSensitivity()
    {
        if (float.TryParse(mouseSensitivityInput.text, out float sens))
        {
            localPlayer.cameraController.SetMouseSensitivity(sens);
        }
    }

    /// <summary>
    /// Set local player after server accepted client
    /// </summary>
    /// <param name="lp"></param>
    public void SetLocalPlayer(LocalPlayerController lp)
    {
        localPlayer = lp;
        UpdateSettings();
    }

    /// <summary>
    /// Update settings
    /// TODO: Load settings from save
    /// </summary>
    public void UpdateSettings()
    {
        mouseSensitivityInput.text = "1,0";
        SetMouseSensitivity();
    }

    /// <summary>
    /// Disable Is Connecting message
    /// </summary>
    public void DisableIsConnecting()
    {
        networkStatusText.text = "";
        //TODO StartCourontine isConnected
    }

    /// <summary>
    /// Display hitmarker
    /// </summary>
    public void HitMark()
    {
        if (hitmarker.activeInHierarchy) return;
        hitmarker.SetActive(true);
        StartCoroutine(HitmarkerCooldown());
    }

    /// <summary>
    /// Set Network Latency
    /// smoothedRTT to network latency
    /// </summary>
    /// <param name="smootedRTT"></param>
    public void SetNetworkLatency(float smootedRTT)
    {
        // (* 1000 / 2) = (* 500) omdat we de netwerk latency ipv RTT willen weten in millisecondes
        pingText.text = ((int)Mathf.Round(smootedRTT * 500.0f)).ToString();
    }

    /// <summary>
    /// Update bullet info
    /// </summary>
    /// <param name="bullets"></param>
    /// <param name="maxBullets"></param>
    public void SetBullets(int bullets, int maxBullets)
    {
        bulletText.text = $"{bullets}/{maxBullets}";
    }

    /// <summary>
    /// Hitmarker cooldown
    /// </summary>
    /// <returns></returns>
    private IEnumerator HitmarkerCooldown()
    {
        yield return new WaitForSeconds(hitmarkerCooldown);

        hitmarker.SetActive(false);
    }
}
