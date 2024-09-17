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
    private float maxDistance = 2f;

    private Transform myTransform; 
    private Vector3 lastPosition;
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

            Vector3 direction = (new Vector3(target.position.x, myTransform.position.y, target.position.z) - myTransform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, lookRotation, rotationSpeed * Time.deltaTime);

            float distanceToTarget = Vector3.Distance(new Vector3(target.position.x, 0, target.position.z), new Vector3(myTransform.position.x, 0, myTransform.position.z));

            if (Vector3.Distance(new Vector3(target.position.x, 0, target.position.z), new Vector3(myTransform.position.x, 0, myTransform.position.z)) > maxDistance)
            {
                myTransform.position = Vector3.MoveTowards(myTransform.position, targetPosition, moveSpeed * Time.deltaTime);
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
    }
}
