using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.XR;

using Unity.Robotics.ROSTCPConnector;

using Float32Msg = RosMessageTypes.Std.Float32Msg;

public class XrFeedbackSubscriber : MonoBehaviour
{
    public InputDevice _rightController;
    public InputDevice _leftController;

    [SerializeField] string topicName_right = "feedback/RIGHT_HAND";
    [SerializeField] string topicName_left = "feedback/LEFT_HAND";

    private ROSConnection ros;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<Float32Msg>(topicName_right, FeedbackRight);
        ros.Subscribe<Float32Msg>(topicName_left, FeedbackLeft);
    }

    void HapticFeedback(Float32Msg feedback, InputDevice controller, InputDeviceCharacteristics Device){
        if (!controller.isValid)
            InitializeInputDevice(InputDeviceCharacteristics.Controller | Device, ref controller);
    
        feedback.data = Math.Min(Math.Max(feedback.data, 0.0f), 1.0f);
        controller.SendHapticImpulse(0, feedback.data, 0.05f);
    }

    void FeedbackRight(Float32Msg feedback)
    {
        HapticFeedback(feedback, _rightController, InputDeviceCharacteristics.Right);
    }

    void FeedbackLeft(Float32Msg feedback)
    {
        HapticFeedback(feedback, _leftController, InputDeviceCharacteristics.Left);
    }

    private void InitializeInputDevice(InputDeviceCharacteristics inputCharacteristics, ref InputDevice inputDevice)
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(inputCharacteristics, devices);

        if (devices.Count > 0)
        {
            inputDevice = devices[0];
        }
    }
}