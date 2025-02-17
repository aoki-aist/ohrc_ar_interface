using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.XR;

using Unity.Robotics.ROSTCPConnector;
using TwistMsg = RosMessageTypes.Geometry.TwistMsg;

// OpenHRC body state message
using BodyStateMsg = RosMessageTypes.Ohrc.BodyStateMsg;

public class XrBodyStatePublisher : MonoBehaviour
{
    public InputDevice _smartphone;
    public InputDevice _rightController;
    public InputDevice _leftController;
    public InputDevice _HMD;

    [SerializeField] string topicName = "body_state";
    // AR Camera を設定
    [SerializeField] private Camera arCamera;

    private Vector3 previousPosition;
    private Quaternion previousRotation;
    private Vector3 velocity;
    private Vector3 angularVelocity;
    private Vector3 angularAcceleration;
    private Vector3 previousAngularVelocity;

    private bool buttonDownFlag = false;


    // ROS Connector
    private ROSConnection ros;
    private BodyStateMsg bodyStateMessage = new BodyStateMsg();
    
    public static readonly InputFeatureUsage<Vector3> pointerVelocity = new InputFeatureUsage<Vector3>("PointerVelocity");
    public static readonly InputFeatureUsage<Vector3> pointerAngularVelocity = new InputFeatureUsage<Vector3>("PointerAngularVelocity");
    public static readonly InputFeatureUsage<Vector3> pointerPosition = new InputFeatureUsage<Vector3>("PointerPosition");
    public static readonly InputFeatureUsage<Quaternion> pointerRotation = new InputFeatureUsage<Quaternion>("PointerRotation");

    private Vector3 filteredVelocity;
    private Vector3 filteredAngularVelocity;
    private Vector3 filteredAngularAcceleration;

    [Range(0.0f, 1.0f)]
    public float filterFactor = 0.1f; // フィルタ係数（0に近いほど平滑化が強い）

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<BodyStateMsg>(topicName);

         // 初期値を保存
        previousPosition = arCamera.transform.position;
        previousRotation = arCamera.transform.rotation;
        previousAngularVelocity = Vector3.zero;

