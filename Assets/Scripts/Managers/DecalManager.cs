using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DecalManager : MonoBehaviour
{
    public static DecalManager instance;

    public int decalCount = 250;
    public List<GameObject> decalObjects;

    private List<GameObject> decals;

    private Queue<int> activeDecals;
    private int activeOffset = 20;

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
        decals = new List<GameObject>();
        activeDecals = new Queue<int>(decalCount - activeOffset);
        int d = 0;
        for (int i = 0; i < decalCount; i++)
        {
            if (i % (decalCount / decalObjects.Count) == decalCount / decalObjects.Count - 1) d++;
            GameObject decal = Instantiate(decalObjects[d]);
            decal.SetActive(false);
            decals.Add(decal);
        }
    }

    public void SetBulletDecal(Vector3 position, Quaternion hitRotation)
    {
        GameObject[] nonActiveDecals = decals.Where(d => !d.activeInHierarchy).ToArray();
        GameObject decal = nonActiveDecals[Random.Range(0, nonActiveDecals.Length)];

        decal.transform.position = position;
        decal.transform.rotation = hitRotation;
        decal.SetActive(true);

        activeDecals.Enqueue(decals.IndexOf(decal));
        if (activeDecals.Count > decalCount - activeOffset)
        {
            int i = activeDecals.Dequeue();
            decals[i].SetActive(false);
        }
    }
}