//using UnityEngine;

//public class KartController : MonoBehaviour
//{
//    [Header("Wheels Settings")]
//    [Tooltip("Kart�� ��� ���� ������Ʈ�� �Ҵ��մϴ�.")]
//    public Transform[] wheels;

//    [Header("Steering Settings")]    
//    [Tooltip("�չ����� �ּ� ���� �����Դϴ�.")]
//    public float steerAngleFrontMin = -30f;
//    [Tooltip("�չ����� �ִ� ���� �����Դϴ�.")]
//    public float steerAngleFrontMax = 30f;
//    [Tooltip("���� �� ����ϴ� ȸ���� ũ���Դϴ�.")]
//    public float steeringForce = 200f;

//    [Header("Motor Settings")]
//    [Tooltip("Kart�� �������� �����մϴ�.")]
//    public float motorForce = 1000f;
//    [Tooltip("Kart�� �ִ� �ӵ��� �����մϴ�. (km/h)")]
//    public float maxSpeedKPH = 280f;

//    [Header("Drift Settings")]
//    [Tooltip("�帮��Ʈ�� Ȱ��ȭ�ϴ� Ű�Դϴ�.")]
//    public KeyCode driftKey = KeyCode.LeftShift;
//    [Tooltip("�帮��Ʈ �� Ÿ�̾��� ���� �������Դϴ�.")]
//    public float driftFactor = 0.5f;
//    [Tooltip("�帮��Ʈ�� ����Ǵ� ���� ���� ũ���Դϴ�.")]
//    public float driftForceSide = 200f;
//    [Tooltip("�帮��Ʈ�� �ִ� ���� �ð�(��)�Դϴ�.")]
//    public float maxDriftDuration = 2f;

//    [Header("Physics Settings")]
//    [Tooltip("������ ������ �� ����Ǵ� �ݴ� ���� ũ���Դϴ�.")]
//    public float antiRollForce = 5000f;
//    [Tooltip("������ ���� �� �ִ� ǥ��(���̾�)�� �����մϴ�.")]
//    public LayerMask groundLayer;
//    [Tooltip("�帮��Ʈ �� ���� ����Ǵ� ���� ũ���Դϴ�.")]
//    public float currentDriftForce = 20f;

//    [Header("Boost Settings")]
//    [Tooltip("�ν��� ���� �����մϴ�.")]
//    public float boostForce = 2000f;
//    [Tooltip("�ν��� ���� �ð�(��)�� �����մϴ�.")]
//    public float boostDuration = 1.5f;

//    [Header("Visual Effects")]
//    [Tooltip("�帮��Ʈ �� �����Ǵ� ��Ű�� ��ũ ������Ʈ�Դϴ�.")]
//    public GameObject[] skidMarks;

//    // ���������� ����� ������
//    private Rigidbody rb;
//    private bool isDrifting = false;
//    private float driftTime = 0f;
//    private float steerInput;
//    private float motorInput;
//    private bool isBoosting = false;
//    private float boostTime = 0f;

//    private void Start()
//    {
//        // Rigidbody �ʱ�ȭ
//        rb = GetComponent<Rigidbody>();
//    }

//    private void Update()
//    {
//        // �÷��̾� �Է� ó��
//        steerInput = Input.GetAxis("Horizontal");
//        motorInput = Input.GetAxis("Vertical");

//        // ���� ȸ�� ������Ʈ
//        UpdateWheelRotation(motorInput);

//        // ���� �ӵ� ���÷���
//        DisplaySpeed();
//    }

//    private void FixedUpdate()
//    {
//        // ������ ������Ʈ ó��
//        HandleSteering(steerInput);  // ���� ó��
//        HandleMovement(motorInput); // ������(������) ó��
//        ApplyAntiRoll();            // ���� ���� ����
//        HandleDrift();              // �帮��Ʈ ó��
//        LimitSpeed();               // �ӵ� ����
//    }

//    private void LimitSpeed()
//    {
//        // �ִ� �ӵ� ����
//        float currentSpeed = rb.velocity.magnitude * 3.6f;
//        if (currentSpeed > maxSpeedKPH)
//        {
//            float speedLimit = maxSpeedKPH / 3.6f;
//            rb.velocity = rb.velocity.normalized * speedLimit;
//        }
//    }

//    private void HandleDrift()
//    {
//        // �帮��Ʈ Ȱ��ȭ Ȯ��
//        if (Input.GetKey(driftKey) && driftTime < maxDriftDuration)
//        {
//            isDrifting = true;
//            driftTime += Time.deltaTime;

