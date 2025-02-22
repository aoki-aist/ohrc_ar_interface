//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.Ohrc
{
    [Serializable]
    public class HandStateMsg : Message
    {
        public const string k_RosMessageName = "ohrc_msgs/HandState";
        public override string RosMessageName => k_RosMessageName;

        public Geometry.PoseMsg pose;
        public Geometry.TwistMsg twist;
        //  geometry_msgs/Wrench wrench
        public float[] stick;
        public bool[] button;
        public bool[] touch;
        public double squeeze;
        public double trigger;
        public double grip;

        public HandStateMsg()
        {
            this.pose = new Geometry.PoseMsg();
            this.twist = new Geometry.TwistMsg();
            this.stick = new float[0];
            this.button = new bool[0];
            this.touch = new bool[0];
            this.squeeze = 0.0;
            this.trigger = 0.0;
            this.grip = 0.0;
        }

        public HandStateMsg(Geometry.PoseMsg pose, Geometry.TwistMsg twist, float[] stick, bool[] button, bool[] touch, double squeeze, double trigger, double grip)
        {
            this.pose = pose;
            this.twist = twist;
            this.stick = stick;
            this.button = button;
            this.touch = touch;
            this.squeeze = squeeze;
            this.trigger = trigger;
            this.grip = grip;
        }

        public static HandStateMsg Deserialize(MessageDeserializer deserializer) => new HandStateMsg(deserializer);

        private HandStateMsg(MessageDeserializer deserializer)
        {
            this.pose = Geometry.PoseMsg.Deserialize(deserializer);
            this.twist = Geometry.TwistMsg.Deserialize(deserializer);
            deserializer.Read(out this.stick, sizeof(float), deserializer.ReadLength());
            deserializer.Read(out this.button, sizeof(bool), deserializer.ReadLength());
            deserializer.Read(out this.touch, sizeof(bool), deserializer.ReadLength());
            deserializer.Read(out this.squeeze);
            deserializer.Read(out this.trigger);
            deserializer.Read(out this.grip);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.pose);
            serializer.Write(this.twist);
            serializer.WriteLength(this.stick);
            serializer.Write(this.stick);
            serializer.WriteLength(this.button);
            serializer.Write(this.button);
            serializer.WriteLength(this.touch);
            serializer.Write(this.touch);
            serializer.Write(this.squeeze);
            serializer.Write(this.trigger);
            serializer.Write(this.grip);
        }

        public override string ToString()
        {
            return "HandStateMsg: " +
            "\npose: " + pose.ToString() +
            "\ntwist: " + twist.ToString() +
            "\nstick: " + System.String.Join(", ", stick.ToList()) +
            "\nbutton: " + System.String.Join(", ", button.ToList()) +
            "\ntouch: " + System.String.Join(", ", touch.ToList()) +
            "\nsqueeze: " + squeeze.ToString() +
            "\ntrigger: " + trigger.ToString() +
            "\ngrip: " + grip.ToString();
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        public static void Register()
        {
            MessageRegistry.Register(k_RosMessageName, Deserialize);
        }
    }
}
