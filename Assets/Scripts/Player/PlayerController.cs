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
    public GameObject colliders;

    public SoundController soundController;

    protected Vector3 movementDelta = Vector3.zero;
    private Vector3 prevPos = Vector3.zero;

    public PlayerController(bool isLocalPlayer,
        bool positionInterpolation, 
        bool rotationInterpolation) : base(isLocalPlayer, positionInterpolation, rotationInterpolation) { }

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        health = maxHealth;
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        movementDelta = transform.position - prevPos;
        soundController.Move(movementDelta);
        prevPos = transform.position;
    }

    public bool IsAlive()
    {
        return health > 0.0f;
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