//            // �ӵ��� ���� �帮��Ʈ �� ����
//            float speedFactor = Mathf.Clamp01(rb.velocity.magnitude / (maxSpeedKPH / 3.6f));
//            float adjustedDriftForceSide = driftForceSide * (1 - speedFactor);

//            // ���� ���⿡ ���� �帮��Ʈ �� ����
//            if (Input.GetAxis("Horizontal") < 0)
//            {
//                currentDriftForce = Mathf.Lerp(currentDriftForce, adjustedDriftForceSide, Time.deltaTime * 5f);
//                Vector3 sideForce = transform.right * -currentDriftForce;
//                rb.AddForce(sideForce, ForceMode.Acceleration);
//            }
//            else if (Input.GetAxis("Horizontal") > 0)
//            {
//                currentDriftForce = Mathf.Lerp(currentDriftForce, adjustedDriftForceSide, Time.deltaTime * 5f);
//                Vector3 sideForce = transform.right * currentDriftForce;
//                rb.AddForce(sideForce, ForceMode.Acceleration);
//            }

//            // ��Ű�� ��ũ Ȱ��ȭ
//            foreach (GameObject skidMark in skidMarks)
//            {
//                skidMark.GetComponent<TrailRenderer>().emitting = true;
//            }
//        }
//        else
//        {
//            // �帮��Ʈ ���� �� �ν��� Ȱ��ȭ
//            if (isDrifting)
//            {
//                isBoosting = true;
//                boostTime = 0f; // �ν��� ���� �ð� �ʱ�ȭ
//            }

//            isDrifting = false;
//            driftTime = 0f;

//            // �帮��Ʈ ����
//            isDrifting = false;
//            driftTime = 0f;
//            currentDriftForce = 0f;

//            // ��Ű�� ��ũ ��Ȱ��ȭ
//            foreach (GameObject skidMark in skidMarks)
//            {
//                skidMark.GetComponent<TrailRenderer>().emitting = false;
//            }
//        }
//    }

//    private void HandleSteering(float steerInput)
//    {
//        // ���� �ΰ��� ���
//        float currentSpeed = rb.velocity.magnitude;
//        float steeringSensitivity = Mathf.Clamp(1 - (currentSpeed / (maxSpeedKPH / 3.6f)), 0.3f, 1f);

//        // ������ ���� ���� ���
//        float steerAngleFrontLeft = Mathf.Lerp(steerAngleFrontMin, steerAngleFrontMax, (steerInput + 1) / 2) * steeringSensitivity;
//        float steerAngleFrontRight = Mathf.Lerp(steerAngleFrontMin, steerAngleFrontMax, (steerInput + 1) / 2) * steeringSensitivity;

//        // ���� ȸ�� �ݿ�
//        wheels[0].localRotation = Quaternion.Euler(0, steerAngleFrontLeft, wheels[0].localRotation.eulerAngles.z);
//        wheels[1].localRotation = Quaternion.Euler(0, steerAngleFrontRight, wheels[1].localRotation.eulerAngles.z);

//        // ���� ���� ��ȯ ó��
//        Vector3 turnDirection = Quaternion.Euler(0, steerInput * steeringForce * steeringSensitivity * Time.deltaTime, 0) * transform.forward;
//        rb.MoveRotation(Quaternion.LookRotation(turnDirection));
//    }

//    private void HandleMovement(float motorInput)
//    {
//        // ������ ���
//        float adjustedMotorForce = isDrifting ? motorForce * 0.5f : motorForce;
//        Vector3 forwardForce = transform.forward * motorInput * adjustedMotorForce;

//        // �帮��Ʈ �� �ӵ� ����
//        if (isDrifting)
//        {
//            rb.velocity *= 0.98f;
//        }
//        // �ν��� Ȱ��ȭ �� �߰� �� ����
//        if (isBoosting)
//        {
//            forwardForce += transform.forward * boostForce;
//            boostTime += Time.deltaTime;

//            // �ν��� �ð��� �� �Ǹ� ����
//            if (boostTime >= boostDuration)
//            {
//                isBoosting = false;
//            }
//        }

//        // ������ ����
//        rb.AddForce(forwardForce, ForceMode.Force);
//    }

