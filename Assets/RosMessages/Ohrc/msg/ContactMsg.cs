//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.Ohrc
{
    [Serializable]
    public class ContactMsg : Message
    {
        public const string k_RosMessageName = "ohrc_msgs/Contact";
        public override string RosMessageName => k_RosMessageName;

        public Std.HeaderMsg header;
        public float f_norm;
        public bool contact;

        public ContactMsg()
        {
            this.header = new Std.HeaderMsg();
            this.f_norm = 0.0f;
            this.contact = false;
        }

        public ContactMsg(Std.HeaderMsg header, float f_norm, bool contact)
        {
            this.header = header;
            this.f_norm = f_norm;
            this.contact = contact;
        }

        public static ContactMsg Deserialize(MessageDeserializer deserializer) => new ContactMsg(deserializer);

        private ContactMsg(MessageDeserializer deserializer)
        {
            this.header = Std.HeaderMsg.Deserialize(deserializer);
            deserializer.Read(out this.f_norm);
            deserializer.Read(out this.contact);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.header);
            serializer.Write(this.f_norm);
            serializer.Write(this.contact);
        }

        public override string ToString()
        {
            return "ContactMsg: " +
            "\nheader: " + header.ToString() +
            "\nf_norm: " + f_norm.ToString() +
            "\ncontact: " + contact.ToString();
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
