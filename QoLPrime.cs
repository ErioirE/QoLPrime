using QoLPrime;
using QoLPrime.Content;
using QoLPrime.Content.Items.Consumables;
using QoLPrime.Content.NPCs;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace QoLPrime
{
	public class QoLPrime : Mod
	{
		public const string AssetPath = "QoLPrime/Assets/";
		public override void AddRecipes() => ExampleRecipes.Load(this);

		public override void Unload() => ExampleRecipes.Unload();

		//TODO: Introduce OOP packets into tML, to avoid this god-class level hardcode.
		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			ExampleModMessageType msgType = (ExampleModMessageType)reader.ReadByte();

			switch (msgType)
			{
				// This message syncs ExamplePlayer.exampleLifeFruits
				case ExampleModMessageType.ExamplePlayerSyncPlayer:
					byte playernumber = reader.ReadByte();
					ExampleLifeFruitPlayer examplePlayer = Main.player[playernumber].GetModPlayer<ExampleLifeFruitPlayer>();
					examplePlayer.exampleLifeFruits = reader.ReadInt32();
					// SyncPlayer will be called automatically, so there is no need to forward this data to other clients.
					break;
				case ExampleModMessageType.ExampleTeleportToStatue:
					if (Main.npc[reader.ReadByte()].ModNPC is ExamplePerson person && person.NPC.active)
					{
						person.StatueTeleport();
					}

					break;
				default:
					Logger.WarnFormat("ExampleMod: Unknown Message type: {0}", msgType);
					break;
			}
		}

	}
	internal enum ExampleModMessageType : byte
	{
		ExamplePlayerSyncPlayer,
		ExampleTeleportToStatue
	}
	
}