using System.IO;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Utilities;
using Microsoft.Xna.Framework.Input;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Terraria;
using MonoMod.RuntimeDetour;
using QoLPrime.Content.Players;
using FluentIL;

namespace QoLPrime
{
    public class QoLPrime : Mod
	{
		public const string AssetPath = "QoLPrime/Assets/";
		public static int currentBiome;
		public static ModHotKey backpackToggle;
		public static ModHotKey printSpawnRate;
		public static MethodInfo chestMethodInfo;

		public static string checkSpawnRate { get; private set; }
		public static QoLPrime Instance { get; private set; }
		public QoLPrime()
		{
			QoLPrime.Instance = this;			
			
		}

		//TODO: Introduce OOP packets into tML, to avoid this god-class level hardcode.
		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			ExampleModMessageType msgType = (ExampleModMessageType)reader.ReadByte();

			
		}
		public static void setSpawnRateLabel(string label)
        {
			checkSpawnRate = label;
        }
		public override void Load()
		{
			backpackToggle = RegisterHotKey("Backpack Toggle", "OemTilde");
			printSpawnRate = RegisterHotKey("Print Spawn Rate", "OemBackslash");
			MonoModHooks.RequestNativeAccess();
			
			
			Hook chestRangeHook = new Hook(typeof(Terraria.Player).GetMethod("HandleBeingInChestRange", BindingFlags.NonPublic | BindingFlags.Instance), typeof(PlayerModification).GetMethod("chestRangeHijack"));
			chestRangeHook.Apply();
			On.Terraria.Player.HandleBeingInChestRange += PlayerModification.chestRangeHijack;
			On.Terraria.UI.ChestUI.QuickStack += PlayerModification.QuickStackHijack;
			On.Terraria.Player.QuickStackAllChests += PlayerModification.quickStackAllHijack;
		}

		public override void Unload()
		{
			QoLPrime.Instance = null;
		}
	}
	internal enum ExampleModMessageType : byte
	{
		ExamplePlayerSyncPlayer,
		ExampleTeleportToStatue
	}
	class SpawnRateMultiplierGlobalNPC : GlobalNPC
	{
		static float multiplier = 2f;
		public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
		{
			spawnRate = (int)(spawnRate / multiplier);

			maxSpawns = (int)(maxSpawns * multiplier);
			QoLPrime.setSpawnRateLabel($"Spawnrate: {spawnRate} Max spawns: {maxSpawns}");
		}
	}
}