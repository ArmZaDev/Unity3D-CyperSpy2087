using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float speed = 12.5f;
    
    //adding Gravity
    public Vector3 veclocity;
    public float gravityModifier;

    public CharacterController myController;
    [SerializeField]private Transform myCameraHead;

    public float mouseSensitivity = 100f;
    private float cameraVerticalRotation;

    public GameObject bullet;
    public Transform firePosition;

    public GameObject muzzleFlash, bulletHole, waterLeak;

    //jumping
    public float jumpHeight = 10f;
    private bool readyToJump;
    public Transform ground;
    public LayerMask groundLayer;
    public float groundDistance = 0.5f;

    //crouching
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 bodyScale;
    public Transform myBody;
    private float initialControllerHeight;
    public float crouchSpeed = 6f;
    private bool isCrouching = false;

    // Start is called before the first frame update
    void Start()
    {
        bodyScale = myBody.localScale;
        initialControllerHeight = myController.height;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
        CameraMovement();
        Shoot();
        Jump();
        Crouching();
    }

    private void Crouching()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            StartCrouching();
        }
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            StopCrouching();
        }
    }

    private void StartCrouching()
    {
        myBody.localScale = crouchScale;
        myCameraHead.position -= new Vector3(0, 1f, 0);
        myController.height /= 2;
        isCrouching = true;
    }

    private void StopCrouching()
    {
        myBody.localScale = bodyScale;
        myCameraHead.position += new Vector3(0, 1f, 0);
        myController.height = initialControllerHeight;
        isCrouching = false;
    }

    void Jump()
    {
        readyToJump = Physics.OverlapSphere(ground.position, groundDistance, groundLayer).Length > 0;

        if (Input.GetButtonDown("Jump") && readyToJump)
        {
            veclocity.y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y) * Time.deltaTime;
        }

        myController.Move(veclocity);
    }

    private void Shoot()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if (Physics.Raycast(myCameraHead.position, myCameraHead.forward, out hit, 100f))
            {
                if (Vector3.Distance(myCameraHead.position, hit.point) > 2f)
                {
                    firePosition.LookAt(hit.point);

                    if (hit.collider.tag == "Shootable")                                     
                        Instantiate(bulletHole, hit.point, Quaternion.LookRotation(hit.normal));                                       
                    
                    if (hit.collider.CompareTag("WaterLeaker"))
                        Instantiate(waterLeak, hit.point, Quaternion.LookRotation(hit.normal));
                }
                
                if (hit.collider.CompareTag("Enemy"))
                {
                    Destroy(hit.collider.gameObject);
                }
            }
            else
            {
                firePosition.LookAt(myCameraHead.position + (myCameraHead.forward * 50f));
            }

            Instantiate(muzzleFlash, firePosition.position, firePosition.rotation, firePosition);
            Instantiate(bullet, firePosition.position, firePosition.rotation);
        }
    }

    private void CameraMovement()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        cameraVerticalRotation -= mouseY;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);

        transform.Rotate(Vector3.up * mouseX);
        myCameraHead.localRotation = Quaternion.Euler(cameraVerticalRotation, 0f, 0f);
    }

    void PlayerMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 movement = x * transform.right + z * transform.forward;

        if (isCrouching)
        {
            movement = movement * crouchSpeed * Time.deltaTime;
        }
        else
        {
            movement = movement * speed * Time.deltaTime;
        }     

        myController.Move(movement);

        veclocity.y += Physics.gravity.y * Mathf.Pow(Time.deltaTime, 2) * gravityModifier;

        if (myController.isGrounded)
        {
            veclocity.y = Physics.gravity.y * Time.deltaTime;
        }

        myController.Move(veclocity);
        
    }
}
