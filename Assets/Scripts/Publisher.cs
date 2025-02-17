using UnityEngine;

public class Publisher : MonoBehaviour
{
    public Camera arCamera; // InspectorでARカメラを指定

    private Vector3 previousPosition;
    private Quaternion previousRotation;
    private Vector3 velocity;
    private Vector3 angularVelocity;
    private Vector3 angularAcceleration;
    private Vector3 previousAngularVelocity;

    void Start()
    {
        // 初期値を保存
        previousPosition = arCamera.transform.position;
        previousRotation = arCamera.transform.rotation;
        previousAngularVelocity = Vector3.zero;
    }

    void Update()
    {
        // 現在の位置と回転
        Vector3 currentPosition = arCamera.transform.position;
        Quaternion currentRotation = arCamera.transform.rotation;

        // 時間差を計算
        float deltaTime = Time.deltaTime;

        // 線速度を計算
        velocity = (currentPosition - previousPosition) / deltaTime;

        // 角速度を計算
        Quaternion deltaRotation = currentRotation * Quaternion.Inverse(previousRotation);
        deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);
        angularVelocity = axis * (angle * Mathf.Deg2Rad / deltaTime);

        // 角加速度を計算
        angularAcceleration = (angularVelocity - previousAngularVelocity) / deltaTime;

        // デバッグ表示
        Debug.Log($"Velocity: {velocity}");
        Debug.Log($"Angular Velocity: {angularVelocity}");
        Debug.Log($"Angular Acceleration: {angularAcceleration}");

        // 現在の位置と回転を更新
        previousPosition = currentPosition;
        previousRotation = currentRotation;
        previousAngularVelocity = angularVelocity;
    }
}
