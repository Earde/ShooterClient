using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyController : PlayerController
{
    public GameObject bloodPrefab;
    public int MaxBlood = 10;

    private List<GameObject> blood = new List<GameObject>();

    public EnemyController() : base(false, true, true) { }

    protected override void Start()
    {
        ChangeColor();
        for (int i = 0; i < MaxBlood; i++)
        {
            GameObject b = Instantiate(bloodPrefab);
            b.SetActive(false);
            b.transform.parent = this.transform;
            blood.Add(b);
        }
        base.Start();
    }

    public void TakeShot(Vector3 hitPoint, Quaternion hitRotation)
    {
        GameObject b = blood.FirstOrDefault(bl => !bl.activeInHierarchy);
        if (b != default && b != null)
        {
            b.transform.position = hitPoint;
            b.transform.rotation = hitRotation;
            b.SetActive(true);
        }
    }

    public override void SetLastAcceptedPosition(PlayerState state)
    {
        state._time = Time.time;
        AddPlayerState(state);
    }

    public override void SetHealth(float _health)
    {
        base.SetHealth(_health);
    }

    public override void Die()
    {
        meshRenderer.enabled = false;
        colliders.SetActive(false);
    }

    public override void Respawn()
    {
        meshRenderer.enabled = true;
        colliders.SetActive(true);
        base.Respawn();
    }

    public override void ChangeColor()
    {
        foreach (Material mat in meshRenderer.materials)
        {
            mat.SetColor("_BaseColor", Color.green);
        }
    }

    public override void Hitmark() { }
}
