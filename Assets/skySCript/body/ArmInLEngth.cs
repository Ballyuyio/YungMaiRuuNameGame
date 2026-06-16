using UnityEngine;

public class RigidArmIK_YAxis : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The main center body object the arm circles around.")]
    public Transform centerObject;

    [Header("Constraint Settings")]
    [Tooltip("The fixed distance from the center object where the arm's pivot sits.")]
    public float orbitRadius = 4f;

    [Tooltip("Flips the Y-axis 180 degrees if the arm is pointing inside-out.")]
    public bool invertDirection = false;

    [Header("Gizmos")]
    public bool showOrbitCircle = true;
    public Color circleColor = Color.cyan;

    void LateUpdate()
    {
        if (centerObject == null) return;
        Vector3 directionFromCenter = transform.position - centerObject.position;

        if (directionFromCenter.sqrMagnitude < 0.001f)
        {
            directionFromCenter = Vector3.right;
        }
        Vector3 clampedPosition = centerObject.position + (directionFromCenter.normalized * orbitRadius);
        transform.position = clampedPosition;
        Vector3 targetDirection = centerObject.position - transform.position;

        if (targetDirection != Vector3.zero)
        {
            if (invertDirection)
            {
                targetDirection = -targetDirection;
            }
            Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, targetDirection.normalized);
            transform.rotation = targetRotation;
        }
    }

    private void OnDrawGizmos()
    {
        if (!showOrbitCircle || centerObject == null) return;

        Gizmos.color = circleColor;
        Gizmos.DrawWireSphere(centerObject.position, orbitRadius);
    }
}