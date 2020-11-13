using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyController : PlayerController
{
    public SkinnedMeshRenderer model;

    public EnemyController() : base(true, true, 2) { }

    public override void SetLastAcceptedPosition(PlayerState state)
    {
        SetNewState(state);
    }

    public override void SetHealth(float _health)
    {
        base.SetHealth(_health);
    }

    public override void Hitmark()
    {
        base.Hitmark();
    }

    public override void Die()
    {
        model.enabled = false; //local player is disabled anyway
        base.Die();
    }

    public override void Respawn()
    {
        model.enabled = true;
        base.Respawn();
    }
}
