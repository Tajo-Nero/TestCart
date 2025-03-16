//using UnityEngine;

//public class KartController : MonoBehaviour
//{
//    [Header("Wheels Settings")]
//    [Tooltip("Kart의 모든 바퀴 오브젝트를 할당합니다.")]
//    public Transform[] wheels;

//    [Header("Steering Settings")]    
//    [Tooltip("앞바퀴의 최소 조향 각도입니다.")]
//    public float steerAngleFrontMin = -30f;
//    [Tooltip("앞바퀴의 최대 조향 각도입니다.")]
//    public float steerAngleFrontMax = 30f;
//    [Tooltip("조향 시 사용하는 회전력 크기입니다.")]
//    public float steeringForce = 200f;

//    [Header("Motor Settings")]
//    [Tooltip("Kart의 추진력을 설정합니다.")]
//    public float motorForce = 1000f;
//    [Tooltip("Kart의 최대 속도를 설정합니다. (km/h)")]
//    public float maxSpeedKPH = 280f;

//    [Header("Drift Settings")]
//    [Tooltip("드리프트를 활성화하는 키입니다.")]
//    public KeyCode driftKey = KeyCode.LeftShift;
//    [Tooltip("드리프트 중 타이어의 마찰 감소율입니다.")]
//    public float driftFactor = 0.5f;
//    [Tooltip("드리프트로 적용되는 측면 힘의 크기입니다.")]
//    public float driftForceSide = 200f;
//    [Tooltip("드리프트의 최대 지속 시간(초)입니다.")]
//    public float maxDriftDuration = 2f;

//    [Header("Physics Settings")]
//    [Tooltip("차량이 기울어질 때 적용되는 반대 힘의 크기입니다.")]
//    public float antiRollForce = 5000f;
//    [Tooltip("바퀴가 닿을 수 있는 표면(레이어)을 지정합니다.")]
//    public LayerMask groundLayer;
//    [Tooltip("드리프트 중 현재 적용되는 힘의 크기입니다.")]
//    public float currentDriftForce = 20f;

//    [Header("Boost Settings")]
//    [Tooltip("부스터 힘을 설정합니다.")]
//    public float boostForce = 2000f;
//    [Tooltip("부스터 지속 시간(초)을 설정합니다.")]
//    public float boostDuration = 1.5f;

//    [Header("Visual Effects")]
//    [Tooltip("드리프트 시 생성되는 스키드 마크 오브젝트입니다.")]
//    public GameObject[] skidMarks;

//    // 내부적으로 사용할 변수들
//    private Rigidbody rb;
//    private bool isDrifting = false;
//    private float driftTime = 0f;
//    private float steerInput;
//    private float motorInput;
//    private bool isBoosting = false;
//    private float boostTime = 0f;

//    private void Start()
//    {
//        // Rigidbody 초기화
//        rb = GetComponent<Rigidbody>();
//    }

//    private void Update()
//    {
//        // 플레이어 입력 처리
//        steerInput = Input.GetAxis("Horizontal");
//        motorInput = Input.GetAxis("Vertical");

//        // 바퀴 회전 업데이트
//        UpdateWheelRotation(motorInput);

//        // 현재 속도 디스플레이
//        DisplaySpeed();
//    }

//    private void FixedUpdate()
//    {
//        // 물리적 업데이트 처리
//        HandleSteering(steerInput);  // 조향 처리
//        HandleMovement(motorInput); // 움직임(추진력) 처리
//        ApplyAntiRoll();            // 차량 균형 유지
//        HandleDrift();              // 드리프트 처리
//        LimitSpeed();               // 속도 제한
//    }

//    private void LimitSpeed()
//    {
//        // 최대 속도 제한
//        float currentSpeed = rb.velocity.magnitude * 3.6f;
//        if (currentSpeed > maxSpeedKPH)
//        {
//            float speedLimit = maxSpeedKPH / 3.6f;
//            rb.velocity = rb.velocity.normalized * speedLimit;
//        }
//    }

