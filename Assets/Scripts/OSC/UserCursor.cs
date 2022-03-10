using System;
using UnityEditor;
using UnityEngine;

namespace UnityOSC
{
    [Serializable]
    public class UserCursor
    {
        // Ip address of this user.
        public GUID id;
        public Vector3 pos;
        public GameObject instance;

        public UserCursor(GUID id, GameObject obj, Vector3 origin)
        {
            this.id = id;
            instance = obj;
            pos = origin;
        }
    }
}
