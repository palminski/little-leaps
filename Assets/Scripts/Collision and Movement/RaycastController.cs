using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour
{

[HideInInspector]
    public BoxCollider2D boxCollider;

[HideInInspector]
    public float verticalRaySpacing;

[HideInInspector]
    public float horizontalRaySpacing;

[HideInInspector]
    public RaycastOrigins raycastOrigins;

    // [SerializeField]
    // public Vector3 velocity;
    [Header("Raycast Settings")]
    [SerializeField]
    public int xRayCount = 4;

    [SerializeField]
    public int yRayCount = 4;

    [SerializeField]
    public float skinWidth = 0.001f;

    [SerializeField]
    public LayerMask collidableLayers;


 //Calculating Spacing and Locations for Rays to Be Cast
    //====================================================================================================

        public virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        updateRaySpacing();
    }

    public void updateRaycastOrigins()
    {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
    }

    public void addToRaycastOriginsX(float ammountToAdd) {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.topRight = new Vector2(bounds.max.x + ammountToAdd, bounds.max.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x+ ammountToAdd, bounds.max.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x+ ammountToAdd, bounds.min.y);
        raycastOrigins.bottomLeft = new Vector2(bounds.min.x+ ammountToAdd, bounds.min.y);
    }

    public void updateRaySpacing()
    {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(skinWidth * -2);
        //We need at least 2 rays per side
        xRayCount = Mathf.Clamp(xRayCount, 2, int.MaxValue);
        yRayCount = Mathf.Clamp(yRayCount, 2, int.MaxValue);

        verticalRaySpacing = bounds.size.x / (xRayCount - 1);
        horizontalRaySpacing = bounds.size.y / (yRayCount - 1);
    }

    //Structs
    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

}
