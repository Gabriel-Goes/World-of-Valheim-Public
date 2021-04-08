﻿using HarmonyLib;

namespace WorldofValheimServerSideCharacters
{
	// Debug Patch Class
	[HarmonyPatch]
	public static class Debug
	{

		public static void Assert(bool cond)
		{
		}

		public static void Log(string str)
		{
			System.Console.WriteLine("World of Valheim Server Side Characters: " + str);
		}

    }
}
