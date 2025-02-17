using UnityEngine;
using UnityEngine.UI;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;

public class GripperPublisher : MonoBehaviour
{
    ROSConnection ros;
    public Slider gripperSlider;
    private float gripperPosition = 0.0f;  // 初期位置
    private float maxEffort = 1.0f;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<Float64MultiArrayMsg>("/gripper_goal");

        // スライダーが変更されたら値を更新
        gripperSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    void OnSliderValueChanged(float value)
    {
        gripperPosition = value;
        PublishGripperGoal();
    }

    void PublishGripperGoal()
    {
        Float64MultiArrayMsg msg = new Float64MultiArrayMsg();
        msg.data = new double[] { gripperPosition, maxEffort };

        ros.Publish("/gripper_goal", msg);
        Debug.Log($"Published gripper goal: Position={gripperPosition}, MaxEffort={maxEffort}");
    }
}
