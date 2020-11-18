using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyController : PlayerController
{
    public EnemyController() : base(false, true, true) { }

    public override void SetLastAcceptedPosition(PlayerState state)
    {
        AddPlayerState(state);
    }

    public override void SetHealth(float _health)
    {
        base.SetHealth(_health);
    }

    public override void Die()
    {
        meshRenderer.enabled = false;
    }

    public override void Respawn()
    {
        meshRenderer.enabled = true;
        base.Respawn();
    }

    public override void ChangeColor()
    {
        meshRenderer.sharedMaterial.SetColor("_BaseColor", Color.green);
    }

    public override void Hitmark() { }
}
