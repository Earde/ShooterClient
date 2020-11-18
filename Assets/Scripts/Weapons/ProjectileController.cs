using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : InterpolationController
{
    public int id;
    public GameObject explosionPrefab;

    public ProjectileController() : base(false, true, false) { }

    public void Initialize(int _id)
    {
        id = _id;
    }

    public void Explode(Vector3 position)
    {
        transform.position = position;
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        GameManager.projectiles.Remove(id);
        Destroy(gameObject);
    }
}
