using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BallHandler : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Rigidbody2D pivot;
    [SerializeField] private float detachDelay = 0.15f;
    [SerializeField] private float respawnDelay = 1f;

    private Rigidbody2D currentBallRigidbody;
    private SpringJoint2D currentBallSpringJoint;

    private Camera mainCamera;
    private bool isDragging;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;

        SpawnNewBall();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentBallRigidbody == null) { return; }


        if (!Touchscreen.current.primaryTouch.press.isPressed)
        {
            if (isDragging)
            {
                LaunchBall();
            }
            
            isDragging = false;
            
            return; // If we are not touching the touch screen, then return.
        }

        isDragging = true;
        currentBallRigidbody.isKinematic = true;

        Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue(); // Read the position of the touch on the screen and return Vector2 value.

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(touchPosition);

        currentBallRigidbody.position = worldPosition;
    }

    private void SpawnNewBall()
    {
        GameObject ballInstance = Instantiate(ballPrefab, pivot.position, Quaternion.identity);

        currentBallRigidbody = ballInstance.GetComponent<Rigidbody2D>();
        currentBallSpringJoint = ballInstance.GetComponent<SpringJoint2D>();

        currentBallSpringJoint.connectedBody = pivot;
    }

    private void LaunchBall()
    {
        currentBallRigidbody.isKinematic = false;
        currentBallRigidbody = null;

        Invoke(nameof(DetachBall), detachDelay);
    }

    private void DetachBall()
    {
        currentBallSpringJoint.enabled = false;
        currentBallSpringJoint = null;

        Invoke(nameof(SpawnNewBall), respawnDelay);
    }
}