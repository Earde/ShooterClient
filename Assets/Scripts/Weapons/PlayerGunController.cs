using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerGunController : GunController
{
    [Header("Setup")]
    public Vector3 offset = new Vector3(0.275f, -0.6f, 0.614f);
    public bool isAutomatic = true;
    public bool playSound = true;
    public float weaponLookDistance = 5.0f;
    public float shootCooldownTime = 0.4f;

    private bool isReadyToShoot = true;
    private Camera cam;
    private float eulerX = 0.0f;
    private Renderer[] renderers;
    private bool isActive = false;

    [Header("Equip")]
    public float equipCooldownTime = 2.0f;
    public float equipRotations = 2.0f;

    private bool isEquiped = false;

    [Header("Recoil")]
    public bool loopShooting = false;
    public bool useRecoil = true;

    public float maxRecoilAngle = 20.0f;
    public float recoilTimeInc = 0.1f;
    public float recoilSpeed = 10.0f;

    private float curRecoilTime = 0.0f;

    [Header("Zoom")]
    public float zoomTime = 1.0f;
    private float curZoomTime = 0.0f;
    private Vector3 zoomTarget;
    private Vector3 zoomInit;
    public float fovZoomIncrease = 24.0f;

    private Transform weaponHolder;
    private float fovInit;

    [Header("Reload")]
    public float reloadCooldownTime = 3.0f;
    public float reloadRotations = 2.0f;
    public int maxBullets = 20;

    private int curBullets = 20;
    private Coroutine reloadCoroutine;

    public void Initialize(Camera camera, Transform _weaponHolder)
    {
        renderers = this.gameObject.GetComponentsInChildren<Renderer>();
        weaponHolder = _weaponHolder;
        zoomInit = new Vector3(weaponHolder.localPosition.x, weaponHolder.localPosition.y, weaponHolder.localPosition.z);
        zoomTarget = new Vector3(0.0f, weaponHolder.localPosition.y, weaponHolder.localPosition.z);
        cam = camera;
        fovInit = cam.fieldOfView;
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
        // Process shot locally
        LocalHit(cam.transform.position, cam.transform.forward, 1000f);
        // Reduce bullets
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

    private void UpdateZoom(float delta)
    {
        curZoomTime += delta;
        curZoomTime = Mathf.Clamp(curZoomTime, 0.0f, zoomTime);
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
        //Zoom
        if (isEquiped && curBullets > 0 && Input.GetMouseButton(1))
        {
            UpdateZoom(Time.deltaTime);
        } else
        {
            UpdateZoom(-Time.deltaTime);
        }
        weaponHolder.transform.localPosition = Vector3.Lerp(zoomInit, zoomTarget, (curZoomTime / zoomTime));
        cam.fieldOfView = Mathf.Lerp(fovInit, fovInit - fovZoomIncrease, (curZoomTime / zoomTime));
        //Shoot
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
