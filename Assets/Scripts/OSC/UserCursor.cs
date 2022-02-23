using System;
using UnityEngine;

namespace UnityOSC
{
    [Serializable]
    public class UserCursor
    {
        // Ip address of this user.
        public string ip;
        public Vector3 pos;
        public GameObject instance;

        public UserCursor(string ip, GameObject obj, Vector3 origin)
        {
            this.ip = ip;
            instance = obj;
            pos = origin;
        }
    }
}