//    private void UpdateWheelRotation(float motorInput)
//    {
//        // �� ������ ȸ�� ���⿡ ���� ������Ʈ
//        float speed = rb.velocity.magnitude;
//        foreach (Transform wheel in wheels)
//        {
//            wheel.Rotate(Vector3.back * motorInput * speed);
//        }
//        //for (int i = 0; i < wheels.Length; i++)
//        //{
//        //    if (i == 0 || i == 2) // 0, 2�� ������ �ݴ� �������� ȸ��
//        //    {
//        //        wheels[i].Rotate(Vector3.back * motorInput * speed);
//        //    }
//        //    else if (i == 1 || i == 3) // 1, 3�� ������ ���������� ȸ��
//        //    {
//        //        wheels[i].Rotate(Vector3.forward * motorInput * speed);
//        //    }
//        //}
//    }

//    private void DisplaySpeed()
//    {
//        // ���� �ӵ��� �ֿܼ� ���
//        float speed = rb.velocity.magnitude * 3.6f;
//        Debug.Log("Current Speed: " + speed.ToString("F2") + " km/h");
//    }

//    private void ApplyAntiRoll()
//    {
//        for (int i = 0; i < wheels.Length; i += 2)
//        {
//            float leftSuspension = GetWheelSuspensionForce(wheels[i]);
//            float rightSuspension = GetWheelSuspensionForce(wheels[i + 1]);

//            float antiRoll = (leftSuspension - rightSuspension) * antiRollForce;

//            if (leftSuspension > 0)
//                rb.AddForceAtPosition(wheels[i].up * antiRoll, wheels[i].position, ForceMode.Force);

//            if (rightSuspension > 0)
//                rb.AddForceAtPosition(wheels[i + 1].up * -antiRoll, wheels[i + 1].position, ForceMode.Force);
//        }
//    }

//    private float GetWheelSuspensionForce(Transform wheel)
//    {
//        Ray ray = new Ray(wheel.position, -wheel.up);
//        if (Physics.Raycast(ray, out RaycastHit hit, 1f, groundLayer))
//        {
//            return 1 - hit.distance;
//        }
//        return 0;
//    }
//}
using System.Collections;
using UnityEngine;

public class KartController : MonoBehaviour
{

    [Header("Steering Settings")]
    [Tooltip("�ּ� ���� ����")]
    public float steerAngleFrontMin = -45f;
    [Tooltip("�ִ� ���� ����")]
    public float steerAngleFrontMax = 45f;
    [Tooltip("���� �ΰ���")]
    public float steeringForce = 200f;

    [Header("Motor Settings")]
    [Tooltip("���ӵ� ��")]
    public float motorForce = 1000f;
    [Tooltip("�ִ� �ӵ� (km/h)")]
    public float maxSpeedKPH = 280f;

    [Header("Drift Settings")]
    [Tooltip("�帮��Ʈ Ű ����")]
    public KeyCode driftKey = KeyCode.LeftShift;
    [Tooltip("�帮��Ʈ ���� ����")]
    public float driftFactor = 0.5f;
    [Tooltip("�帮��Ʈ ���� ��")]
    public float driftForceSide = 200f;
    [Tooltip("�ִ� �帮��Ʈ ���� �ð�")]
    public float maxDriftDuration = 2f;
    [Tooltip("�帮��Ʈ �� ���� ���� ��")]
    public float currentDriftForce = 20f;

    [Header("Drag Settings")]
    [Tooltip("�⺻ �巡�� ��")]
    public float normalDrag = 0.5f;
    [Tooltip("�帮��Ʈ �� �巡�� ��")]
    public float driftDrag = 0.01f;    
    [Tooltip("�⺻ Angular Drag ��")]
    public float normalAngularDrag = 0.05f;
    [Tooltip("�帮��Ʈ �� Angular Drag ��")]
    public float driftAngularDrag = 0.01f;

    [Header("Physics Settings")]
    [Tooltip("��Ƽ �� ����")]
    public float antiRollForce = 5000f;
    [Tooltip("���� ���̾�")]
    public LayerMask groundLayer;

    [Header("Boost Settings")]
    [Tooltip("�ν�Ʈ ��")]
    public float boostForce = 1.1f;
    [Tooltip("�ν�Ʈ ���� �ð�")]
    public float boostDuration = 1.5f;

    [Header("Wheels and Visual Effects")]
    [Tooltip("�� ��ü �迭")]
    public Transform[] wheels;
    [Tooltip("��Ű�� ��ũ ȿ��")]
    public GameObject[] skidMarks;

    [Header("UI Settings")]
    [Tooltip("�ӵ� ǥ�� �ؽ�Ʈ")]
    public UnityEngine.UI.Text speedText;    

