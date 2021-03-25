
namespace ValheimOnline
{
    // 
    // The state of the client to follow
    // upon connecting, will sync with the server and enforce the settings provided.
    // 

    // Example to add client class for RPC
    //
    //   Register the RPC call on the client
    //     This is done in the OnNewConnection Method
    //
    //     peer.m_rpc.Register<ZPackage>("Client", new Action<ZRpc, ZPackage>(Client.RPC) );
    //
    //   Tell the server to call the RPC
    //     This is done in the SendPeerInfo Method
    //
    //      Debug.Log("S2C ClientState (SendPeerInfo)");
    //      Client._debug();
    //      rpc.Invoke("Client", new object[] {
    //          Client.Serialize()
    //      });
    //

public static class Client
    {
        // Zone Details
        public static bool InSafeZone = false;
        public static bool InBattleZone = false;

        // PVP Details
        // Can we toggle PVP on the client
        public static bool PVPEnforced = false;
        // Are we enforcing PVP On (TRUE) or Off (FALSE)
        public static bool PVPisEnabled = false;

        // Current PVP mode for the client
        public static bool PVPMode = false;

        // Show our position on the map
        public static bool PVPSharePosition = false;
        public static bool PositionEnforced = false;

        // Generic debug output
        // Do not change name to debug. Will break "debug()" function in class.
#if DEBUG
        public static void _debug()
        {
            Debug.Log("Loaded Client Data: ");
            Debug.Log("  InSafeZone: " + Client.InSafeZone);
            Debug.Log("  InBattleZone: " + Client.InBattleZone);
            Debug.Log("  PVPEnforced: " + Client.PVPEnforced);
            Debug.Log("  PVPisEnabled: " + Client.PVPisEnabled);
            Debug.Log("  PVPMode: " + Client.PVPMode);
            Debug.Log("  PositionEnforced: " + Client.PositionEnforced);
            Debug.Log("  PVPSharePosition: " + Client.PVPSharePosition);
        }
#endif

        // Compress the data into a zip (compressed) stream (serial)
        public static ZPackage Serialize()
        {
            var zip = new ZPackage();
            zip.Write(Client.PVPEnforced);
            zip.Write(Client.PVPisEnabled);
            zip.Write(Client.InSafeZone);
            zip.Write(Client.InBattleZone);
            zip.Write(Client.PVPMode);
            zip.Write(Client.PositionEnforced);
            zip.Write(Client.PVPSharePosition);
            return zip;
        }

        // Extract the data from the zipped data
        public static void Deserialize(ZPackage data)
        {
            Client.PVPEnforced = data.ReadBool();
            Client.PVPisEnabled = data.ReadBool();
            Client.InSafeZone = data.ReadBool();
            Client.InBattleZone = data.ReadBool();
            Client.PVPMode = data.ReadBool();
            Client.PositionEnforced = data.ReadBool();
            Client.PVPSharePosition = data.ReadBool();
        }

        // RPC function class. This is the class that you register to receive rpc data.
        public static void RPC(ZRpc rpc, ZPackage data)
        {
            Debug.Log("S2C Client (RPC Call)");
            Debug.Assert(!ZNet.instance.IsServer());
#if DEBUG
            Debug.Log("Before");
            _debug();
#endif
            Deserialize(data);
#if DEBUG
            Debug.Log("After");
            _debug();
#endif
        }
    }
}