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

    private List<GameObject> blood = new List<GameObject>();

    public PlayerController(bool positionInterpolation, 
        bool rotationInterpolation,
        int stateHistoryCount) : base(positionInterpolation, rotationInterpolation, stateHistoryCount) { }

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

    public virtual void SetLastAcceptedPosition(PlayerState state) { }

    public virtual void SetHealth(float _health)
    {
        health = _health;

        if (health <= 0f) Die();
    }

    public virtual void Hitmark() { }

    public virtual void Die() { }

    public virtual void Respawn()
    {
        SetHealth(maxHealth);
    }
}
