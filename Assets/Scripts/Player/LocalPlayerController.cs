using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LocalPlayerController : PlayerController
{
    public MoveController moveController; 
    public CameraController cameraController;
    public AudioController audioController;

    private IngameMenu ingameMenu;

    public LocalPlayerController() : base(true, true, 2) { }

    private void Start()
    {
        ingameMenu = GameObject.FindObjectOfType<IngameMenu>();
    }

    public override void SetLastAcceptedPosition(PlayerState state)
    {
        moveController.SetLastAcceptedPosition(state);
        base.SetLastAcceptedPosition(state);
    }

    public override void SetHealth(float _health)
    {
        if (_health < health) audioController.TakeDamage();
        base.SetHealth(_health);
    }

    public override void Hitmark()
    {
        ingameMenu.HitMark();
    }

    public override void Die()
    {
        audioController.Die();
        base.Die();
    }

    public override void Respawn()
    {
        base.Respawn();
    }
}
