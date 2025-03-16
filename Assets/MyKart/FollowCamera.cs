using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("ī�޶� ���� Kart ������Ʈ�Դϴ�.")]
    public Transform target; // ���� ��� (Kart)

    [Header("Camera Offset Settings")]
    [Tooltip("ī�޶�� Kart ���� ��ġ �������Դϴ�.")]
    public Vector3 offset = new Vector3(0f, 5f, -10f); // �⺻ ������ ��
    [Tooltip("ī�޶� �̵� �ӵ��Դϴ�.")]
    public float followSpeed = 10f;
    [Tooltip("ī�޶� ȸ�� �ӵ��Դϴ�.")]
    public float rotationSmoothSpeed = 5f;

    private void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("FollowCamera: Target�� �������� �ʾҽ��ϴ�!");
            return;
        }

        // ��ġ ��� (Kart�� ��ġ + ������)
        Vector3 desiredPosition = target.position + target.rotation * offset; // ��� ȸ���� ���� ������ ����
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // ȸ�� ��� (����� ȸ�� ������ ����)
        Quaternion desiredRotation = Quaternion.Euler(0, target.eulerAngles.y, 0); // Y�� �߽� ȸ��
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSmoothSpeed * Time.deltaTime);
    }
}
