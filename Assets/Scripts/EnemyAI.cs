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
    private Cow cow;
    private bool isCowLevitating = false;
    private Vector3 lastPosition;
    private Quaternion targetRotation;

    [Header("Audio Sources")]
    [SerializeField]
    private AudioSource moveAudioSource;
    [SerializeField]
    private AudioSource levitationAudioSource;

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
        if (target == null)
        {
            FindNextCow();
        }
    }

    private void HandleMovement()
    {
        float terrainHeightAtCurrentPosition = Terrain.activeTerrain.SampleHeight(myTransform.position);
        float desiredY = terrainHeightAtCurrentPosition + hoverHeight;

        if (isCowLevitating)
        {
            myTransform.position = new Vector3(myTransform.position.x, desiredY, myTransform.position.z);
            return;
        }

        Vector3 direction = (new Vector3(target.position.x, 0, target.position.z) - new Vector3(myTransform.position.x, 0, myTransform.position.z)).normalized;
        float distanceToTarget = Vector3.Distance(new Vector3(target.position.x, 0, target.position.z), new Vector3(myTransform.position.x, 0, myTransform.position.z));

        if (distanceToTarget > maxDistance + 0.01f)
        {
            Vector3 horizontalMove = direction * moveSpeed * Time.deltaTime;
            myTransform.position += new Vector3(horizontalMove.x, 0, horizontalMove.z);

            float smoothedY = Mathf.Lerp(myTransform.position.y, desiredY, Time.deltaTime * moveSpeed);
            myTransform.position = new Vector3(myTransform.position.x, smoothedY, myTransform.position.z);
        }
        else
        {
            myTransform.position = new Vector3(myTransform.position.x, desiredY, myTransform.position.z);
            CheckForCowBelow();
        }

        HandleRotation();
        HandleMoveAudio();

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

    private void HandleMoveAudio()
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

                HandleLevitationAudio();
            }
        }
    }

    private void HandleLevitationAudio()
    {
        levitationAudioSource.UnPause();
        cow = currentCow.GetComponent<Cow>();
        cow.StartMooSound();
    }

    private void LevitateCow()
    {
        float terrainHeightAtCurrentPosition = Terrain.activeTerrain.SampleHeight(myTransform.position);
        float desiredY = terrainHeightAtCurrentPosition + hoverHeight;
        myTransform.position = new Vector3(myTransform.position.x, desiredY, myTransform.position.z);

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
        if (currentCow == null)
        {
            return;
        }
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
        Debug.Log("Absorbing Cow: " + currentCow.name);
        //TODO Particles

        if (cow != null)
        {
            cow.IsBeingTargeted = false;
        }

        if (cow != null)
        {
            cow.StopMooSound();
        }

        if (cow != null)
        {
            CowManager.Instance.UnregisterCow(cow);
        }

        if (currentCow != null)
        {
            Destroy(currentCow.gameObject);
        }

        isCowLevitating = false;
        currentCow = null;
        cow = null;
        target = null;

        if (levitationAudioSource != null)
        {
            levitationAudioSource.Pause();
        }

        FindNextCow();
    }

    private void FindNextCow()
    {
        List<Cow> cows = new List<Cow>(CowManager.Instance.GetCows());
        Debug.Log("EnemyAI: Total Cows: " + cows.Count);

        if (cows.Count > 0)
        {
            Cow selectedCow = null;

            foreach (Cow cow in cows)
            {
                Debug.Log($"Cow {cow.name} IsBeingTargeted: {cow.IsBeingTargeted}");
            }

            do
            {
                int randomIndex = Random.Range(0, cows.Count);
                Cow randomCow = cows[randomIndex];

                if (!randomCow.IsBeingTargeted)
                {
                    Debug.Log("EnemyAI: Targeting Cow: " + randomCow.name);
                    selectedCow = randomCow;
                    selectedCow.IsBeingTargeted = true;
                    break;
                }

                cows.RemoveAt(randomIndex);
            } while (cows.Count > 0);

            if (selectedCow != null)
            {
                target = selectedCow.transform;
                cow = selectedCow;
                Debug.Log("New Target Cow: " + target.name);
            }
            else
            {
                Debug.Log("No untargeted cows available.");
                target = null;
                cow = null;
            }
        }
        else
        {
            Debug.Log("No cows available.");
            target = null;
            cow = null;
        }
    }

    public void DropCow()
    {
        Debug.Log("Dropping Cow");

        if (cow == null)
        {
            Debug.LogWarning("Cow is null in DropCow(), attempting to retrieve from target.");
            if (target != null)
            {
                cow = target.GetComponent<Cow>();
            }
        }

        if (cow != null)
        {
            Debug.Log($"Resetting IsBeingTargeted for cow: {cow.name}");
            cow.IsBeingTargeted = false;
            cow.StopMooSound();
        }
        else
        {
            Debug.LogError("unable to reset IsBeingTargeted because cow is still null");
        }

        if (currentCow != null && isCowLevitating)
        {
            Rigidbody cowRigidbody = currentCow.GetComponent<Rigidbody>();
            if (cowRigidbody != null)
            {
                cowRigidbody.isKinematic = false;
                cowRigidbody.useGravity = true;
            }
        }

        isCowLevitating = false;
        currentCow = null;
        cow = null;
        target = null;

        if (levitationAudioSource != null)
        {
            levitationAudioSource.Pause();
        }
    }
}
