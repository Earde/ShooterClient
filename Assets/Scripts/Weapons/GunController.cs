using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public GameObject muzzleFlashParticle;
    public Vector3 flashOffset = new Vector3(-0.035f, 0.4145f, 0.7936f);

    public AudioSource shootSound;

    public float enemyDelay = 0.1f;

    protected GameObject flash = null;

    protected void LocalHit(Vector3 fromPos, Vector3 forward, float distance, int id = -1)
    {
        RaycastHit[] hits = Physics.RaycastAll(fromPos, forward, distance);
        hits = hits.OrderBy(h => h.distance).ToArray();
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.CompareTag("Enemy"))
            {
                EnemyController ec = hits[i].collider.GetComponentInParent<EnemyController>();
                if (id >= 0 && id == ec.id)
                {
                    continue;
                }
                ec.TakeShot(hits[i].point + hits[i].normal * 0.001f, Quaternion.FromToRotation(Vector3.up, hits[i].normal));
            }
            else if (hits[i].collider.GetType() == typeof(MeshCollider) && hits[i].transform.gameObject.layer == LayerMask.NameToLayer("Map"))
            {
                DecalManager.instance.SetBulletDecal(hits[i].point + hits[i].normal * 0.001f, Quaternion.FromToRotation(Vector3.back, hits[i].normal));
                break;
            }
        }
    }

    protected virtual void Update() { }
}