    private Rigidbody rb;
    private bool isDrifting = false;
    private float driftTime = 0f;
    private float steerInput;
    private float motorInput;
    private bool isBoostTriggered = false;
    private bool isUpArrowKeyPressed = false; // ���� Ű�� �����ִ� ����
    private bool wasUpArrowKeyReleased = true; // ������ Ű�� ������ ����



    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        // ������ٵ� �巡�� �ʱ�ȭ
        rb.drag = normalDrag;
        // �⺻ Angular Drag �� �ʱ�ȭ
        rb.angularDrag = normalAngularDrag;

        // ��Ű�帶ũ �ʱ� ��Ȱ��ȭ
        foreach (GameObject skidMark in skidMarks)
        {
            skidMark.GetComponent<TrailRenderer>().emitting = false;
        }
    }


    private void Update()
    {
        steerInput = Input.GetAxis("Horizontal");
        motorInput = Input.GetAxis("Vertical");

        // Ű �Է� ���� ����
        if (Input.GetKey(KeyCode.UpArrow))
        {
            if (wasUpArrowKeyReleased)
            {
                isUpArrowKeyPressed = true; // ���Ӱ� Ű �Է�
                wasUpArrowKeyReleased = false; // �Է� ���·� ��ȯ
            }
            else
            {
                isUpArrowKeyPressed = false; // ���� �Է��� ����
            }
        }
        else
        {
            wasUpArrowKeyReleased = true; // Ű�� ������ ���·� ����
            isUpArrowKeyPressed = false; // �Է� ����
        }

        UpdateWheelRotation(motorInput);
        DisplaySpeed();
    }


    private void FixedUpdate()
    {
        HandleSteering(steerInput);  // ���� ó��
        HandleMovement(motorInput); // ����/���� ó��
        //ApplyAntiRoll();            // ��Ƽ�� ó��
        LimitSpeed();               // �ִ� �ӵ� ����

        // �帮��Ʈ ���� �� ���� ����
        if (Input.GetKey(driftKey))
        {
            StartDrift(); // �帮��Ʈ ����
        }
        else
        {
            EndDrift(); // �帮��Ʈ ����
        }
    }


    private void LimitSpeed()
    {
        float currentSpeed = rb.velocity.magnitude * 3.6f;
        if (currentSpeed > maxSpeedKPH)
        {
            float speedLimit = maxSpeedKPH / 3.6f;
            rb.velocity = rb.velocity.normalized * speedLimit;
        }
    }

    public void StartDrift()
    {
        if (!isDrifting)
        {
            isDrifting = true;
            driftTime = 0f;

            // �ﰢ���� ���� ȿ��
            rb.velocity *= 0.8f;
            // �帮��Ʈ �� �巡�� �� ����
            rb.drag = driftDrag;
            // Angular Drag ����
            rb.angularDrag = driftAngularDrag;

            // �帮��Ʈ �� ����
            float speedFactor = Mathf.Clamp01(rb.velocity.magnitude / (maxSpeedKPH / 3.6f));
            float adjustedDriftForceSide = driftForceSide * (1 - speedFactor) * 5f;

            if (Input.GetAxis("Horizontal") < 0)
            {
                currentDriftForce = Mathf.Lerp(currentDriftForce, adjustedDriftForceSide, Time.deltaTime * 10f);
                Vector3 sideForce = transform.right * -currentDriftForce;
                rb.AddForce(sideForce, ForceMode.Acceleration);
            }
            else if (Input.GetAxis("Horizontal") > 0)
            {
                currentDriftForce = Mathf.Lerp(currentDriftForce, adjustedDriftForceSide, Time.deltaTime * 10f);
                Vector3 sideForce = transform.right * currentDriftForce;
                rb.AddForce(sideForce, ForceMode.Acceleration);
            }

            // ��Ű�� ��ũ Ȱ��ȭ
            foreach (GameObject skidMark in skidMarks)
            {
                skidMark.GetComponent<TrailRenderer>().emitting = true;
            }
            Debug.Log("�帮��Ʈ ����!");
        }
    }





    public void EndDrift()
    {
        if (isDrifting)
        {
            isDrifting = false;
            driftTime = 0f;
            currentDriftForce = 0f;
            // �⺻ �巡�� �� ����
            rb.drag = normalDrag;
            // Angular Drag ����
            rb.angularDrag = normalAngularDrag;

            // ��Ű�帶ũ ��Ȱ��ȭ
            foreach (GameObject skidMark in skidMarks)
            {
                skidMark.GetComponent<TrailRenderer>().emitting = false;
            }

            Debug.Log("�帮��Ʈ ����!");
            StartCoroutine(BoostCheckCoroutine());
        }
    }
    private IEnumerator BoostCheckCoroutine()
    {
        float timer = 0f;
        while (timer < 0.5f)
        {
            if (isUpArrowKeyPressed) // ���Է� ����
            {
                TriggerBoost();
                break;
            }
            timer += Time.deltaTime;
            yield return null;
        }
    }

    private void TriggerBoost()
    {
        if (!isBoostTriggered)
        {
            isBoostTriggered = true;
            Debug.Log("�ν��� �ߵ�!");
            StartCoroutine(ApplyBoostCoroutine());
        }
    }

    private IEnumerator ApplyBoostCoroutine()
    {
        float timer = 0f;
        while (timer < boostDuration)
        {
            float currentSpeed = rb.velocity.magnitude * 3.6f; // m/s�� km/h�� ��ȯ
            if (currentSpeed < maxSpeedKPH) // �ִ� �ӵ� �ʰ� ���� Ȯ��
            {
                rb.AddForce(transform.forward * boostForce, ForceMode.Acceleration);
            }
            timer += Time.deltaTime;
            yield return null;
        }
        isBoostTriggered = false;
    }

    private void HandleSteering(float steerInput)
    {
        float currentSpeed = rb.velocity.magnitude;
        float steeringSensitivity = Mathf.Clamp(1 - (currentSpeed / (maxSpeedKPH / 3.6f)), 0.5f, 2.0f); // �ӵ��� ���� ���� �ΰ��� ����

        // �̵� ����� ��ȭ
        if (isDrifting) // �帮��Ʈ �߿��� ��ȭ�� ���� ȿ�� �߰�
        {
            rb.velocity = Quaternion.Euler(0, steerInput * 90f * Time.deltaTime, 0) * rb.velocity;
        }

        // ���� ���� ���
        float steerAngleFrontLeft = Mathf.Lerp(steerAngleFrontMin, steerAngleFrontMax, (steerInput + 1) / 2) * steeringSensitivity;
        float steerAngleFrontRight = Mathf.Lerp(steerAngleFrontMin, steerAngleFrontMax, (steerInput + 1) / 2) * steeringSensitivity;

        // �� ���� ȸ��
        wheels[0].localRotation = Quaternion.Euler(0, steerAngleFrontLeft, wheels[0].localRotation.eulerAngles.z);
        wheels[1].localRotation = Quaternion.Euler(0, steerAngleFrontRight, wheels[1].localRotation.eulerAngles.z);

        // ���� ��ü ȸ�� ó��
        Vector3 turnDirection = Quaternion.Euler(0, steerInput * steeringForce * steeringSensitivity * Time.deltaTime, 0) * transform.forward;
        rb.MoveRotation(Quaternion.LookRotation(turnDirection));
    }



    private void HandleMovement(float motorInput)
    {
        float adjustedMotorForce = isDrifting ? motorForce * 0.3f : motorForce;
        Vector3 forwardForce = transform.forward * motorInput * adjustedMotorForce;

        rb.AddForce(forwardForce, ForceMode.Force);
    }

    private void UpdateWheelRotation(float motorInput)
    {
        float speed = rb.velocity.magnitude;
        foreach (Transform wheel in wheels)
        {
            wheel.Rotate(Vector3.back * motorInput * speed);
        }
    }

    private void DisplaySpeed()
    {
        float speed = rb.velocity.magnitude * 3.6f; // m/s�� km/h�� ��ȯ
        if (speedText != null)
        {
            speedText.text = "Speed: " + speed.ToString("F2") + " km/h";
        }
    }


    private void ApplyAntiRoll()
    {
        for (int i = 0; i < wheels.Length; i += 2)
        {
            float leftSuspension = GetWheelSuspensionForce(wheels[i]);
            float rightSuspension = GetWheelSuspensionForce(wheels[i + 1]);

            float antiRoll = (leftSuspension - rightSuspension) * antiRollForce;

            if (leftSuspension > 0)
                rb.AddForceAtPosition(wheels[i].up * antiRoll, wheels[i].position, ForceMode.Force);

            if (rightSuspension > 0)
                rb.AddForceAtPosition(wheels[i + 1].up * -antiRoll, wheels[i + 1].position, ForceMode.Force);
        }
    }

    private float GetWheelSuspensionForce(Transform wheel)
    {
        Ray ray = new Ray(wheel.position, -wheel.up);
        if (Physics.Raycast(ray, out RaycastHit hit, 1f, groundLayer))
        {
            return 1 - hit.distance;
        }
        return 0;
    }
}
