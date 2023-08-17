using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(SwipeControllerEditor))]
public class SwipeControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SwipeController component = (SwipeController)target;
        if (component == null)
            return;

        // Set up the parameters for the arc.
        Vector3 position = component.transform.position; // The center position of the arc.
        float radius = 1.0f; // The radius of the arc.
        float startAngle = 0.0f; // Start angle in degrees.
        float endAngle = 180.0f; // End angle in degrees.
        float arcWidth = 0.1f; // The width of the arc.

        // Draw the arc using the DrawArc method.
        DrawArc(position, radius, startAngle, endAngle, arcWidth);
    }

    private void DrawArc(Vector3 position, float radius, float startAngle, float endAngle, float arcWidth)
    {
        int segments = 60;
        float angleStep = (endAngle - startAngle) / segments;
        float currentAngle = startAngle;

        Vector3 prevPoint = Vector3.zero;
        for (int i = 0; i <= segments; i++)
        {
            float x = position.x + Mathf.Cos(currentAngle * Mathf.Deg2Rad) * radius;
            float y = position.y + Mathf.Sin(currentAngle * Mathf.Deg2Rad) * radius;
            Vector3 currentPoint = new Vector3(x, y, position.z);

            if (i > 0)
            {
                
                Handles.DrawSolidArc(position, Vector3.forward, prevPoint - position, angleStep, arcWidth);
            }

            prevPoint = currentPoint;
            currentAngle += angleStep;
        }
    }
}