//    private void HandleDrift()
//    {
//        // 드리프트 활성화 확인
//        if (Input.GetKey(driftKey) && driftTime < maxDriftDuration)
//        {
//            isDrifting = true;
//            driftTime += Time.deltaTime;

//            // 속도에 따라 드리프트 힘 조정
//            float speedFactor = Mathf.Clamp01(rb.velocity.magnitude / (maxSpeedKPH / 3.6f));
//            float adjustedDriftForceSide = driftForceSide * (1 - speedFactor);

//            // 조향 방향에 따른 드리프트 힘 적용
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

//            // 스키드 마크 활성화
//            foreach (GameObject skidMark in skidMarks)
//            {
//                skidMark.GetComponent<TrailRenderer>().emitting = true;
//            }
//        }
//        else
//        {
//            // 드리프트 종료 시 부스터 활성화
//            if (isDrifting)
//            {
//                isBoosting = true;
//                boostTime = 0f; // 부스터 지속 시간 초기화
//            }

//            isDrifting = false;
//            driftTime = 0f;

//            // 드리프트 종료
//            isDrifting = false;
//            driftTime = 0f;
//            currentDriftForce = 0f;

//            // 스키드 마크 비활성화
//            foreach (GameObject skidMark in skidMarks)
//            {
//                skidMark.GetComponent<TrailRenderer>().emitting = false;
//            }
//        }
//    }

//    private void HandleSteering(float steerInput)
//    {
//        // 조향 민감도 계산
//        float currentSpeed = rb.velocity.magnitude;
//        float steeringSensitivity = Mathf.Clamp(1 - (currentSpeed / (maxSpeedKPH / 3.6f)), 0.3f, 1f);

//        // 바퀴별 조향 각도 계산
//        float steerAngleFrontLeft = Mathf.Lerp(steerAngleFrontMin, steerAngleFrontMax, (steerInput + 1) / 2) * steeringSensitivity;
//        float steerAngleFrontRight = Mathf.Lerp(steerAngleFrontMin, steerAngleFrontMax, (steerInput + 1) / 2) * steeringSensitivity;

//        // 바퀴 회전 반영
//        wheels[0].localRotation = Quaternion.Euler(0, steerAngleFrontLeft, wheels[0].localRotation.eulerAngles.z);
//        wheels[1].localRotation = Quaternion.Euler(0, steerAngleFrontRight, wheels[1].localRotation.eulerAngles.z);

//        // 차량 방향 전환 처리
//        Vector3 turnDirection = Quaternion.Euler(0, steerInput * steeringForce * steeringSensitivity * Time.deltaTime, 0) * transform.forward;
//        rb.MoveRotation(Quaternion.LookRotation(turnDirection));
//    }

//    private void HandleMovement(float motorInput)
//    {
//        // 추진력 계산
//        float adjustedMotorForce = isDrifting ? motorForce * 0.5f : motorForce;
//        Vector3 forwardForce = transform.forward * motorInput * adjustedMotorForce;

//        // 드리프트 중 속도 감소
//        if (isDrifting)
//        {
//            rb.velocity *= 0.98f;
//        }
//        // 부스터 활성화 시 추가 힘 적용
//        if (isBoosting)
//        {
//            forwardForce += transform.forward * boostForce;
//            boostTime += Time.deltaTime;

//            // 부스터 시간이 다 되면 종료
//            if (boostTime >= boostDuration)
//            {
//                isBoosting = false;
//            }
//        }

//        // 추진력 적용
//        rb.AddForce(forwardForce, ForceMode.Force);
//    }

