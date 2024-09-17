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
    [SerializeField] private float shootingDelay = 0.3f;
    [SerializeField] private bool automaticFire = false;
    [SerializeField] private float spreadIntensity = 0.05f;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private float bulletSpeed = 30.0f;
    [SerializeField] private float lifeTime = 3.0f;

    [SerializeField] private AudioSource shootAudioSource;

    private PlayerInputActions _playerInputActions;
    private Coroutine _continuousFireCoroutine;
    private float _buttonHoldTime = 0.0f;
    private float _holdThreshold = 0.3f;

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();

        readyToShoot = true;
    }

    private void Start()
    {
        _playerInputActions.Player.Shoot.performed += StartShooting;
        _playerInputActions.Player.Shoot.canceled += StopShooting;
        _playerInputActions.Player.Enable();
    }

    private void Update()
    {
        if (isShooting)
        {
            _buttonHoldTime += Time.deltaTime;

            if (_buttonHoldTime > _holdThreshold && !automaticFire)
            {
                automaticFire = true;
                if (_continuousFireCoroutine == null)
                {
                    _continuousFireCoroutine = StartCoroutine(ContinuousFire());
                }
            }
        }
    }
    private void StartShooting(InputAction.CallbackContext context)
    {
        _buttonHoldTime = 0.0f;
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
        _buttonHoldTime = 0.0f;
        readyToShoot = true;

        if (_continuousFireCoroutine != null)
        {
            StopCoroutine(_continuousFireCoroutine);
            _continuousFireCoroutine = null;
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
        if (shootAudioSource != null)
        {
            shootAudioSource.Play();
        }

        Vector3 shootingDirection = CalcDirectionAndSpread().normalized;
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        bullet.transform.forward = shootingDirection;
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletSpeed, ForceMode.Impulse);
        StartCoroutine(DestroyBullet(bullet, lifeTime));
    }

    private Vector3 CalcDirectionAndSpread()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100);
        }

        Vector3 shootingDirection = targetPoint - bulletSpawn.position;
        float xSpread = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float ySpread = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        shootingDirection.x += xSpread;
        shootingDirection.y += ySpread;

        return shootingDirection;
    }

    private IEnumerator DestroyBullet(GameObject bullet, float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(bullet);
    }
}
