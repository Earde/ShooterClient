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

    private List<PlayerGunController> gunControllers = new List<PlayerGunController>();
    private int curGun = 0;

    private void Start()
    {
        foreach (GameObject gun in guns)
        {
            GameObject g = Instantiate(gun);
            PlayerGunController gc = g.GetComponent<PlayerGunController>();
            gc.Initialize(cam, weaponHolder);
            gc.SetActive(false);
            gunControllers.Add(gc);
        }
        gunControllers[curGun].SetActive(true);
    }

    private void Update()
    {
        // Update bullet info
        IngameMenuManager.instance.SetBullets(gunControllers[curGun].GetBullets(), gunControllers[curGun].maxBullets);
        // Change gun
        int newGun = -1;
        for (int i = 0; i < guns.Length; i++)
        {
            if (Input.GetKey(KeyCode.Alpha1 + i))
            {
                newGun = i;
                break;
            }
        }
        if (newGun >= 0 && curGun != newGun)
        {
            gunControllers[curGun].SetActive(false);
            curGun = newGun;
            gunControllers[curGun].SetActive(true);
            ClientSend.PlayerChangeGun(Time.time, curGun);
        }
    }
}