using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    [SerializeField] Camera playerCamera;

    [SerializeField] private bool isShooting;
    [SerializeField] private bool readyToShoot;
    [SerializeField] private bool allowReset = true;
    [SerializeField] private float shootingDelay = 0.3f;

    [SerializeField] private bool automaticFire = false;
    [SerializeField] private int bulletsPerBurst = 3;
    [SerializeField] private int currentBurst;

    [SerializeField] private float spreadIntensity;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private float bulletSpeed = 30.0f;
    [SerializeField] private float lifeTime = 3.0f;

    [SerializeField] private enum ShootingMode { Single, Burst };

    [SerializeField] private ShootingMode shootingMode;

    private PlayerInputActions playerInputActions;
    private Coroutine continuousFireCoroutine;
    private float buttonHoldTime = 0.0f;
    private float holdThreshold = 0.3f;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();

        readyToShoot = true;
    }

    private void Start()
    {
        playerInputActions.Player.Shoot.performed += StartShooting;
        playerInputActions.Player.Shoot.canceled += StopShooting;
        playerInputActions.Player.Enable();
    }

    private void Update()
    {
        if (isShooting)
        {
            buttonHoldTime += Time.deltaTime;

            if (buttonHoldTime > holdThreshold && !automaticFire)
            {
                automaticFire = true;
                if (continuousFireCoroutine == null)
                {
                    continuousFireCoroutine = StartCoroutine(ContinuousFire());
                }
            }
        }
    }
    private void StartShooting(InputAction.CallbackContext context)
    {
        buttonHoldTime = 0.0f;
        isShooting = true;

        if (!automaticFire && readyToShoot)
        {
            FireWeapon();
        }
    }

    private void StopShooting(InputAction.CallbackContext context)
    {
        isShooting = false;
        automaticFire = false;
        buttonHoldTime = 0.0f;
        readyToShoot = true;

        if (continuousFireCoroutine != null)
        {
            StopCoroutine(continuousFireCoroutine);
            continuousFireCoroutine = null;
        }
    }

    private IEnumerator ContinuousFire()
    {
        while (isShooting)
        {
            if (readyToShoot)
            {
                FireWeapon();
                readyToShoot = false;

                yield return new WaitForSeconds(shootingDelay);

                readyToShoot = true;
            }
            yield return null;
        }
    }

    private void FireWeapon()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().AddForce(bulletSpawn.forward.normalized * bulletSpeed, ForceMode.Impulse);
        StartCoroutine(DestroyBullet(bullet, lifeTime));
    }

    private IEnumerator DestroyBullet(GameObject bullet, float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(bullet);
    }
}
