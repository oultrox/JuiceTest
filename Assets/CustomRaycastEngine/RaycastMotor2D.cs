using UnityEngine;

namespace CustomRaycastEngine
{
    /// <summary>
    /// Raycast-based motor for simulated physics.
    /// </summary>
    public class RaycastMotor2D
    {
        public const float SKIN_WIDTH = 0.015f;
        public const float DISTANCE_BETWEEN_RAYS = 0.08f;
        public const float HORIZONTAL_RAY_VERTICAL_PADDING = 0.1f;
        public const float VERTICAL_RAY_HORIZONTAL_PADDING = 0.2f;
        protected BoxCollider2D collider;
        protected Bounds bounds;
        protected RaycastOrigin raycastOrigin;

        
        public int HorizontalRayCount { get; private set; }
        public int VerticalRayCount { get; private set; }
        public float HorizontalRaySpacing { get; private set; }
        public float VerticalRaySpacing { get; private set; }

        protected struct RaycastOrigin
        {
            public Vector2 topLeft, topRight;
            public Vector2 bottomLeft, bottomRight;
        }

        public RaycastMotor2D(BoxCollider2D collider)
        {
            this.collider = collider;
            CalculateRaySpacing();
            UpdateRaycastOrigins();
        }

        public void UpdateRaycastOrigins()
        {
            bounds = collider.bounds;
            bounds.Expand(SKIN_WIDTH * -3);

            raycastOrigin.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
            raycastOrigin.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
            raycastOrigin.topLeft = new Vector2(bounds.min.x, bounds.max.y);
            raycastOrigin.topRight = new Vector2(bounds.max.x, bounds.max.y);
        }

        void CalculateRaySpacing()
        {
            bounds = collider.bounds;
            bounds.Expand(SKIN_WIDTH * -2);

            float adjustedHeight = bounds.size.y - (HORIZONTAL_RAY_VERTICAL_PADDING * 2);
            HorizontalRayCount = Mathf.RoundToInt(adjustedHeight / DISTANCE_BETWEEN_RAYS);
            VerticalRayCount = Mathf.RoundToInt(bounds.size.x / DISTANCE_BETWEEN_RAYS);

            HorizontalRaySpacing = adjustedHeight / (HorizontalRayCount - 1);
            VerticalRaySpacing = bounds.size.x / (VerticalRayCount - 1);
        }

        public Vector2 GetHorizontalRayOrigin(bool leftSide, int index)
        {
            float yOffset = HORIZONTAL_RAY_VERTICAL_PADDING + HorizontalRaySpacing * index;
            return (leftSide ? raycastOrigin.bottomLeft : raycastOrigin.bottomRight) + Vector2.up * yOffset;
        }

        public Vector2 GetVerticalRayOrigin(bool bottomSide, int index)
        {
            float usableWidth = bounds.size.x - VERTICAL_RAY_HORIZONTAL_PADDING * 2;
            float xOffset = VERTICAL_RAY_HORIZONTAL_PADDING + (usableWidth / (VerticalRayCount - 1)) * index;
            return (bottomSide ? raycastOrigin.bottomLeft : raycastOrigin.topLeft) + Vector2.right * xOffset;
        }
    }
}
