using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameMenu : MonoBehaviour
{
    public static IngameMenu instance;

    public GameObject ingameMenu;
    public Text healthText;
    public Text networkStatusText;
    public Text fpsCounterText;
    public Text pingText;

    public GameObject hitmarker;

    public GameObject settingsMenu;
    public InputField mouseSensitivityInput;

    public float hudRefreshRate = 1.0f;

    private PlayerController localPlayer = null;
    private CameraController localCamera = null;
    private bool settingsEnabled = false;

    private float timer;

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
        }
        if (Time.unscaledTime > timer)
        {
            fpsCounterText.text = ((int)(1f / Time.unscaledDeltaTime)).ToString();
            pingText.text = (0.0f).ToString();
            timer = Time.unscaledTime + hudRefreshRate;
        }
    }

    private void ToggleSettingsMenu()
    {
        settingsEnabled = !settingsEnabled;
        settingsMenu.SetActive(settingsEnabled);
        ingameMenu.SetActive(!settingsEnabled);
        localCamera.ToggleCursorMode(!settingsEnabled);
    }

    public void SetMouseSensitivity()
    {
        if (float.TryParse(mouseSensitivityInput.text, out float sens))
        {
            localCamera.SetMouseSensitivity(sens);
        }
    }

    public void SetLocalPlayer(PlayerController lp)
    {
        localPlayer = lp;
        localCamera = localPlayer.cameraController;
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
        StartCoroutine(hitmarkerCooldown());
    }

    private IEnumerator hitmarkerCooldown()
    {
        yield return new WaitForSeconds(0.1f);

        hitmarker.SetActive(false);
    }
}
