﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using ValheimPermissions;
using System.Linq;
using Steamworks;

namespace WorldofValheimZones
{

    public static class Util
    {
        public static List<Util.ConnectionData> Connections = new List<Util.ConnectionData>();

        public class ConnectionData
        {

            public ZRpc rpc;
        }
        public static float RestrictionCheckFloatReturn(string restriction)
        {
            string PlayerSteamID = SteamUser.GetSteamID().ToString();
            Player p = Player.m_localPlayer;
            // Are we in a zone? if so select that zone.
            ZoneHandler.Zone z = new ZoneHandler.Zone();
            ZoneHandler.ZoneTypes zt = new ZoneHandler.ZoneTypes();
            ZoneHandler.ZoneConfig zc = new ZoneHandler.ZoneConfig();
            List<ZoneHandler.Zone> zlist = ZoneHandler.ListOccupiedZones(p.transform.position);
            string ZoneType = "wilderness";
            if (zlist.Count == 0)
            {
                zt = ZoneHandler.FindZoneType("wilderness");
                ZoneHandler.ZoneConfig zonename = new ZoneHandler.ZoneConfig { Name = ZoneType };
                bool contains = ZoneHandler.ZoneC.Any(Name => Name.Name == zonename.Name);
                if (!contains)
                {
                    return 1;
                }
                else
                {
                    zc = ZoneHandler.FindZoneConfig("wilderness");
                }

            }
            else
            {
                z = ZoneHandler.TopZone(zlist);
                zt = ZoneHandler.FindZoneType(z.Type);
                ZoneType = z.Type;
                ZoneHandler.ZoneConfig zonename = new ZoneHandler.ZoneConfig { Name = ZoneType };
                bool contains = ZoneHandler.ZoneC.Any(Name => Name.Name == zonename.Name);
                if (!contains)
                {
                    return 1;
                }
                else
                {
                    zc = ZoneHandler.FindZoneConfig(z.Type);
                }

            }
            string key = "";
            string admins = "";
            // Lets set our admins and keys..
            admins = zc.Admins;
            key = zc.Configurations;
            // Lets see if the user is actually an admin in the zone first..
            if (admins.Contains(WorldofValheimZones.MySteamID))
            {
                // Ok they are an admin. Therefore, do not initialize the change...
                return 1;
            }
            if (key.ToLower().Contains(restriction))
            {
                string s = key.ToLower();
                string restrictioncheck = restriction + "(";
                int indexStart = s.IndexOf(restrictioncheck) + restrictioncheck.Length;
                string test = "";
                for (int i = indexStart; i < indexStart + 20; i++)
                {
                    if (s[i] == ')') break;
                    test += s[i];
                }
                float multiplier = 1;
                float.TryParse(test, out multiplier);
                return multiplier;
            }
            else
                return 1;
        }
        public static bool RestrictionCheck(string restriction)
        {
            Player p = Player.m_localPlayer;
            // Are we in a zone? if so select that zone.
            ZoneHandler.Zone z = new ZoneHandler.Zone();
            ZoneHandler.ZoneTypes zt = new ZoneHandler.ZoneTypes();
            ZoneHandler.ZoneConfig zc = new ZoneHandler.ZoneConfig();
            List<ZoneHandler.Zone> zlist = ZoneHandler.ListOccupiedZones(p.transform.position);
            string ZoneType = "wilderness";
            if (zlist.Count == 0)
            {
                zt = ZoneHandler.FindZoneType("wilderness");
                ZoneHandler.ZoneConfig zonename = new ZoneHandler.ZoneConfig { Name = ZoneType };
                bool contains = ZoneHandler.ZoneC.Any(Name => Name.Name == zonename.Name);
                if (!contains)
                {
                    return false;
                }
                else
                {
                    zc = ZoneHandler.FindZoneConfig("wilderness");
                }

            }
            else
            {
                z = ZoneHandler.TopZone(zlist);
                zt = ZoneHandler.FindZoneType(z.Type);
                ZoneType = z.Type;
                ZoneHandler.ZoneConfig zonename = new ZoneHandler.ZoneConfig { Name = ZoneType };
                bool contains = ZoneHandler.ZoneC.Any(Name => Name.Name == zonename.Name);
                if (!contains)
                {
                    return false;
                }
                else
                {
                    zc = ZoneHandler.FindZoneConfig(z.Type);
                }

            }
            string key = "";
            string admins = "";
            // Lets set our admin list and keys...
            admins = zc.Admins;
            key = zc.Configurations;
            // Lets check and see if the user is actually an admin in the zone.
            if (admins.Contains(WorldofValheimZones.MySteamID))
            {
                return false;
            }
            if (key.ToLower().Contains(restriction))
                return true;
            else
                return false;
        }
        public static void DownloadPAreasStart(long sender, ZPackage pkg)
        {
            if (pkg != null && pkg.Size() > 0 && !ZNet.instance.IsServer() && !ZNet.instance.IsDedicated())
            {
                WorldofValheimZones.ZonePermissions.Clear();
                int Count = pkg.ReadInt();
                for (int i = 0; i < Count; i++)
                {
                    if (pkg.ReadBool())
                    {

                        string ZoneType = pkg.ReadString();
                        Debug.Log($"Loading Configuration for Zone Type: {ZoneType}");
                        string configs = pkg.ReadString();
                        WorldofValheimZones.AreaInfo info = new WorldofValheimZones.AreaInfo();
                        info.configs = configs;
                        WorldofValheimZones.ZonePermissions.Add(ZoneType, info);
                        //print($"ADDED AREA {area},{info.range},{info.configs}");
                    }
                }
            }
        }
        public static void DoAreaEffect(Vector3 pos)
        {
            if (WorldofValheimZones.EffectTick <= 0)
            {
                WorldofValheimZones.EffectTick = 120;
                GameObject znet = ZNetScene.instance.GetPrefab("vfx_lootspawn");
                GameObject obj = UnityEngine.Object.Instantiate(znet, pos, Quaternion.identity);
                DamageText.WorldTextInstance worldTextInstance = new DamageText.WorldTextInstance();
                worldTextInstance.m_worldPos = pos;
                worldTextInstance.m_gui = UnityEngine.Object.Instantiate<GameObject>(DamageText.instance.m_worldTextBase, DamageText.instance.transform);
                worldTextInstance.m_textField = worldTextInstance.m_gui.GetComponent<Text>();
                DamageText.instance.m_worldTexts.Add(worldTextInstance);
                worldTextInstance.m_textField.color = Color.cyan;
                worldTextInstance.m_textField.fontSize = 24;
                worldTextInstance.m_textField.text = "PRIVATE AREA";
                worldTextInstance.m_timer = -2f;
            }
        }
        public static bool isServer()
        {
            return SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;
        }
        public static bool isAdmin(long sender)
        {
            ZNetPeer peer = ZNet.instance.GetPeer(sender);
            string SteamID = sender.ToString();
            if (
                ZNet.instance.m_adminList != null &&
                ZNet.instance.m_adminList.Contains(SteamID)
            )
                return true;
            else
            {
                return false;
            }
        }
        public static void Broadcast(string text, string username = ModInfo.Title)
        {
            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "ChatMessage", new object[]
            {
                new Vector3(0,100,0),
                2,
                username,
                text
            });
        }
        public static void RoutedBroadcast(long peer, string text, string username = ModInfo.Title)
        {
            ZRoutedRpc.instance.InvokeRoutedRPC(peer, "ChatMessage", new object[]
            {
                new Vector3(0,100,0),
                2,
                username,
                text
            });
        }
        public static void ReloadZones(long sender, ZPackage pkg)
        {
            ZNetPeer peer = ZNet.instance.GetPeer(sender);
            if (peer != null)
            {
                string permissionnode = "HackShardGaming.WoV-Zones.Reload";
                string peerSteamID = ((ZSteamSocket)peer.m_socket).GetPeerID().m_SteamID.ToString(); // Get the SteamID from peer.
                bool PlayerPermission = ValheimPermissions.ValheimDB.CheckUserPermission(peerSteamID, permissionnode);
                if (PlayerPermission)
                {
                    ZoneHandler.LoadZoneData(WorldofValheimZones.ZonePath.Value);
                    ZoneHandler.LoadZoneConfigurationData(WorldofValheimZones.ZoneConfigurationPath.Value);
                    Util.Broadcast("Reloading Zone");
                    Debug.Log("S2C ZoneHandler (SendPeerInfo)");
                    ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "WoV-Z-ZoneHandler", new object[] {
                        ZoneHandler.Serialize(peerSteamID)
                    }) ;
                }
                else
                {
                    RoutedBroadcast(sender, $"Sorry! You do not have the permission to use !ReloadZones (Required Permission: {permissionnode})");
                }
            }
        }
        public static void AddZone(long sender, ZPackage pkg)
        {
            if (pkg != null && pkg.Size() > 0)
            { // Check that our Package is not null, and if it isn't check that it isn't empty.
                ZNetPeer peer = ZNet.instance.GetPeer(sender); // Get the Peer from the sender, to later check the SteamID against our Adminlist.
                if (peer != null)
                { // Confirm the peer exists
                    string permissionnode = "HackShardGaming.WoV-Zones.Add";
                    string peerSteamID = ((ZSteamSocket)peer.m_socket).GetPeerID().m_SteamID.ToString(); // Get the SteamID from peer.
                    bool PlayerPermission = ValheimPermissions.ValheimDB.CheckUserPermission(peerSteamID, permissionnode);
                    if (PlayerPermission)
                        {
                        string msg = pkg.ReadString();
                        string[] results = msg.Split(' ');
                        string Name = results[0];
                        Debug.Log($"C-<S AddZone (RPC Call)");
                        string Type = results[1];
                        ZoneHandler.ZoneTypes zt = ZoneHandler.FindZoneType(results[1]);
                        if (zt.Name != Type)
                        {
                            msg = $"ERROR: The requested Zone Type {Type} does not exist!";
                            Util.RoutedBroadcast(sender, msg);
                            return;
                        }
                        int Priority = Int32.Parse(results[2]);
                        if (Priority < 1 || Priority > 5)
                        {
                            msg = $"ERROR: The requested Priority {Priority} is out of bounds! (Priorities are ranged from 1-5)!";
                            Util.RoutedBroadcast(sender, msg);
                            return;
                        }
                        string Shape = results[3];
                        if (Shape.ToLower() != "circle" && Shape.ToLower() != "square")
                        {
                            msg = $"ERROR: The requested Shape: {Shape} is incorrectly formated! (Shapes can either be circle or square only)";
                            Util.RoutedBroadcast(sender, msg);
                            return;
                        }
                        string X = results[4];
                        if (!X.Contains("."))
                        {
                            msg = $"ERROR: The requested X {X} is incorrectly formated! (Correct Format is 0.0)!";
                            Util.RoutedBroadcast(sender, msg);
                            return;
                        }
                        string Y = results[5];
                        if (!Y.Contains("."))
                        {
                            msg = $"ERROR: The requested Y {Y} is incorrectly formated! (Correct Format is 0.0)!";
                            Util.RoutedBroadcast(sender, msg);
                            return;
                        }
                        string R = results[6];
                        if (!R.Contains("."))
                        {
                            msg = $"ERROR: The requested Radius {R} is incorrectly formated! (Correct Format is 0.0)!";
                            Util.RoutedBroadcast(sender, msg);
                            return;
                        }
                        string addline = Name + " " + Type + " " + Priority + " " + Shape + " " + X + " " + Y + " " + R;
                        File.AppendAllText(WorldofValheimZones.ZonePath.Value, addline + Environment.NewLine);
                    }
                    else
                    {
                        Util.RoutedBroadcast(sender, $"Sorry! You do not have the permission to use !AddZone (Required Permission: {permissionnode})");
                        Debug.Log($"An unauthorized user {peerSteamID} attempted to use the AddZone RPC!");
                        string msg = pkg.ReadString();
                        Debug.Log($"Here is a log of the attempted AddZone {msg}");
                    }
                }
            }
        }

        public static Util.ConnectionData GetServer()
        {
            Debug.Assert(!ZNet.instance.IsServer());
            Debug.Assert(Util.Connections.Count == 1);
            return Util.Connections[0];
        }
        

    }
}