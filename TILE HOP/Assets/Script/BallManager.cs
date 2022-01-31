using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BallManager : MonoBehaviour
{
    internal Transform target;

    [SerializeField]
    float movementSensivity = 5;

    //Events
    public event Action callForNextTarget;

    Rigidbody rigid;
    Material deactiveTileColor_face;
    bool isInCollision = false;

    //Touch
    private Touch touch;
    [SerializeField]
    private float touchSpeed = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        rigid = GetComponent<Rigidbody>();
        deactiveTileColor_face = FindObjectOfType<GameManager>().getDeactiveTilesColour();
        FindObjectOfType<GameManager>().UpdateTarget += SetTarget;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && GameManager.isBallRunning) {
            touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved) {
                transform.position = new Vector3(
                    Mathf.Clamp(transform.position.x + touch.deltaPosition.x * touchSpeed, -1, 1),
                    transform.position.y, transform.position.z);    
            }
        }

        if (Input.GetKey(KeyCode.LeftArrow) && GameManager.isBallRunning) 
        { 
            transform.position = new Vector3(Mathf.Clamp(transform.position.x - Time.deltaTime * movementSensivity, -1, 1), transform.position.y, transform.position.z);
        }
        if (Input.GetKey(KeyCode.RightArrow) && GameManager.isBallRunning)
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x + Time.deltaTime * movementSensivity, -1, 1), transform.position.y, transform.position.z);
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Tile") && !isInCollision && GameManager.isBallRunning) {
            isInCollision = true;
            collision.transform.GetChild(2).GetComponent<MeshRenderer>().material = deactiveTileColor_face;
            collision.transform.GetChild(3).GetChild(0).GetComponent<ParticleSystem>().Play();
            collision.gameObject.GetComponent<Animator>().enabled = true;
            callForNextTarget?.Invoke();

            Debug.Log("Check");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Tile") && isInCollision)
        {
            isInCollision = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7) {
            GameManager.isBallRunning = false;
            GameManager.isGameOver = true;
            FindObjectOfType<camFollow>().isMoving = false;
            FindObjectOfType<UIManager>().showRestartAndQuiteButton();
        }

        if (other.gameObject.layer == 8)
        {
            other.gameObject.SetActive(false);
            FindObjectOfType<GameManager>().setDiamond();
        }
    }

    void SetTarget(Transform target, float individual_angle) {
        this.target = target;
        jump(individual_angle);
    }

    void jump(float individual_angle) {
        rigid.useGravity = true;
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        
        Vector3 p = target.position;

        float gravity = Physics.gravity.magnitude;

        // Selected angle in radians
        float angle = individual_angle * Mathf.Deg2Rad;

        // Positions of this object and the target on the same plane
        Vector3 planarTarget = new Vector3(p.x, 0, p.z);
        Vector3 planarPostion = new Vector3(transform.position.x, 0, transform.position.z);

        // Planar distance between objects
        float distance = Vector3.Distance(planarTarget, planarPostion);

        // Distance along the y axis between objects
        float yOffset = transform.position.y - p.y;

        float initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));
        Vector3 velocity = new Vector3(0, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));

        // Rotate velocity to match the direction between the two objects
        //float angleBetweenObjects = Vector3.Angle(Vector3.forward, planarTarget - planarPostion);
        //Vector3 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;

        // Fire!
        rigid.velocity = velocity;

        // Alternative way:
        //rigid.AddForce(finalVelocity * rigid.mass, ForceMode.Impulse);
    }
}
