using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	Rigidbody rb;
    Gyroscope gyro;

    private GameObject cameraContainer;
    private Quaternion rot;

    // Start is called before the first frame update
    void Start()
    {
        cameraContainer = new GameObject("Camera Container");
        cameraContainer.transform.position = transform.position;
        transform.SetParent(cameraContainer.transform);

		rb = GetComponent<Rigidbody>();
        EnableGyro();
    }

    // Update is called once per frame
    void Update()
    {
        if (gyro.enabled)
            transform.localRotation = gyro.attitude * rot;
    }

    private bool EnableGyro()
	{
        if (SystemInfo.supportsGyroscope)
		{
            gyro = Input.gyro;
            gyro.enabled = true;

            cameraContainer.transform.rotation = Quaternion.Euler(90f, 90f, 0);
            rot = new Quaternion(0, 0, 1, 0);
            return true;
		}

        return false;
	}
}
