using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputs : MonoBehaviour
{
    [SerializeField] float horSpeed;
    [SerializeField] float vertSpeed;

    [SerializeField] float moveHor;
    [SerializeField] float moveVert;

    [SerializeField] float movingSlow;
    [SerializeField] float maxSpeed;
    [SerializeField] float initialMoveSpeed;

    [SerializeField] GameObject demon;
    public Vector3 spawnPoint;

    bool moveFastReady = true;

    [SerializeField] GameObject stick;

    Vector3 moveForce;

    Rigidbody rb;

    bool mouseHeld = false;

    MeshPlusPlus activeMesh;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        moveHor = Input.GetAxis("Horizontal");
        moveVert = Input.GetAxis("Vertical");

        //if (Input.GetKey(KeyCode.R))
        //{
        //    stick.GetComponent<DrawingMesh>().CreateNewMeshStroke();
        //}
        //if (Input.GetKeyDown(KeyCode.G))
        //{
        //    GestureManager.Instance.TestRecognizer();
        //}

        if (Input.GetKeyDown(KeyCode.H))
        {
            activeMesh = GestureManager.Instance.FindClosestStroke(transform.position);
            if (activeMesh != null)
                GestureManager.Instance.CombineCloseStrokes(activeMesh);
            else
                Debug.LogError("No meshes found");
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            Instantiate(demon, spawnPoint, Quaternion.identity);
        }

        if (Input.GetMouseButton(1))
        {
            if (!mouseHeld)
                stick.GetComponent<DrawingMesh>().CreateNewMeshStroke();

            RaycastHit hit;
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);
            stick.GetComponent<DrawingMesh>().AddPointToMesh(hit);

            mouseHeld = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            stick.GetComponent<DrawingMesh>().AddToMeshList();
            mouseHeld = false;
        }

        float scrollMod = -1;
        float clippingMod = 0.5f;

        if (Camera.main.nearClipPlane + Input.mouseScrollDelta.y * scrollMod * clippingMod > 0)
        {
            Camera.main.transform.position += new Vector3(0, Input.mouseScrollDelta.y * scrollMod, Input.mouseScrollDelta.y * -0.75f * scrollMod);
            Camera.main.nearClipPlane += Input.mouseScrollDelta.y * scrollMod * clippingMod;
        }
    }

    private void FixedUpdate()
    {
        //if (rb.velocity.magnitude < maxSpeed)
        //    rb.AddForce((new Vector3(moveHor * horSpeed, 0, moveVert * vertSpeed)));

        Debug.Log(rb.velocity.magnitude);

        //if (rb.velocity.magnitude < 0)
        //{
        //    moveFastReady = true;
        //}

        //if ( moveFastReady)
        //    rb.AddForce((new Vector3(moveHor * initialMoveSpeed, 0, moveVert * initialMoveSpeed)), ForceMode.VelocityChange);

        rb.velocity = new Vector3(moveHor * horSpeed, 0 , moveVert * vertSpeed);
    }
}
