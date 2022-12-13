using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintMaker : MonoBehaviour
{

    [SerializeField] GameObject paintCube;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);
        if (Input.GetMouseButton(0))
        {
            paintCube.GetComponent<RTPaintBrush>().Paint(hit);
        }
    }
}
