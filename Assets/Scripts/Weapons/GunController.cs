using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Vector3 offset = new Vector3(0.275f, -0.6f, 0.614f);

    public GameObject muzzleFlashParticle;
    public Vector3 flashOffset = new Vector3(-0.035f, 0.4145f, 0.7936f);

    public bool isAutomatic = true;

    public float weaponLookDistance = 5.0f;

    public float recoilSpeedDec = 5.0f;
    public float recoilAngleInc = 2.5f;
    public float maxRecoil = 75.0f;

    public AudioSource shootSound;

    public float readyCooldown = 2.0f;
    public float shootCooldown = 0.4f;

    public bool useRecoil = true;
    public bool playSound = true;
    public bool loopShooting = false;

    private float curRecoil = 0.0f;

    private GameObject flash = null;
    private Camera cam;
    private Transform weaponHolder;

    private Renderer[] renderers;

    private bool isActive = false;
    private bool isReadyToUse = false;
    private bool isReadyToShoot = true;

    public void Initialize(Camera camera, Transform _weaponHolder)
    {
        renderers = this.gameObject.GetComponentsInChildren<Renderer>();
        weaponHolder = _weaponHolder;
        cam = camera;
        flash = Instantiate(muzzleFlashParticle);
    }

    public void SetActive(bool active)
    {
        isReadyToUse = false;
        isReadyToShoot = true;
        isActive = active;
        curRecoil = 0.0f;
        foreach (Renderer r in renderers)
        {
            r.enabled = isActive;
        }
        if (isActive) StartCoroutine(ReadyCooldown());
        Update();
    }

    private IEnumerator ReadyCooldown()
    {
        yield return new WaitForSeconds(readyCooldown);
        isReadyToUse = true;
    }

    private IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(shootCooldown);
        isReadyToShoot = true;
    }

    public void Shoot(Vector3 barrelPosition)
    {
        if (!isActive) return;
        // Set shoot cooldown
        isReadyToShoot = false;
        StartCoroutine(ShootCooldown());
        // Send shot to network
        ClientSend.PlayerShoot(cam.transform.forward, Time.time, GetPlayersTime());
        // Sound
        if (playSound) shootSound.Play();
        // Muzzle flash position
        flash.transform.position = barrelPosition;
        flash.transform.rotation = transform.rotation;
        flash.SetActive(true);
        LocalHit();
    }

    private void LocalHit()
    {
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 10000f))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                hit.collider.GetComponent<PlayerController>().TakeShot(hit.point, hit.normal);
            }
        }
    }

    private float GetPlayersTime()
    {
        float time = -1.0f;
        foreach (PlayerController p in GameManager.players.Values)
        {
            if (p == null || p.id == Client.instance.myId) continue;
            if (p.GetCurrentStateTime() > time)
            {
                time = p.GetCurrentStateTime();
            }
        }
        return time < 0.0f ? Time.time : time;
    }

    private float GetRecoilInc()
    {
        return Time.deltaTime * (recoilAngleInc / shootCooldown);
    }

    private void Update()
    {
        if (!isActive) return;
        // Weapon
        transform.position = weaponHolder.transform.position + weaponHolder.transform.forward * offset.z + weaponHolder.transform.right * offset.x + weaponHolder.transform.up * offset.y;
        transform.LookAt(cam.transform.position + cam.transform.forward * weaponLookDistance);
        // Recoil
        if (useRecoil)
        {
            if (!isReadyToShoot) curRecoil += GetRecoilInc();
            curRecoil -= Time.deltaTime * recoilSpeedDec;
        }
        curRecoil = Mathf.Clamp(curRecoil, 0.0f, maxRecoil);
        Vector3 eulerAngles = transform.eulerAngles;
        eulerAngles.x -= curRecoil;
        transform.eulerAngles = eulerAngles;
        // Barrel position
        Vector3 barrelPosition = transform.position +
            transform.forward * flashOffset.z +
            transform.right * flashOffset.x +
            transform.up * flashOffset.y;
        // Muzzle Flash position
        flash.transform.position = barrelPosition;
        flash.transform.rotation = transform.rotation;

        if (loopShooting || 
            (isReadyToShoot && 
            isReadyToUse &&
            ((Input.GetMouseButton(0) && isAutomatic) || (Input.GetMouseButtonDown(0) && !isAutomatic))
            ))
        {
            Shoot(barrelPosition);
        }
    }
}