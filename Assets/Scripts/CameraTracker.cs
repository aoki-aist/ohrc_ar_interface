using UnityEngine;

public class CameraTracker : MonoBehaviour
{
    // AR Camera を設定（Unity Editorでドラッグ＆ドロップ）
    [SerializeField]
    private Camera arCamera;

    void Update()
    {
        // カメラの位置と回転を取得
        Vector3 cameraPosition = arCamera.transform.position;
        Quaternion cameraRotation = arCamera.transform.rotation;

        // オイラー角（X, Y, Z軸の回転角度）を取得
        Vector3 eulerRotation = cameraRotation.eulerAngles;

        // ログ出力
        Debug.Log($"Camera Position: {cameraPosition}");
        Debug.Log($"Camera Rotation (Quaternion): {cameraRotation}");
        Debug.Log($"Camera Rotation (Euler): {eulerRotation}");
    }
}
