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

    private int damageDone = 0;
    private int kills = 0;
    private int deaths = 0;

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

    /// <summary>
    /// Send movement direction to soundController
    /// </summary>
    protected override void Update()
    {
        base.Update();
        movementDelta = transform.position - prevPos;
        soundController.Move(movementDelta);
        prevPos = transform.position;
    }

    /// <summary>
    /// Get KDA
    /// </summary>
    /// <returns></returns>
    public string GetScore()
    {
        return $"{username} K: {kills} D: {deaths} Damage: {damageDone}";
    }

    /// <summary>
    /// Set score from server
    /// </summary>
    /// <param name="_damageDone"></param>
    /// <param name="_kills"></param>
    /// <param name="_deaths"></param>
    public void SetScore(int _damageDone, int _kills, int _deaths)
    {
        damageDone = _damageDone;
        kills = _kills;
        deaths = _deaths;
    }

    /// <summary>
    /// Alive?
    /// </summary>
    /// <returns></returns>
    public bool IsAlive()
    {
        return health > 0.0f;
    }

    /// <summary>
    /// Update health
    /// Die if health < 0
    /// </summary>
    /// <param name="_health"></param>
    public virtual void SetHealth(float _health)
    {
        health = _health;
        if (health <= 0f) Die();
    }

    /// <summary>
    /// Respawn player
    /// </summary>
    public virtual void Respawn()
    {
        SetHealth(maxHealth);
    }

    public abstract void ChangeColor();

    public abstract void SetLastAcceptedPosition(PlayerState state);

    public abstract void Hitmark();

    public abstract void Die();
}
