using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GunManager : MonoBehaviour
{
    public Transform weaponHolder;
    public Camera cam;
    public GameObject[] guns;

    private List<GunController> gunControllers = new List<GunController>();
    private int curGun = 0;

    private void Start()
    {
        foreach (GameObject gun in guns)
        {
            GameObject g = Instantiate(gun);
            GunController gc = g.GetComponent<GunController>();
            gc.Initialize(cam, weaponHolder);
            gc.SetActive(false);
            gunControllers.Add(gc);
        }
        gunControllers[curGun].SetActive(true);
    }

    private void Update()
    {
        int newGun = -1;
        for (int i = 0; i < guns.Length; i++)
        {
            if (Input.GetKey(KeyCode.Alpha1 + i))
            {
                newGun = i;
                break;
            }
        }
        if (newGun >= 0)
        {
            gunControllers[curGun].SetActive(false);
            curGun = newGun;
            gunControllers[curGun].SetActive(true);
        }
    }
}