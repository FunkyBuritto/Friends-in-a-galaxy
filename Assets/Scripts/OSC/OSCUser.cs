using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityOSC
{
    public enum UserRole { Driver = 0, Gunner = 1, Spectator = 2 }

    [Serializable]
    public class OSCUser
    {
        // Ip address of this user.
        public string ip;
        public GUID id;
        public UserRole role;
        public Coroutine timeout;

        public OSCUser(string ip, UserRole role)
        {
            this.ip = ip;
            this.role = role;
            id = GUID.Generate();
        }

        /// <summary>
        /// Add a hook to the OSC server for this specific user.
        /// DO NOT ADD /Device/ infront of the <param name="address">address</param>.
        /// </summary>
        /// <param name="address">The OSC address {type}</param>
        /// <param name="handler">The callback for when a message is recieved</param>
        public void AddHook(string address, Action<ArrayList> handler)
        {
            OSCHandler.AddUserHook($"/{Constants.UUID}/" + address, ip, (msg) => { handler.Invoke(msg.values); });
        }

        public static OSCUser GetDriver() => LobbyManager.instance.users.Values.First(u => u.role == UserRole.Driver);
        public static OSCUser GetGunner() => LobbyManager.instance.users.Values.First(u => u.role == UserRole.Gunner);
    }
}
