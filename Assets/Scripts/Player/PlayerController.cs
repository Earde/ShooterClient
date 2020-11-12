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
    public SkinnedMeshRenderer model;
    public GameObject gun;
    public GameObject bloodPrefab;
    public int MaxBlood = 10;

    public MoveController moveController = null; //null if not localPlayer
    public CameraController cameraController = null; //null if not localPlayer
    public AudioController audioController = null;//null if not localPlayer

    private List<GameObject> blood = new List<GameObject>();

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

    public void Die()
    {
        audioController.Die();
        gun.SetActive(false);
        model.enabled = false;
    }

    public void Respawn()
    {
        gun.SetActive(true);
        model.enabled = true;
        SetHealth(maxHealth);
    }
}
