using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerGunController : GunController
{
    public Vector3 offset = new Vector3(0.275f, -0.6f, 0.614f);

    public bool isAutomatic = true;

    public float weaponLookDistance = 5.0f;

    public float recoilAngleInc = 1f;
    public float maxRecoilX = 20.0f;
    public float recoilSpeed = 10.0f;

    public float readyCooldown = 2.0f;
    public float shootCooldown = 0.4f;

    public bool useRecoil = true;
    public bool playSound = true;
    public bool loopShooting = false;

    private float curRecoil = 0.0f;
    private float eulerX = 0.0f;

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
        if (useRecoil) curRecoil += recoilAngleInc;
        StartCoroutine(ShootCooldown());
        // Send shot to network
        ClientSend.PlayerShoot(cam.transform.forward, Time.time, enemyDelay);
        // Sound
        if (playSound) shootSound.Play();
        // Muzzle flash position
        flash.transform.position = barrelPosition;
        flash.transform.rotation = transform.rotation;
        flash.SetActive(true);
        LocalHit(cam.transform.position, cam.transform.forward, 1000f);
    }

    protected override void Update()
    {
        base.Update();

        if (!isActive) return;
        // Weapon
        transform.position = weaponHolder.transform.position + cam.transform.forward * offset.z + cam.transform.right * offset.x + cam.transform.up * offset.y;
        transform.LookAt(cam.transform.position + cam.transform.forward * weaponLookDistance);
        // Recoil
        if (useRecoil)
        {
            float eX = transform.localEulerAngles.x;
            curRecoil = Mathf.Clamp(curRecoil, 0.0f, maxRecoilX);
            if (eulerX > curRecoil) eulerX = Mathf.Lerp(eulerX, curRecoil, Time.deltaTime * recoilSpeed);
            else eulerX = Mathf.Lerp(eulerX, curRecoil, Time.deltaTime * recoilSpeed / 2);
            transform.localEulerAngles = new Vector3(eX - eulerX, transform.localEulerAngles.y, transform.localEulerAngles.z);
            curRecoil -= Time.deltaTime;
        }
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