        filteredVelocity = Vector3.zero;
        filteredAngularVelocity = Vector3.zero;
        filteredAngularAcceleration = Vector3.zero;
    }

    // get pose and twist from controllers
    void getPoseTwist(ref InputDevice device, ref RosMessageTypes.Ohrc.BodyPartStateMsg poseTwistMsg, 
        InputFeatureUsage<Vector3> inputVelocity, InputFeatureUsage<Vector3> inputAngularVelocity, InputFeatureUsage<Vector3> inputPosition, InputFeatureUsage<Quaternion> inputRotation)
    {
         // カメラの位置と回転を取得
        Vector3 cameraPosition = arCamera.transform.position;
        Quaternion cameraRotation = arCamera.transform.rotation;
        

        
        if (filteredVelocity != null)
        {
            poseTwistMsg.twist.linear.x = filteredVelocity.z;
            poseTwistMsg.twist.linear.y = -filteredVelocity.x;
            poseTwistMsg.twist.linear.z = filteredVelocity.y;
            // poseTwistMsg.twist.linear.x = 0;
            // poseTwistMsg.twist.linear.y = 0;
            // poseTwistMsg.twist.linear.z = filteredVelocity.y;
        }
        if (filteredAngularVelocity != null)
        {
            poseTwistMsg.twist.angular.x = -filteredAngularVelocity.z;
            poseTwistMsg.twist.angular.y = filteredAngularVelocity.x;
            poseTwistMsg.twist.angular.z = -filteredAngularVelocity.y;
            // poseTwistMsg.twist.angular.x = 0;
            // poseTwistMsg.twist.angular.y = 0;
            // poseTwistMsg.twist.angular.z = 0;
        }
        if (cameraPosition != null)
        {
            poseTwistMsg.pose.position.x = cameraPosition.z;
            poseTwistMsg.pose.position.y = -cameraPosition.x;
            poseTwistMsg.pose.position.z = cameraPosition.y;
            // poseTwistMsg.pose.position.x = 0;
            // poseTwistMsg.pose.position.y = 0;
            // poseTwistMsg.pose.position.z = 0;
        }
        if (cameraRotation != null)
        {
            poseTwistMsg.pose.orientation.x = cameraRotation.z;
            poseTwistMsg.pose.orientation.y = -cameraRotation.x;
            poseTwistMsg.pose.orientation.z = cameraRotation.y;
            poseTwistMsg.pose.orientation.w = -cameraRotation.w;
        }
    }

    void getDevicePoseTwist(ref InputDevice device, ref RosMessageTypes.Ohrc.BodyPartStateMsg poseTwistMsg)
    {
        getPoseTwist(ref device, ref poseTwistMsg, CommonUsages.deviceVelocity, CommonUsages.deviceAngularVelocity, CommonUsages.devicePosition, CommonUsages.deviceRotation);
    }

    void getPointerPoseTwist(ref InputDevice device, ref RosMessageTypes.Ohrc.BodyPartStateMsg poseTwistMsg)
    {
        getPoseTwist(ref device, ref poseTwistMsg, pointerVelocity, pointerAngularVelocity, pointerPosition, pointerRotation);
    }

    // get input from controllers
    void getHeadInput(ref InputDevice device, ref RosMessageTypes.Ohrc.BodyPartStateMsg BodyPartStateMsg)
    {
        getPointerPoseTwist(ref device, ref BodyPartStateMsg);

        if (device.TryGetFeatureValue(CommonUsages.trigger, out float trigger))
        {
            BodyPartStateMsg.trigger = trigger;
        }  
        // if (device.TryGetFeatureValue(CommonUsages.grip, out float grip))
        // {
        //     BodyPartStateMsg.grip = grip;
        // }
        BodyPartStateMsg.grip = 1.0;

        if (device.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 stick))
        {
            Array.Resize(ref BodyPartStateMsg.stick, 2);
            BodyPartStateMsg.stick[0] = stick.x;
            BodyPartStateMsg.stick[1] = stick.y;
        }

        Array.Resize(ref BodyPartStateMsg.button, 3);
        // get button input from controllers
        if (device.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButton))
        {
            BodyPartStateMsg.button[0] = primaryButton;
        }
        if (device.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryButton))
        {
            BodyPartStateMsg.button[1] = secondaryButton;
        }
        if (device.TryGetFeatureValue(CommonUsages.primary2DAxisClick , out bool stickClick))
        {
            BodyPartStateMsg.button[2] = stickClick;
        }

        //get button touch input from controllers
        Array.Resize(ref BodyPartStateMsg.touch, 3);
        if (device.TryGetFeatureValue(CommonUsages.primaryTouch, out bool primaryTouch))
        {
            BodyPartStateMsg.touch[0] = primaryTouch;
        }
        if (device.TryGetFeatureValue(CommonUsages.secondaryTouch, out bool secondaryTouch))
        {
            BodyPartStateMsg.touch[1] = secondaryTouch;
        }
        if (device.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out bool stickTouch))
        {
            BodyPartStateMsg.touch[2] = stickTouch;
        }

    }


    void getHMDInput(ref InputDevice device, ref RosMessageTypes.Ohrc.BodyPartStateMsg headStateMsg)
    {
        getDevicePoseTwist(ref device, ref headStateMsg);
    }


    private void InitializeInputDevices()
    { 
        if(!_rightController.isValid)
            InitializeInputDevice(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right, ref _rightController);
        if (!_leftController.isValid) 
            InitializeInputDevice(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left, ref _leftController);
        if (!_HMD.isValid) 
            InitializeInputDevice(InputDeviceCharacteristics.HeadMounted, ref _HMD);
        if (!_smartphone.isValid) 
            InitializeInputDevice(InputDeviceCharacteristics.Camera | InputDeviceCharacteristics.Simulated6DOF | InputDeviceCharacteristics.TrackedDevice, ref _smartphone);
    }

    private void InitializeInputDevice(InputDeviceCharacteristics inputCharacteristics, ref InputDevice inputDevice)
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(inputCharacteristics, devices);

        if (devices.Count > 0)
            inputDevice = devices[0];
    }

    void Update()
    {
        if(buttonDownFlag){
        if (!_rightController.isValid || !_leftController.isValid || !_HMD.isValid || !_smartphone.isValid)
            InitializeInputDevices();

        //getHeadInput(ref _rightController, ref bodyStateMessage.right_hand);
        //tHeadInput(ref _leftController, ref bodyStateMessage.left_hand);

       // 時間差を取得
        float deltaTime = Time.deltaTime;

        // 現在の位置と回転を取得
        Vector3 currentPosition = arCamera.transform.position;
        Quaternion currentRotation = arCamera.transform.rotation;

        // 線速度を計算
        velocity = (currentPosition - previousPosition) / deltaTime;

        // 低パスフィルタを適用
        filteredVelocity = Vector3.Lerp(filteredVelocity, velocity, filterFactor);

        // 角速度を計算
        Quaternion deltaRotation = currentRotation * Quaternion.Inverse(previousRotation);
        deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);
        angularVelocity = axis * (angle * Mathf.Deg2Rad / deltaTime);

        // 低パスフィルタを適用
        filteredAngularVelocity = Vector3.Lerp(filteredAngularVelocity, angularVelocity, filterFactor);

        // 角加速度を計算
        angularAcceleration = (angularVelocity - previousAngularVelocity) / deltaTime;

        // 低パスフィルタを適用
        filteredAngularAcceleration = Vector3.Lerp(filteredAngularAcceleration, angularAcceleration, filterFactor);
    
        // 現在の位置と回転を更新
        previousPosition = currentPosition;
        previousRotation = currentRotation;
        previousAngularVelocity = angularVelocity;

        // publish only smartphone positions as right hand
        getHeadInput(ref _smartphone, ref bodyStateMessage.right_hand);

        
        //getHMDInput(ref _HMD, ref bodyStateMessage.head);

        ros.Publish(topicName, bodyStateMessage);
        }
    }


    // ボタンを押したときの処理
  public void OnButtonDown()
  {
    Debug.Log("Down");
    previousPosition = arCamera.transform.position;
    previousRotation = arCamera.transform.rotation;
    previousAngularVelocity = Vector3.zero;

    filteredVelocity = Vector3.zero;
    filteredAngularVelocity = Vector3.zero;
    filteredAngularAcceleration = Vector3.zero;

    buttonDownFlag = true;
  }
  // ボタンを離したときの処理
  public void OnButtonUp()
  {
    Debug.Log("Up");
    buttonDownFlag = false;
    // 初期値を保存
  }
}


