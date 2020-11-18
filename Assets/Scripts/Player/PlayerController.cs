using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class PlayerController : InterpolationController
{
    public int id;
    public string username;
    public float health;
    public float maxHealth;
    public int itemCount = 0;

    public SkinnedMeshRenderer meshRenderer;

    public GameObject bloodPrefab;
    public int MaxBlood = 10;

    private List<GameObject> blood = new List<GameObject>();

    public PlayerController(bool isLocalPlayer,
        bool positionInterpolation, 
        bool rotationInterpolation) : base(isLocalPlayer, positionInterpolation, rotationInterpolation) { }

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

    public virtual void SetHealth(float _health)
    {
        health = _health;
        if (health <= 0f) Die();
    }

    public virtual void Respawn()
    {
        SetHealth(maxHealth);
    }

    public abstract void ChangeColor();

    public abstract void SetLastAcceptedPosition(PlayerState state);

    public abstract void Hitmark();

    public abstract void Die();
}
