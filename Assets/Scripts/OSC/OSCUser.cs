namespace UnityOSC
{
    [System.Serializable]
    public class OSCUser
    {
        // Ip address of this user.
        public string ip;

        public OSCUser(string ip)
        {
            this.ip = ip;
        }

        /// <summary>
        /// Add a hook to the OSC server for this specific user.
        /// DO NOT ADD /Device/ infront of the <param name="address">address</param>.
        /// </summary>
        /// <param name="address">The OSC address {type}</param>
        /// <param name="handler">The callback for when a message is recieved</param>
        public void AddHook(string address, OscMessageHandler handler)
        {
            OSCHandler.AddUserHook("/Device/" + address, ip, handler);
        }

        public static OSCUser GetDriver() => LobbyManager.instance.users[0];
        public static OSCUser GetGunner() => LobbyManager.instance.users[1];
    }
}
