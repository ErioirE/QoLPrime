using System.IO;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace QoLPrime
{
    public class QoLPrime : Mod
	{
		public const string AssetPath = "QoLPrime/Assets/";
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
		public override void Load()
		{
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
}