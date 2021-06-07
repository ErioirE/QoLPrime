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
using Terraria.GameContent;
using Terraria.UI.Gamepad;
using Microsoft.Xna.Framework;
using Terraria.GameInput;
using Terraria.Localization;
using Microsoft.Xna.Framework.Graphics;
using QoLPrime.Content.Buffs;
using QoLPrime.Items;

namespace QoLPrime
{
    public class QoLPrime : Mod
	{
		public const string AssetPath = "QoLPrime/Assets/";
		public static int currentBiome;

		public static ModHotKey backpackToggle;
		public static ModHotKey printSpawnRate;
		public static ModHotKey quickStackHotkey;

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
			quickStackHotkey = RegisterHotKey("Quick Stack/Quick Stack all", "OemSemicolon");
			backpackToggle = RegisterHotKey("Toggle Backpack", "OemTilde");
			printSpawnRate = RegisterHotKey("Print Spawn Rate", "OemBackslash");
			
			
			MonoModHooks.RequestNativeAccess();
			
			Hook chestRangeHook = new Hook(typeof(Player).GetMethod("HandleBeingInChestRange", BindingFlags.NonPublic | BindingFlags.Instance), typeof(PlayerModification).GetMethod("chestRangeHijack"));
			chestRangeHook.Apply();
			Hook drawInvHook = new Hook(typeof(Main).GetMethod("DrawInventory", BindingFlags.NonPublic | BindingFlags.Instance), typeof(QoLPrime).GetMethod("drawInventoryHijack"));
			drawInvHook.Apply();
			Hook drawNpcHook = new Hook(typeof(Main).GetMethod("DrawNPCs", BindingFlags.NonPublic | BindingFlags.Instance), typeof(QoLPrime).GetMethod("drawNpcsHijack"));
			drawNpcHook.Apply();

			On.Terraria.Player.HandleBeingInChestRange += PlayerModification.chestRangeHijack;
			On.Terraria.UI.ChestUI.QuickStack += PlayerModification.QuickStackHijack;
			On.Terraria.Player.QuickStackAllChests += PlayerModification.quickStackAllHijack;
			On.Terraria.Main.DrawInventory += QoLPrime.drawInventoryHijack;
			On.Terraria.Main.DrawNPCs += QoLPrime.drawNpcsHijack;
		}

		public override void Unload()
		{
			QoLPrime.Instance = null;
		}
		public static void drawNpcsHijack(On.Terraria.Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
			orig(self, behindTiles);
			int xOffset = 10;
			int yOffset = -15;
			Color iconColorIfMana = new Color(255,255,255) * PlayerModification.fadeMultipler;
			Color iconColorIfLife = new Color(255,255,255) * PlayerModification.fadeMultipler; ;
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);
			if (Main.LocalPlayer.HasBuff(ModContent.BuffType<RavenousBuff>())&& !NightsBlood.willHurt)
			{
				Texture2D textureToUse = RavenousBuff.textureToUseForLife.Value;
				Main.spriteBatch.Draw(RavenousBuff.textureToUseForMana.Value, new Vector2(Main.screenWidth/2-Player.defaultWidth+xOffset, Main.screenHeight/2 - Player.defaultHeight + yOffset), new Microsoft.Xna.Framework.Rectangle(0, 0, RavenousBuff.textureToUseForMana.Width(), RavenousBuff.textureToUseForMana.Height()), iconColorIfMana,0f, default(Vector2), 1f, SpriteEffects.None, 0f);
			}
			else if (Main.LocalPlayer.HasBuff(ModContent.BuffType<RavenousBuff>()) && NightsBlood.willHurt)
            {
				
				Main.spriteBatch.Draw(RavenousBuff.textureToUseForLife.Value, new Vector2(Main.screenWidth / 2 - Player.defaultWidth+xOffset, Main.screenHeight / 2 - Player.defaultHeight+yOffset), new Microsoft.Xna.Framework.Rectangle(0, 0, RavenousBuff.textureToUseForLife.Width(), RavenousBuff.textureToUseForLife.Height()), iconColorIfLife, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
			}
		}
		public static void drawInventoryHijack(On.Terraria.Main.orig_DrawInventory orig, Main self)
		{
				orig(self);

			if (PlayerModification.backpackEnabled && Main.LocalPlayer.chest == -5) {
				int num108 = 0;
				int num109 = 498;
				int num110 = 410;
				int num111 = TextureAssets.ChestStack[num108].Width();
				int num112 = TextureAssets.ChestStack[num108].Height();
				UILinkPointNavigator.SetPosition(301, new Vector2((float)num109 + (float)num111 * 0.75f, (float)num110 + (float)num112 * 0.75f));
				if (Main.mouseX >= num109 && Main.mouseX <= num109 + num111 && Main.mouseY >= num110 && Main.mouseY <= num110 + num112 && !PlayerInput.IgnoreMouseInterface)
				{
					num108 = 1;
					if (!Main.allChestStackHover)
					{
						SoundEngine.PlaySound(12);
						Main.allChestStackHover = true;
					}

					if (Main.mouseLeft && Main.mouseLeftRelease)
					{
						Main.mouseLeftRelease = false;
						Main.player[Main.myPlayer].QuickStackAllChests();
						Recipe.FindRecipes();
					}

					Main.player[Main.myPlayer].mouseInterface = true;
				}
				else if (Main.allChestStackHover)
				{
					SoundEngine.PlaySound(12);
					Main.allChestStackHover = false;
				}

				Main.spriteBatch.Draw(TextureAssets.ChestStack[num108].Value, new Vector2(num109, num110), new Microsoft.Xna.Framework.Rectangle(0, 0, TextureAssets.ChestStack[num108].Width(), TextureAssets.ChestStack[num108].Height()), Microsoft.Xna.Framework.Color.White, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
				if (!Main.mouseText && num108 == 1)
					self.MouseText(Language.GetTextValue("GameUI.QuickStackToNearby"), 0, 0);
			}

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