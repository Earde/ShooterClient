using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : InterpolationController
{
    public int id;
    public string username;
    public float health;
    public float maxHealth;
    public int itemCount = 0;

    public GameObject bloodPrefab;
    public int MaxBlood = 10;

    //null if localPlayer
    public SkinnedMeshRenderer model;

    //null if not localPlayer
    public MoveController moveController = null; 
    public CameraController cameraController = null;
    public AudioController audioController = null;

    private List<GameObject> blood = new List<GameObject>();

    private IngameMenu ingameMenu;

    public PlayerController() : base(true, true, 2) { }

    private void Start()
    {
        for (int i = 0; i < MaxBlood; i++)
        {
            GameObject b = Instantiate(bloodPrefab);
            b.SetActive(false);
            blood.Add(b);
        }
    }

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        health = maxHealth;
        ingameMenu = GameObject.FindObjectOfType<IngameMenu>();
    }

    public bool IsAlive()
    {
        return health > 0.0f;
    }

    public void SetLastAcceptedPosition(PlayerState state)
    {
        if (moveController != null) { moveController.SetLastAcceptedPosition(state); }
        else { SetNewState(state); }
    }

    public void SetHealth(float _health)
    {
        if (_health < health && audioController != null) audioController.TakeDamage();
        health = _health;

        if (health <= 0f) Die();
    }

    public void TakeShot(Vector3 position, Vector3 view)
    {
        GameObject b = blood.FirstOrDefault(bl => !bl.activeInHierarchy);
        if (b != default)
        {
            b.transform.position = position;
            b.transform.rotation.SetLookRotation(view);
            b.SetActive(true);
        }
    }

    public void Hitmark()
    {
        ingameMenu.HitMark();
    }

    public void Die()
    {
        if (audioController != null) audioController.Die();
        if (model != null) model.enabled = false; //local player is disabled anyway
    }

    public void Respawn()
    {
        if (model != null) model.enabled = true;
        SetHealth(maxHealth);
    }
}
