using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGizmo : MonoBehaviour {

    public float xSize = 1;     //X value gizmo size
    public float ySize = 1;     //Y value gizmo size
    public float zSize = 1;     //Z value gizmo size
    public Color gizmoColor;    //Color of gizmo wireframe

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireCube(transform.position, new Vector3(xSize, ySize, zSize));
    }
}