//    private void UpdateWheelRotation(float motorInput)
//    {
//        // 각 바퀴를 회전 방향에 따라 업데이트
//        float speed = rb.velocity.magnitude;
//        foreach (Transform wheel in wheels)
//        {
//            wheel.Rotate(Vector3.back * motorInput * speed);
//        }
//        //for (int i = 0; i < wheels.Length; i++)
//        //{
//        //    if (i == 0 || i == 2) // 0, 2번 바퀴는 반대 방향으로 회전
//        //    {
//        //        wheels[i].Rotate(Vector3.back * motorInput * speed);
//        //    }
//        //    else if (i == 1 || i == 3) // 1, 3번 바퀴는 정방향으로 회전
//        //    {
//        //        wheels[i].Rotate(Vector3.forward * motorInput * speed);
//        //    }
//        //}
//    }

//    private void DisplaySpeed()
//    {
//        // 현재 속도를 콘솔에 출력
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
    [Tooltip("최소 조향 각도")]
    public float steerAngleFrontMin = -45f;
    [Tooltip("최대 조향 각도")]
    public float steerAngleFrontMax = 45f;
    [Tooltip("조향 민감도")]
    public float steeringForce = 200f;

    [Header("Motor Settings")]
    [Tooltip("가속도 힘")]
    public float motorForce = 1000f;
    [Tooltip("최대 속도 (km/h)")]
    public float maxSpeedKPH = 280f;

    [Header("Drift Settings")]
    [Tooltip("드리프트 키 설정")]
    public KeyCode driftKey = KeyCode.LeftShift;
    [Tooltip("드리프트 감속 비율")]
    public float driftFactor = 0.5f;
    [Tooltip("드리프트 측면 힘")]
    public float driftForceSide = 200f;
    [Tooltip("최대 드리프트 지속 시간")]
    public float maxDriftDuration = 2f;
    [Tooltip("드리프트 중 현재 측면 힘")]
    public float currentDriftForce = 20f;

    [Header("Drag Settings")]
    [Tooltip("기본 드래그 값")]
    public float normalDrag = 0.5f;
    [Tooltip("드리프트 중 드래그 값")]
    public float driftDrag = 0.01f;    
    [Tooltip("기본 Angular Drag 값")]
    public float normalAngularDrag = 0.05f;
    [Tooltip("드리프트 중 Angular Drag 값")]
    public float driftAngularDrag = 0.01f;

    [Header("Physics Settings")]
    [Tooltip("안티 롤 강도")]
    public float antiRollForce = 5000f;
    [Tooltip("지면 레이어")]
    public LayerMask groundLayer;

    [Header("Boost Settings")]
    [Tooltip("부스트 힘")]
    public float boostForce = 1.1f;
    [Tooltip("부스트 지속 시간")]
    public float boostDuration = 1.5f;

    [Header("Wheels and Visual Effects")]
    [Tooltip("휠 객체 배열")]
    public Transform[] wheels;
    [Tooltip("스키드 마크 효과")]
    public GameObject[] skidMarks;

    [Header("UI Settings")]
    [Tooltip("속도 표시 텍스트")]
    public UnityEngine.UI.Text speedText;    

    private Rigidbody rb;
    private bool isDrifting = false;
    private float driftTime = 0f;
    private float steerInput;
    private float motorInput;
    private bool isBoostTriggered = false;
    private bool isUpArrowKeyPressed = false; // 현재 키가 눌려있는 상태
    private bool wasUpArrowKeyReleased = true; // 이전에 키가 떼어진 상태



    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        // 리지드바디 드래그 초기화
        rb.drag = normalDrag;
        // 기본 Angular Drag 값 초기화
        rb.angularDrag = normalAngularDrag;

        // 스키드마크 초기 비활성화
        foreach (GameObject skidMark in skidMarks)
        {
            skidMark.GetComponent<TrailRenderer>().emitting = false;
        }
    }


    private void Update()
    {
        steerInput = Input.GetAxis("Horizontal");
        motorInput = Input.GetAxis("Vertical");

        // 키 입력 상태 추적
        if (Input.GetKey(KeyCode.UpArrow))
        {
            if (wasUpArrowKeyReleased)
            {
                isUpArrowKeyPressed = true; // 새롭게 키 입력
                wasUpArrowKeyReleased = false; // 입력 상태로 전환
            }
            else
            {
                isUpArrowKeyPressed = false; // 지속 입력은 무시
            }
        }
        else
        {
            wasUpArrowKeyReleased = true; // 키가 떼어진 상태로 변경
            isUpArrowKeyPressed = false; // 입력 해제
        }

        UpdateWheelRotation(motorInput);
        DisplaySpeed();
    }


    private void FixedUpdate()
    {
        HandleSteering(steerInput);  // 조향 처리
        HandleMovement(motorInput); // 가속/감속 처리
        //ApplyAntiRoll();            // 안티롤 처리
        LimitSpeed();               // 최대 속도 제한

        // 드리프트 시작 및 종료 관리
        if (Input.GetKey(driftKey))
        {
            StartDrift(); // 드리프트 시작
        }
        else
        {
            EndDrift(); // 드리프트 종료
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

            // 즉각적인 감속 효과
            rb.velocity *= 0.8f;
            // 드리프트 중 드래그 값 적용
            rb.drag = driftDrag;
            // Angular Drag 조정
            rb.angularDrag = driftAngularDrag;

            // 드리프트 힘 조정
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

            // 스키드 마크 활성화
            foreach (GameObject skidMark in skidMarks)
            {
                skidMark.GetComponent<TrailRenderer>().emitting = true;
            }
            Debug.Log("드리프트 시작!");
        }
    }





    public void EndDrift()
    {
        if (isDrifting)
        {
            isDrifting = false;
            driftTime = 0f;
            currentDriftForce = 0f;
            // 기본 드래그 값 복원
            rb.drag = normalDrag;
            // Angular Drag 복원
            rb.angularDrag = normalAngularDrag;

            // 스키드마크 비활성화
            foreach (GameObject skidMark in skidMarks)
            {
                skidMark.GetComponent<TrailRenderer>().emitting = false;
            }

            Debug.Log("드리프트 종료!");
            StartCoroutine(BoostCheckCoroutine());
        }
    }
    private IEnumerator BoostCheckCoroutine()
    {
        float timer = 0f;
        while (timer < 0.5f)
        {
            if (isUpArrowKeyPressed) // 재입력 감지
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
            Debug.Log("부스터 발동!");
            StartCoroutine(ApplyBoostCoroutine());
        }
    }

    private IEnumerator ApplyBoostCoroutine()
    {
        float timer = 0f;
        while (timer < boostDuration)
        {
            float currentSpeed = rb.velocity.magnitude * 3.6f; // m/s를 km/h로 변환
            if (currentSpeed < maxSpeedKPH) // 최대 속도 초과 여부 확인
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
        float steeringSensitivity = Mathf.Clamp(1 - (currentSpeed / (maxSpeedKPH / 3.6f)), 0.5f, 2.0f); // 속도에 따라 조향 민감도 증가

        // 이동 방향과 조화
        if (isDrifting) // 드리프트 중에는 강화된 유턴 효과 추가
        {
            rb.velocity = Quaternion.Euler(0, steerInput * 90f * Time.deltaTime, 0) * rb.velocity;
        }

        // 조향 각도 계산
        float steerAngleFrontLeft = Mathf.Lerp(steerAngleFrontMin, steerAngleFrontMax, (steerInput + 1) / 2) * steeringSensitivity;
        float steerAngleFrontRight = Mathf.Lerp(steerAngleFrontMin, steerAngleFrontMax, (steerInput + 1) / 2) * steeringSensitivity;

        // 앞 바퀴 회전
        wheels[0].localRotation = Quaternion.Euler(0, steerAngleFrontLeft, wheels[0].localRotation.eulerAngles.z);
        wheels[1].localRotation = Quaternion.Euler(0, steerAngleFrontRight, wheels[1].localRotation.eulerAngles.z);

        // 차량 자체 회전 처리
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
        float speed = rb.velocity.magnitude * 3.6f; // m/s를 km/h로 변환
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
