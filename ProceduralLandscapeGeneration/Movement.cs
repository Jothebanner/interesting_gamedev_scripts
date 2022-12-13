using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private Camera cam;

    [SerializeField] float moveSpeed = 2000f;
    [SerializeField] float dragModifier = 0.1f;
    [SerializeField] private float lookSensitivity = 7f;
    float editorSetDragModifier;

    [SerializeField] float targetSpeed = 20f;
    Rigidbody rb;

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private Vector3 cameraRotation = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        editorSetDragModifier = dragModifier;
    }

    // Update is called once per frame
    void Update()
    {
        float moveHor = Input.GetAxisRaw("Horizontal");
        float moveVert = Input.GetAxisRaw("Vertical");

        Vector3 _movHorizontal = transform.right * moveHor;
        Vector3 _movVertical = transform.forward * moveVert;

        velocity = (_movHorizontal + _movVertical).normalized;

        // Calculate rotation as a 3D vector
        float _yRot = Input.GetAxisRaw("Mouse X");

        Vector3 _rotation = new Vector3(0f, _yRot, 0f) * lookSensitivity;

        // Apply rotation
        Rotate(_rotation);

        float _xRot = Input.GetAxisRaw("Mouse Y");

        Vector3 _cameraRotation = new Vector3(_xRot, 0f, 0f) * lookSensitivity;

        //Apply camera rotation
        RotateCamera(_cameraRotation);
    }

	private void FixedUpdate()
	{
        //Debug.Log(velocity.normalized);
        Debug.Log(Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z));

        // if above the target speed increase drag to slow down player
        if (Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z) > targetSpeed)
            dragModifier = 1.1f;
        else
            dragModifier = editorSetDragModifier;

        // how quickly to stop if input axes are 0
        if (velocity.magnitude == 0)
		{
            dragModifier = 2f;
		}

        // velocitychange to ignore weight
        rb.AddForce(velocity * moveSpeed * Time.deltaTime, ForceMode.VelocityChange);

        // add drag for x and z so we don't effect gravity
        if (rb.velocity.magnitude > 0 )
		{
            if (Mathf.Abs(rb.velocity.x) > 0)
                rb.AddForce(new Vector3(-rb.velocity.x ,0f, 0f) * dragModifier * Time.deltaTime, ForceMode.Impulse);
            if (Mathf.Abs(rb.velocity.z) > 0)
                rb.AddForce(new Vector3(0f, 0f, -rb.velocity.z) * dragModifier * Time.deltaTime, ForceMode.Impulse);
		}
	}

	private void LateUpdate()
	{
        PerformCameraRotation();
        PerformRotation();
    }

    public void Rotate(Vector3 _rotation)
    {
        rotation = _rotation;
    }

    // gets a rotational vector for the camera
    public void RotateCamera(Vector3 _cameraRotation)
    {
        cameraRotation = _cameraRotation;
    }

    void PerformRotation()
    {
        if (rotation != Vector3.zero)
        {
            // calculates where it wants to go before it go so it's really good for networked movement
            rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));

        }
    }

    void PerformCameraRotation()
    {
        if (cam != null)
        {
            cam.transform.Rotate(-cameraRotation);
        }
    }
}
