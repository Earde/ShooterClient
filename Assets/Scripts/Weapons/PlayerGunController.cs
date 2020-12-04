using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerGunController : GunController
{
    public Vector3 offset = new Vector3(0.275f, -0.6f, 0.614f);
    public float shootCooldownTime = 0.4f;

    public bool isAutomatic = true;
    public bool playSound = true;

    #region recoil
    public bool loopShooting = false;
    public bool useRecoil = true;

    public float maxRecoilAngle = 20.0f;
    public float recoilTimeInc = 0.1f;
    public float recoilSpeed = 10.0f;

    private float curRecoilTime = 0.0f;
    #endregion

    private Camera cam;

    private Transform weaponHolder;

    public float weaponLookDistance = 5.0f;

    private float eulerX = 0.0f;

    private Renderer[] renderers;

    private bool isActive = false;
    private bool isReadyToShoot = true;

    #region equip
    public float equipCooldownTime = 2.0f;
    public float equipRotations = 2.0f;

    private bool isEquiped = false;
    #endregion

    #region reload
    public float reloadCooldownTime = 3.0f;
    public float reloadRotations = 2.0f;

    private int curBullets = 20;
    public int maxBullets = 20;

    private Coroutine reloadCoroutine;
    #endregion

    public void Initialize(Camera camera, Transform _weaponHolder)
    {
        renderers = this.gameObject.GetComponentsInChildren<Renderer>();
        weaponHolder = _weaponHolder;
        cam = camera;
        flash = Instantiate(muzzleFlashParticle);
        curBullets = maxBullets;
    }

    public void SetActive(bool active)
    {
        if (reloadCoroutine != null) StopCoroutine(reloadCoroutine);
        isEquiped = false;
        isReadyToShoot = true;
        isActive = active;
        curRecoilTime = 0.0f;
        eulerX = 0.0f;
        foreach (Renderer r in renderers)
        {
            r.enabled = isActive;
        }
        if (isActive) StartCoroutine(EquipCooldown());
        if (curBullets <= 0)
        {
            reloadCoroutine = StartCoroutine(ReloadCooldown());
        }
        Update();
    }

    private IEnumerator EquipCooldown()
    {
        yield return new WaitForSeconds(equipCooldownTime);
        eulerX = 0.0f;
        curRecoilTime = 0.0f;
        isEquiped = true;
    }

    private IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(shootCooldownTime);
        isReadyToShoot = true;
    }

    private IEnumerator ReloadCooldown()
    {
        yield return new WaitForSeconds(reloadCooldownTime);
        curBullets = maxBullets;
    }

    public int GetBullets()
    {
        return curBullets;
    }

    public void Shoot(Vector3 barrelPosition)
    {
        if (!isActive) return;
        // Set shoot cooldown
        isReadyToShoot = false;
        if (useRecoil) curRecoilTime += recoilTimeInc;
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

        curBullets--;
        if (curBullets <= 0)
        {
            StartReload();
        }
    }

    private void StartReload()
    {
        eulerX = 0;
        curBullets = 0;
        reloadCoroutine = StartCoroutine(ReloadCooldown());
    }

    protected override void Update()
    {
        base.Update();

        if (curBullets > 0 && curBullets < maxBullets && Input.GetKeyDown(KeyCode.R))
        {
            StartReload();
        }

        if (!isActive) return;
        // Weapon position
        transform.position = weaponHolder.transform.position + cam.transform.forward * offset.z + cam.transform.right * offset.x + cam.transform.up * offset.y;
        transform.LookAt(cam.transform.position + cam.transform.forward * weaponLookDistance);
        // Equiping?
        if (!isEquiped)
        {
            eulerX += Time.deltaTime * (360.0f / equipCooldownTime) * equipRotations;
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x + eulerX, transform.localEulerAngles.y, transform.localEulerAngles.z);
        }
        // Reloading?
        else if (curBullets <= 0)
        {
            eulerX += Time.deltaTime * (360.0f / reloadCooldownTime) * reloadRotations;
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x - eulerX, transform.localEulerAngles.y, transform.localEulerAngles.z);
        }
        // Recoil
        else if (useRecoil)
        {
            if (curRecoilTime > 0.0f)
            {
                eulerX = Mathf.Lerp(eulerX, maxRecoilAngle, Time.deltaTime * recoilSpeed);
            } else
            {
                curRecoilTime = 0.0f;
                eulerX = Mathf.Lerp(eulerX, 0.0f, Time.deltaTime * recoilSpeed / 2);
            }
            eulerX = Mathf.Clamp(eulerX, 0.0f, maxRecoilAngle);

            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x - eulerX, transform.localEulerAngles.y, transform.localEulerAngles.z);
            curRecoilTime -= Time.deltaTime;
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
            isEquiped &&
            curBullets > 0 &&
            ((Input.GetMouseButton(0) && isAutomatic) || (Input.GetMouseButtonDown(0) && !isAutomatic))
            ))
        {
            Shoot(barrelPosition);
        }
    }
}
