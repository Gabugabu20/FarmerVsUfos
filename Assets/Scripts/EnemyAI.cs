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
    private float cowStopHeight = 1f;

    [SerializeField]
    private float rotationSpeedDuringLevitation = 10f;

    private Transform myTransform;
    private Transform currentCow;
    [SerializeField] private bool isCowLevitating = false;
    private Vector3 lastPosition;
    private Quaternion targetRotation;
    private AudioSource moveAudioSource;

    void Awake()
    {
        myTransform = transform;
        lastPosition = myTransform.position;
        moveAudioSource = GetComponent<AudioSource>();
        moveAudioSource.Play();
        moveAudioSource.Pause();
    }

    void Start()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Cow");
        if (go != null)
        {
            target = go.transform;
        }
    }

    void Update()
    {
        if (target != null)
        {
            Debug.DrawLine(target.position, myTransform.position, Color.red);

            Vector3 targetPosition = new Vector3(target.position.x, target.position.y + hoverHeight, target.position.z);

            Vector3 direction = (new Vector3(target.position.x, myTransform.position.y, target.position.z) - myTransform.position);

            if (!isCowLevitating && direction.sqrMagnitude > 0.001f)
            {
                direction.Normalize();
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                myTransform.rotation = Quaternion.Slerp(myTransform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
            }

            float distanceToTarget = Vector3.Distance(new Vector3(target.position.x, 0, target.position.z), new Vector3(myTransform.position.x, 0, myTransform.position.z));

            if (!isCowLevitating)
            {
                if (distanceToTarget > maxDistance + 0.01f)
                {
                    myTransform.position = Vector3.MoveTowards(myTransform.position, targetPosition, moveSpeed * Time.deltaTime);
                }
                else
                {
                    myTransform.position = targetPosition;
                    if (!isCowLevitating)
                    {
                        CheckForCowBelow();
                    }
                }
            }

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

            lastPosition = myTransform.position;
        }
        if (isCowLevitating && currentCow != null)
        {
            LevitateCow();
            RotateCowRandomly();
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
            }
        }
    }

    private void LevitateCow()
    {
        currentCow.position = Vector3.MoveTowards(currentCow.position, myTransform.position - new Vector3(0, cowStopHeight, 0), levitationSpeed * Time.deltaTime);

        if (Vector3.Distance(currentCow.position, myTransform.position - new Vector3(0, cowStopHeight, 0)) < 0.1f)
        {
            StartCoroutine(DropCow());
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
            foreach (GameObject cow in cows)
            {
                if (cow != null && cow != currentCow)
                {
                    target = cow.transform;
                    break;
                }
            }
        }
        else
        {
            target = null;
        }
    }

}
