using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public int spawnerId;
    public bool hasItem;
    public List<MeshRenderer> itemModels;

    public float itemRotationSpeed = 50f;
    public float itemBobSpeed = 2f;
    private Vector3 basePosition;

    private void Start()
    {
        foreach (MeshRenderer mr in this.gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            itemModels.Add(mr);
        }
    }

    private void Update()
    {
        if (hasItem)
        {
            transform.Rotate(Vector3.up, itemRotationSpeed * Time.deltaTime, Space.World);
            transform.position = basePosition + new Vector3(0f, 0.25f * Mathf.Sin(Time.time * itemBobSpeed), 0f);
        }
    }

    public void Initialize(int _spawnerId, bool _hasItem)
    {
        spawnerId = _spawnerId;
        hasItem = _hasItem;
        EnableModels(hasItem);

        basePosition = transform.position;
    }

    private void EnableModels(bool enable)
    {
        foreach (MeshRenderer mr in itemModels)
        {
            mr.enabled = enable;
        }
    }

    public void ItemSpawned()
    {
        hasItem = true;
        EnableModels(hasItem);
    }

    public void ItemPickedUp()
    {
        hasItem = false;
        EnableModels(hasItem);
    }
}
