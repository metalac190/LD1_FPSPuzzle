using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This Editor script will use DrawGizmo to shade a Collider without applying a material on it. This allows it to be visible in the editor, but invisible in the game
/// NOTE: The box is drawn based off of the scale on the gameobject. If the Collider is not 1,1,1 inside of collider settings, it will appear off.
/// </summary>

[DisallowMultipleComponent]                         //Prevent multiple copies of this component to be added to the same gameObject

public class DrawGizmo_ShadeCollider : MonoBehaviour {

    [Header("Draw Settings")]
    [Tooltip("Color to draw the box")]
    [SerializeField] Color colorToDraw = Color.yellow;

    /// <summary>
    /// Draw a visualization of our trigger so that we can see it.
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.color = colorToDraw;
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}
