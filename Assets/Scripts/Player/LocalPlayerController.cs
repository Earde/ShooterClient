using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LocalPlayerController : PlayerController
{
    public MoveController moveController; 
    public CameraController cameraController;
    public AudioController audioController;

    public LocalPlayerController() : base(true, true, false) { }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void SetLastAcceptedPosition(PlayerState state)
    {
        moveController.SetLastAcceptedPosition(state);
    }

    public override void SetHealth(float _health)
    {
        if (_health < health) audioController.TakeDamage();
        base.SetHealth(_health);
    }

    public override void Hitmark()
    {
        IngameMenuManager.instance.HitMark();
    }

    public override void Die()
    {
        audioController.Die();
    }

    public override void Respawn()
    {
        base.Respawn();
    }

    public override void ChangeColor()
    {
        //TODO: Change color based on missing health
    }
}
