﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyController : PlayerController
{
    public EnemyGunController gunController;
    public MeshRenderer[] gunRenderers;

    public GameObject bloodPrefab;
    public int MaxBlood = 10;

    private List<GameObject> blood = new List<GameObject>();

    public EnemyAnimationController animationController;

    public float colorLerpTime = 2.5f;
    
    private float colorTime = 0.0f;
    private Color prevColor = Color.green;
    private Color toColor = Color.red;

    public EnemyController() : base(false, true, true) { }

    protected override void Start()
    {
        for (int i = 0; i < MaxBlood; i++)
        {
            GameObject b = Instantiate(bloodPrefab);
            b.SetActive(false);
            b.transform.parent = this.transform;
            blood.Add(b);
        }
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        animationController.Move(movementDelta, this.transform.forward);
        ChangeColor();
    }

    public void Shoot(float time, Vector3 pos, Vector3 forw)
    {
        gunController.AddShot(time, pos, forw, id);
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
        foreach (MeshRenderer rd in gunRenderers)
        {
            rd.enabled = false;
        }
        colliders.SetActive(false);
    }

    public override void Respawn()
    {
        meshRenderer.enabled = true;
        foreach (MeshRenderer rd in gunRenderers)
        {
            rd.enabled = true;
        }
        colliders.SetActive(true);
        base.Respawn();
    }

    public override void ChangeColor()
    {
        if (colorTime >= colorLerpTime)
        {
            prevColor = toColor;
            toColor = Random.ColorHSV();
            colorTime = 0.0f;
        }

        foreach (Material mat in meshRenderer.materials)
        {
            mat.SetColor("_BaseColor", Color.Lerp(prevColor, toColor, colorTime / colorLerpTime));
        }

        colorTime += Time.deltaTime;
    }

    public override void Hitmark() { }
}
