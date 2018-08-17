using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script will draw a gizmo line between 2 points
public class DrawGizmo_LineBetween : MonoBehaviour {

    [Header("Draw Settings")]
    [Tooltip("Color of the line to Draw")]
    [SerializeField] Color colorToDraw = Color.yellow;
    [Tooltip("Position to draw from")]
    [SerializeField] Transform position01;
    [Tooltip("Position to draw to")]
    [SerializeField] Transform position02;

    void OnDrawGizmos()
    {
        //Render the color that we specified at the top
        Gizmos.color = colorToDraw;
        //Draw the line from/to the positions of both transforms
        Gizmos.DrawLine(position01.position, position02.position);
    }
}
