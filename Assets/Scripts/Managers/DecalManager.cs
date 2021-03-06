﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DecalManager : MonoBehaviour
{
    public static DecalManager instance;

    public int activeDecalCount = 250;
    public List<GameObject> decalObjects;

    private List<GameObject> decals;

    private Queue<int> activeDecals;

    private int decalCount;

    //Singleton
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object.");
            Destroy(this);
        }
    }

    private void Start()
    {
        decalCount = activeDecalCount + 50;
        decalCount += decalCount % decalObjects.Count;
        decals = new List<GameObject>();
        activeDecals = new Queue<int>(activeDecalCount);
        int d = -1;
        for (int i = 0; i < decalCount; i++)
        {
            if (i % (decalCount / decalObjects.Count) == 0) d++;
            GameObject decal = Instantiate(decalObjects[d]);
            decal.SetActive(false);
            decals.Add(decal);
        }
    }

    /// <summary>
    /// Set bullet decal at position, normal
    /// </summary>
    /// <param name="position"></param>
    /// <param name="normal"></param>
    public void SetBulletDecal(Vector3 position, Quaternion normal)
    {
        GameObject[] nonActiveDecals = decals.Where(d => !d.activeInHierarchy).ToArray();
        GameObject decal = nonActiveDecals[Random.Range(0, nonActiveDecals.Length)];

        decal.transform.position = position;
        decal.transform.rotation = normal;
        decal.SetActive(true);
        decal.GetComponent<AudioSource>().Play();

        activeDecals.Enqueue(decals.IndexOf(decal));
        if (activeDecals.Count > activeDecalCount)
        {
            int i = activeDecals.Dequeue();
            decals[i].SetActive(false);
        }
    }
}