using UnityEngine;
using UnityEngine.UI;
using Unity.Robotics.ROSTCPConnector;

public class RosIpSetup : MonoBehaviour
{
    public InputField ipInputField;
    public Button connectButton;
    public Text statusText;

    void Start()
    {
        connectButton.onClick.AddListener(SetROSIPAddress);
        statusText.text = "Enter ROS IP Address";
    }

    void SetROSIPAddress()
    {
        string ipAddress = ipInputField.text.Trim();

        if (string.IsNullOrEmpty(ipAddress))
        {
            statusText.text = "IP Address cannot be empty!";
            return;
        }

        ROSConnection.instance.RosIPAddress = ipAddress;
        ROSConnection.instance.Connect();
        statusText.text = $"Connected to ROS at {ipAddress}";
        Debug.Log($"ROS IP Set to: {ipAddress}");
    }
}
