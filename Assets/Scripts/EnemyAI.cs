using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField]
    private float rotationSpeed = 2f;

    [SerializeField]
    private float hoverHeight = 10f;

    [SerializeField]
    private float maxDistance = 0.1f;

    [SerializeField]
    private float levitationSpeed = 2f;

    [SerializeField]
    private float cowStopDistance = 0.5f;

    [SerializeField]
    private float rotationSpeedDuringLevitation = 30f;

    private Transform myTransform;
    private Transform currentCow;
    private bool isCowLevitating = false;
    private Vector3 lastPosition;
    private Quaternion targetRotation;

    [Header("Audio Sources")]
    [SerializeField]
    private AudioSource moveAudioSource;
    [SerializeField]
    private AudioSource levitationAudioSource;

    [SerializeField]
    private AudioClip levitationSoundClip;

    void Awake()
    {
        myTransform = transform;
        lastPosition = myTransform.position;

        moveAudioSource.Play();
        moveAudioSource.Pause();

        levitationAudioSource.Play();
        levitationAudioSource.Pause();
    }

    void Start()
    {
        FindNextCow();
    }

    void Update()
    {
        if (target != null)
        {
            HandleMovement();
        }
        if (isCowLevitating && currentCow != null)
        {
            LevitateCow();
            RotateCowRandomly();
        }
    }

    private void HandleMovement()
    {
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y + hoverHeight, target.position.z);

        float distanceToTarget = Vector3.Distance(new Vector3(target.position.x, 0, target.position.z), new Vector3(myTransform.position.x, 0, myTransform.position.z));

        HandleRotation();

        if (!isCowLevitating)
        {
            if (distanceToTarget > maxDistance + 0.01f)
            {
                myTransform.position = Vector3.MoveTowards(myTransform.position, targetPosition, moveSpeed * Time.deltaTime);
            }
            else
            {
                myTransform.position = targetPosition;
                CheckForCowBelow();
            }
        }

        HandleAudio();

        lastPosition = myTransform.position;
    }

    private void HandleRotation()
    {
        Vector3 direction = (new Vector3(target.position.x, myTransform.position.y, target.position.z) - myTransform.position);
        if (!isCowLevitating && direction.sqrMagnitude > 0.001f)
        {
            direction.Normalize();
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleAudio()
    {
        if (myTransform.position != lastPosition)
        {
            if (!moveAudioSource.isPlaying)
            {
                moveAudioSource.UnPause();
            }
        }
        else
        {
            if (moveAudioSource.isPlaying)
            {
                moveAudioSource.Pause();
            }
        }
    }

    private void CheckForCowBelow()
    {
        Vector3 rayOrigin = myTransform.position;
        Vector3 rayDirection = Vector3.down;

        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag("Cow"))
            {
                currentCow = hit.collider.transform;
                isCowLevitating = true;
                SetRandomCowRotation();

                Rigidbody cowRigidbody = currentCow.GetComponent<Rigidbody>();
                if (cowRigidbody != null)
                {
                    cowRigidbody.isKinematic = true;
                }

                levitationAudioSource.UnPause();
            }
        }
    }

    private void LevitateCow()
    {
        Vector3 cowTargetPosition = myTransform.position - new Vector3(0, cowStopDistance, 0);

        currentCow.position = Vector3.MoveTowards(currentCow.position, cowTargetPosition, levitationSpeed * Time.deltaTime);

        float distanceToUFO = Vector3.Distance(currentCow.position, cowTargetPosition);
        if (distanceToUFO <= 0.1f)
        {
            AbsorbCow();
        }
    }

    private void RotateCowRandomly()
    {
        currentCow.rotation = Quaternion.RotateTowards(currentCow.rotation, targetRotation, rotationSpeedDuringLevitation * Time.deltaTime);

        if (Quaternion.Angle(currentCow.rotation, targetRotation) < 1f)
        {
            SetRandomCowRotation();
        }
    }

    private void SetRandomCowRotation()
    {
        targetRotation = Random.rotation;
    }

    private void AbsorbCow()
    {
        //TODO Particles

        Destroy(currentCow.gameObject);

        isCowLevitating = false;
        currentCow = null;
        target = null;

        levitationAudioSource.Pause();

        FindNextCow();
    }

    private IEnumerator DropCow()
    {
        isCowLevitating = false;

        yield return new WaitForSeconds(1f);

        Rigidbody cowRigidbody = currentCow.GetComponent<Rigidbody>();
        if (cowRigidbody != null)
        {
            cowRigidbody.isKinematic = false;
        }

        currentCow = null;

        FindNextCow();
    }

    private void FindNextCow()
    {
        GameObject[] cows = GameObject.FindGameObjectsWithTag("Cow");
        if (cows.Length > 0)
        {
            do
            {
                target = cows[Random.Range(0, cows.Length)].transform;
            } while (target == null && cows.Length > 0);
        }
        else
        {
            target = null;
        }
    }
}
