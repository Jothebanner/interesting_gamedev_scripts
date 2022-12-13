using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicCircle
{

    private MeshPlusPlus[] meshStrokes;
    public MagicCircleBoundaries MCBoundaries;
    public Vector3 CirlceCenter { get; set; }

    public MagicCircle(MeshPlusPlus[] strokes)
    {
        meshStrokes = strokes;
        CalculateBoundaries();
        GetCenterOfCircle();
        CalculateCornerBoundaries();
        CreatePhysicalBoundries();
    }

    private void CalculateBoundaries()
    {
        Vector3 upper = Vector3.negativeInfinity;
        Vector3 lower = Vector3.positiveInfinity;
        Vector3 left = Vector3.positiveInfinity;
        Vector3 right = Vector3.negativeInfinity; 

        foreach (MeshPlusPlus stroke in meshStrokes)
        {
            foreach (Vector3 point in stroke.strokePoints)
            {
                if (point.z > upper.z)
                    upper = point;

                if (point.z < lower.z)
                    lower = point;

                if (point.x < left.x)
                    left = point;

                if (point.x > right.x)
                    right = point;
            }
        }

        MCBoundaries = new MagicCircleBoundaries(upper, lower, left, right);
    }

    private void CalculateCornerBoundaries()
    {
        Vector3 upperLeft = Vector3.zero;
        Vector3 upperRight = Vector3.zero;
        Vector3 lowerLeft = Vector3.zero;
        Vector3 lowerRight = Vector3.zero;


        float upperLeftScore = 0;
        float upperRightScore = 0;
        float lowerLeftScore = 0;
        float lowerRightScore = 0;

        foreach (MeshPlusPlus stroke in meshStrokes)
        {
            foreach (Vector3 point in stroke.strokePoints)
            {
                float currentUpperLeftScore;
                float currentUpperRightScore;
                float currentLowerLeftScore;
                float currentLowerRightScore;

                float absX = Mathf.Abs(point.x - CirlceCenter.x);
                float absZ = Mathf.Abs(point.z - CirlceCenter.z);
                float xDistanceFromCenter = point.x - CirlceCenter.x;
                float zDistanceFromCenter = point.z - CirlceCenter.z;

                if (xDistanceFromCenter < 0 && zDistanceFromCenter > 0)
                {

                    currentUpperLeftScore = absX + absZ;

                    if (currentUpperLeftScore > upperLeftScore)
                    {
                        upperLeftScore = currentUpperLeftScore;
                        upperLeft = point;
                    }
                }

                /// Potential for futher accuracy
                /// get the difference between the x and z and the smallest difference is probably the most corner
                /// IDK tho

                // upper left
                //if (xDistanceFromCenter < 0 && point.z - CirlceCenter.z > 0)
                //{
                //    Debug.Log(x);
                //    Debug.Log(z);
                //    currentUpperLeft = x > z ? x / z : z / x;
                //    Debug.Log(currentUpperLeft);
                //    if (currentUpperLeft + x + z > upperLeftScore)
                //    {
                //        upperLeft = point;
                //        upperLeftScore = currentUpperLeft + x + z;
                //        Debug.Log("score: " + upperLeftScore);
                //    }
                //}

                // upper right
                if (xDistanceFromCenter > 0 && zDistanceFromCenter > 0)
                {
                    currentUpperRightScore = absX + absZ;

                    if (currentUpperRightScore > upperRightScore)
                    {
                        upperRightScore = currentUpperRightScore;
                        upperRight = point;
                    }
                }

                // bottom left
                if (xDistanceFromCenter < 0 && zDistanceFromCenter < 0)
                {
                    currentLowerLeftScore = absX + absZ;

                    if (currentLowerLeftScore > lowerLeftScore)
                    {
                        lowerLeftScore = currentLowerLeftScore;
                        lowerLeft = point;
                    }
                }

                // bottom right
                if (xDistanceFromCenter > 0 && zDistanceFromCenter < 0)
                {
                    currentLowerRightScore = absX + absZ;

                    if (currentLowerRightScore > lowerRightScore)
                    {
                        lowerRightScore = currentLowerRightScore;
                        lowerRight = point;
                    }
                }
            }
        }

        //GestureManager.Instance.MakeCube(upperLeft, "UpperLeft", Color.gray);
        //GestureManager.Instance.MakeCube(upperRight, "UpperRight", Color.gray);
        //GestureManager.Instance.MakeCube(lowerLeft, "lowerLeft", Color.gray);
        //GestureManager.Instance.MakeCube(lowerRight, "LowerRight", Color.gray);

        MCBoundaries.UpperLeft = upperLeft;
        MCBoundaries.UpperRight = upperRight;
        MCBoundaries.LowerLeft = lowerLeft;
        MCBoundaries.LowerRight = lowerRight;

    }

    // uhhh
    void GetCenterOfCircle()
    {
        Vector3 center = Vector3.zero;
        float heightSigned;
        float widthSigned;
        float centerVerticle;
        float centerHor;

        heightSigned = MCBoundaries.Upper.z - MCBoundaries.Lower.z;
        widthSigned = MCBoundaries.Right.x - MCBoundaries.Left.x;

        centerVerticle = MCBoundaries.Upper.z - (heightSigned / 2);
        centerHor = MCBoundaries.Right.x - (widthSigned / 2);

        CirlceCenter = new Vector3(centerHor, 0, centerVerticle);

    }

    void CreatePhysicalBoundries()
    {
        SynthesizePhysicalBoundary(MCBoundaries.Upper, MCBoundaries.UpperRight);
        SynthesizePhysicalBoundary(MCBoundaries.UpperRight, MCBoundaries.Right);
        SynthesizePhysicalBoundary(MCBoundaries.Right, MCBoundaries.LowerRight);
        SynthesizePhysicalBoundary(MCBoundaries.LowerRight, MCBoundaries.Lower);
        SynthesizePhysicalBoundary(MCBoundaries.Lower, MCBoundaries.LowerLeft);
        SynthesizePhysicalBoundary(MCBoundaries.LowerLeft, MCBoundaries.Left);
        SynthesizePhysicalBoundary(MCBoundaries.Left, MCBoundaries.UpperLeft);
        SynthesizePhysicalBoundary(MCBoundaries.UpperLeft, MCBoundaries.Upper);
    }

    void SynthesizePhysicalBoundary(Vector3 pointOne, Vector3 pointTwo)
    {
        var width = Vector3.Distance(pointOne, pointTwo);
        Vector3 position = Vector3.Lerp(pointOne, pointTwo, 0.5f);
        GameObject cube = GestureManager.Instance.MakeCube(position);
        cube.transform.LookAt(CirlceCenter);
        cube.transform.localScale = new Vector3(width, 10, 0.3f);
    }

}



public struct MagicCircleBoundaries
{ 
    public MagicCircleBoundaries(Vector3 upper, Vector3 lower, Vector3 left, Vector3 right, Vector3 upperLeft = default(Vector3), Vector3 upperRight = default(Vector3), Vector3 lowerLeft = default(Vector3), Vector3 lowerRight = default(Vector3))
    {
        Upper = upper;
        Lower = lower;
        Left = left;
        Right = right;
        UpperLeft = upperLeft;
        UpperRight = upperRight;
        LowerLeft = lowerLeft;
        LowerRight = lowerRight;

    }

    public Vector3 Upper { get; }
    public Vector3 Lower { get; }
    public Vector3 Left { get; }
    public Vector3 Right { get; }
    public Vector3 UpperLeft { get; set; }
    public Vector3 UpperRight { get; set; }
    public Vector3 LowerLeft { get; set; }
    public Vector3 LowerRight { get; set; }
}
