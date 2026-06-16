using UnityEngine;
using UnityEngine.InputSystem;

public class LegMovementPrototype : MonoBehaviour
{
    private enum ControlledLeg { None, Left, Right }

    [Header("Leg Rigidbody References")]
    [SerializeField] private Rigidbody leftLegRb;
    [SerializeField] private Rigidbody rightLegRb;

    [Header("3D Mouse Movement Settings")]
    [SerializeField] private float mouseSensitivity = 15f; // ความไวของเมาส์แกน X, Y
    [SerializeField] private float scrollSensitivity = 50f; // ความไวของลูกกลิ้งเมาส์แกน Z
    [SerializeField] private float maxForce = 3000f;       // ลิมิตแรงสูงสุดกันขากระเด็นหาย

    [Header("Rotation Settings (WASD)")]
    [SerializeField] private float rotationTorque = 150f;

    private ControlledLeg currentActiveLeg = ControlledLeg.None;
    private Vector2 movementInput = Vector2.zero;
    private Vector2 mouseDeltaInput = Vector2.zero;
    private float scrollInput = 0f;

    // 1. ปุ่มปลดล็อกขาซ้าย
    public void OnSelectLeftLeg(InputAction.CallbackContext context)
    {
        if (context.started || context.performed) currentActiveLeg = ControlledLeg.Left;
        else if (context.canceled && currentActiveLeg == ControlledLeg.Left) currentActiveLeg = ControlledLeg.None;
    }

    // 2. ปุ่มปลดล็อกขาขวา
    public void OnSelectRightLeg(InputAction.CallbackContext context)
    {
        if (context.started || context.performed) currentActiveLeg = ControlledLeg.Right;
        else if (context.canceled && currentActiveLeg == ControlledLeg.Right) currentActiveLeg = ControlledLeg.None;
    }

    // 3. หมุนขา (ยืมช่อง Move วิ่งเข้าข่ายหมุนตัว)
    public void OnLegMoveInput(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    // 4. รับค่าขยับเมาส์ (เชื่อมกับ Look)
    public void OnMouseMoveInput(InputAction.CallbackContext context)
    {
        mouseDeltaInput = context.ReadValue<Vector2>();
    }

    // 5. ใหม่! รับค่าลูกกลิ้งเมาส์สำหรับดันแกนลึก (Z)
    // ให้สร้าง Event หรือดูวิธีตั้งค่าด้านล่างครับ
    public void OnMouseScrollInput(InputAction.CallbackContext context)
    {
        scrollInput = context.ReadValue<Vector2>().y; // ล้อเมาส์ปกติจะส่งค่ามาในแกน Y ของ Vector2
    }

    private void FixedUpdate()
    {
        if (currentActiveLeg == ControlledLeg.Left && leftLegRb != null)
        {
            ApplyFree3DMovement(leftLegRb);
        }
        else if (currentActiveLeg == ControlledLeg.Right && rightLegRb != null)
        {
            ApplyFree3DMovement(rightLegRb);
        }
    }

    private void ApplyFree3DMovement(Rigidbody legRb)
    {
        // --- 1. การหมุนขาด้วยคีย์บอร์ด (WASD ขยับบิดมุม) ---
        if (Mathf.Abs(movementInput.x) > 0.01f)
        {
            Vector3 torqueDirection = transform.up * movementInput.x * rotationTorque;
            legRb.AddTorque(torqueDirection * Time.fixedDeltaTime, ForceMode.Force);
        }

        // --- 2. การเคลื่อนที่ 3 มิติอิสระด้วยเมาส์ (อิงตามมุมมองกล้อง) ---
        Transform camTransform = Camera.main.transform;
        
        // แปลงความเคลื่อนไหวเมาส์ให้เป็นทิศทางในโลก 3D ตามหน้ากล้อง
        Vector3 moveDirection = (camTransform.right * mouseDeltaInput.x * mouseSensitivity) + 
                                (camTransform.up * mouseDeltaInput.y * mouseSensitivity) + 
                                (camTransform.forward * scrollInput * scrollSensitivity);

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            // จำกัดแรงไม่ให้สะบัดแรงจนฟิสิกส์ระเบิด
            Vector3 appliedForce = moveDirection * Time.fixedDeltaTime;
            appliedForce = Vector3.ClampMagnitude(appliedForce, maxForce * Time.fixedDeltaTime);

            // ใส่แรงดันแบบสมบูรณ์ 3 มิติ (พุ่ง, ลอย, จม ได้อิสระ)
            legRb.AddForce(appliedForce, ForceMode.Force);
        }
    }
}