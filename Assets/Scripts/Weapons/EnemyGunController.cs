using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyGunController : GunController
{
    private List<ShootInput> shootInputs = new List<ShootInput>();

    class ShootInput
    {
        public int ShooterId { get; set; }
        public float Time { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Forward { get; set; }
    }

    private void Start()
    {
        flash = Instantiate(muzzleFlashParticle);
        flash.SetActive(false);
    }

    public void AddShot(float time, Vector3 position, Vector3 forward, int shooterId)
    {
        flash.SetActive(true);
        shootInputs.Add(new ShootInput { Time = time, Forward = forward, Position = position, ShooterId = shooterId });
        shootInputs = shootInputs.OrderBy(si => si.Time).ToList();
    }

    protected override void Update()
    {
        base.Update();

        if (shootInputs.Count > 0 && shootInputs.First().Time < Time.time - enemyDelay)
        {
            flash.SetActive(true);
            LocalHit(shootInputs.First().Position, shootInputs.First().Forward, 1000.0f, shootInputs.First().ShooterId);
            shootInputs.RemoveAt(0);
            shootSound.Play();
        }

        Vector3 barrelPosition = transform.position +
            transform.forward * flashOffset.z +
            transform.right * flashOffset.x +
            transform.up * flashOffset.y;

        flash.transform.position = barrelPosition;
        flash.transform.rotation = transform.rotation;
    }
}
