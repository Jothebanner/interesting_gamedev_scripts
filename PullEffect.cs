using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PullEffect : MonoBehaviour
{
    public GameObject cube;

    // Better to reference those already in the Inspector
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MeshCollider meshCollider;

    private Mesh _mesh;

    private void Awake()
    {
        if (!meshFilter) meshFilter = GetComponent<MeshFilter>();
        if (!meshRenderer) meshRenderer = GetComponent<MeshRenderer>();
        if (!meshCollider) meshCollider = GetComponent<MeshCollider>();

        _mesh = meshFilter.mesh;

        // create new colors array where the colors will be created
        var colors = new Color[_mesh.vertices.Length];
        for (var k = 0; k < colors.Length; k++)
        {
            colors[k] = Color.white;
        }
        _mesh.colors = colors;
    }

    private void Update()
    {
        _mesh = meshFilter.mesh;

        _mesh.RecalculateNormals();

        if (!Input.GetMouseButtonDown(0)) return;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit))
        {
            Debug.Log(hit.triangleIndex);
            //cube.transform.position = hit.point;

            // Get current vertices, triangles and colors
            var vertices = _mesh.vertices;
            var triangles = _mesh.triangles;
            var colors = _mesh.colors;

            // Get the vert indices for this triangle
            var vert1Index = triangles[hit.triangleIndex * 3 + 0];
            var vert2Index = triangles[hit.triangleIndex * 3 + 1];
            var vert3Index = triangles[hit.triangleIndex * 3 + 2];

            // Get the positions for the vertices
            var vert1Pos = vertices[vert1Index];
            var vert2Pos = vertices[vert2Index];
            var vert3Pos = vertices[vert3Index];

            // Now for all three vertices we first check if any other triangle if using it
            // by simply count how often the indices are used in the triangles list
            var vert1Occurrences = 0;
            var vert2Occurrences = 0;
            var vert3Occurrences = 0;
            foreach (var index in triangles)
            {
                if (index == vert1Index) vert1Occurrences++;
                else if (index == vert2Index) vert2Occurrences++;
                else if (index == vert3Index) vert3Occurrences++;
            }

            // Create copied Lists so we can dynamically add entries
            var newVertices = vertices.ToList();
            var newColors = colors.ToList();

            // Now if a vertex is shared we need to add a new individual vertex
            // and also an according entry for the color array
            // and update the vertex index
            // otherwise we will simply use the vertex we already have
            if (vert1Occurrences > 1)
            {
                newVertices.Add(vert1Pos);
                newColors.Add(new Color());
                vert1Index = newVertices.Count - 1;
            }

            if (vert2Occurrences > 1)
            {
                newVertices.Add(vert2Pos);
                newColors.Add(new Color());
                vert2Index = newVertices.Count - 1;
            }

            if (vert3Occurrences > 1)
            {
                newVertices.Add(vert3Pos);
                newColors.Add(new Color());
                vert3Index = newVertices.Count - 1;
            }

            // Update the indices of the hit triangle to use the (eventually) new
            // vertices instead
            triangles[hit.triangleIndex * 3 + 0] = vert1Index;
            triangles[hit.triangleIndex * 3 + 1] = vert2Index;
            triangles[hit.triangleIndex * 3 + 2] = vert3Index;

            // color these vertices
            newColors[vert1Index] = Color.blue;
            newColors[vert2Index] = Color.blue;
            newColors[vert3Index] = Color.blue;

            var vertArray = newVertices.ToArray();

            // add height to vertices for coolness
   //         vertArray[vert1Index].y -= 1f;
			//vertArray[vert2Index].y -= 1f;
			//vertArray[vert3Index].y -= 1f;


			// write everything back

			// somehow this makes them not share colors...  idk man

			_mesh.vertices = vertArray;
            _mesh.triangles = triangles;
            _mesh.colors = newColors.ToArray();

            _mesh.RecalculateNormals();

            GetComponent<MeshCollider>().sharedMesh = _mesh;
        }
        else
        {
            Debug.Log("no hit");
        }
    }
}

