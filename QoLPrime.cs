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
using Terraria.UI;
using QoLPrime.Content.UI;
using Terraria.DataStructures;
using System.Collections.Generic;

namespace QoLPrime
{
    public class QoLPrime : Mod
	{
		public const string AssetPath = "QoLPrime/Assets/";
		public static int currentBiome;
		static Texture2D value = TextureAssets.InventoryBack.Value;
		public static int invBottom = (int)((value.Bounds.Height*6.5f)*Main.inventoryScale);
		public static int invBottomOffset = 165;
		public static ModHotKey backpackToggle;
		public static ModHotKey printSpawnRate;
		public static ModHotKey quickStackHotkey;
		public static bool inventoryOffsetAdjusted = false;
		public static string checkSpawnRate { get; private set; }
		public static QoLPrime Instance { get; private set; }
		public static Player qolPlayer = Main.LocalPlayer;
		public static int myPlayer = Main.myPlayer;


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
			MethodInfo trashMethodInfo = typeof(Main).GetMethod("DrawTrashItemSlot", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static |BindingFlags.Public);
			if (trashMethodInfo != null) {

				Hook drawTrashHook = new Hook(trashMethodInfo, typeof(QoLPrime).GetMethod("drawTrashHijack"));
				drawTrashHook.Apply();
				On.Terraria.Main.DrawTrashItemSlot += QoLPrime.drawTrashHijack;
			}
			MethodInfo bestMethodInfo = typeof(Main).GetMethod("DrawBestiaryIcon", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
			if (bestMethodInfo != null)
			{

				Hook drawBestHook = new Hook(bestMethodInfo, typeof(QoLPrime).GetMethod("drawBestHijack"));
				drawBestHook.Apply();
				On.Terraria.Main.DrawBestiaryIcon += QoLPrime.drawBestHijack;
			}
			MethodInfo emoteMethodInfo = typeof(Main).GetMethod("DrawEmoteBubblesButton", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
			if (emoteMethodInfo != null)
			{

				Hook drawEmoteHook = new Hook(emoteMethodInfo, typeof(QoLPrime).GetMethod("drawEmoteHijack"));
				drawEmoteHook.Apply();
				On.Terraria.Main.DrawEmoteBubblesButton += QoLPrime.drawEmoteHijack;
			}
			//Hook ChestUIDrawHook = new Hook(typeof(ChestUI).GetMethod("Draw"), typeof(QoLPrime).GetMethod("ChestUIDrawHijack"));
			On.Terraria.Player.HandleBeingInChestRange += PlayerModification.chestRangeHijack;
			On.Terraria.UI.ChestUI.QuickStack += PlayerModification.QuickStackHijack;
			On.Terraria.Player.QuickStackAllChests += PlayerModification.quickStackAllHijack;
			On.Terraria.Main.DrawInventory += QoLPrime.drawInventoryHijack;
			On.Terraria.Main.DrawNPCs += QoLPrime.drawNpcsHijack;
			On.Terraria.UI.ChestUI.Draw += QoLPrime.ChestUIDrawHijack;

		}

		public override void Unload()
		{
			QoLPrime.Instance = null;
		}
		public static void drawBestHijack(On.Terraria.Main.orig_DrawBestiaryIcon orig, int pivotTopLeftX, int pivotTopLeftY)
        {
            if (!PlayerModification.backpackEnabled)
            {
				orig(pivotTopLeftX,pivotTopLeftY);
            }
        }
		public static void drawEmoteHijack(On.Terraria.Main.orig_DrawEmoteBubblesButton orig, int pivotTopLeftX, int pivotTopLeftY)
		{
			if (!PlayerModification.backpackEnabled)
			{
				orig(pivotTopLeftX, pivotTopLeftY);
			}
		}
		public static void drawTrashHijack(On.Terraria.Main.orig_DrawTrashItemSlot orig, int pivotTopLeftX, int pivotTopLeftY)
        {
			myPlayer = Main.myPlayer;
			Player[] player = Main.player;
			qolPlayer = Main.LocalPlayer;
			Point16 trashSlotOffset = Main.trashSlotOffset;
			int mouseX = Main.mouseX;
			int mouseY = Main.mouseY;
			Main.inventoryScale = 0.85f;



			Main.inventoryScale = 0.85f;
			int num = 448 + pivotTopLeftX;
			int num2 = 258 + pivotTopLeftY;

			if (PlayerModification.backpackEnabled)
			{
				num2 += invBottomOffset;
			}

			if ((player[myPlayer].chest != -1 || Main.npcShop > 0) && !Main.recBigList)
			{
				num2 += 168;
				Main.inventoryScale = 0.755f;
				num += 5;
			}
			else if ((player[myPlayer].chest == -1 || Main.npcShop == -1) && trashSlotOffset != Point16.Zero)
			{
				num += trashSlotOffset.X;
				num2 += trashSlotOffset.Y;
				Main.inventoryScale = 0.755f;
			}
			Rectangle trashInvSlotBounds = new Rectangle(num, num2, (int)(TextureAssets.InventoryBack.Width() * Main.inventoryScale), (int)(TextureAssets.InventoryBack.Height() * Main.inventoryScale));

			new Microsoft.Xna.Framework.Color(150, 150, 150, 150);
			Point mousePos = new Point(mouseX, mouseY);

			if (trashInvSlotBounds.Contains(mousePos) && !PlayerInput.IgnoreMouseInterface)
			{
				player[myPlayer].mouseInterface = true;
				ItemSlot.LeftClick(ref player[myPlayer].trashItem, 6);
				if (Main.mouseLeftRelease && Main.mouseLeft)
					Recipe.FindRecipes();

				ItemSlot.MouseHover(ref player[myPlayer].trashItem, 6);
			}

			ItemSlot.Draw(Main.spriteBatch, ref player[myPlayer].trashItem, 6, new Vector2(num, num2));
			//Texture2D _texture = new Texture2D(Main.graphics.GraphicsDevice, 4, 4);
			//Main.spriteBatch.Draw(_texture, trashInvSlotBounds.TopLeft(), null, Color.Red, 0f, Vector2.Zero, trashInvSlotBounds.Size() / 4f, SpriteEffects.None, 1f);

		}
		public static void ChestUIDrawHijack(On.Terraria.UI.ChestUI.orig_Draw orig, SpriteBatch spriteBatch)
        {
			orig(spriteBatch);
			DrawCustomChestUI.Draw(spriteBatch);
        }
		public static void drawNpcsHijack(On.Terraria.Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
			orig(self, behindTiles);
			int xOffset = 0;
			int yOffset = 15;
			float scaleMult = NightsBlood.scaleToAddToIcon;
			Texture2D manaTexture = RavenousBuff.textureToUseForMana.Value;
			Texture2D lifeTexture = RavenousBuff.textureToUseForLife.Value;
			Color iconColorIfMana = new Color(255,255,255) * PlayerModification.fadeMultipler;
			Color iconColorIfLife = new Color(255,255,255) * PlayerModification.fadeMultipler; ;
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);
			if (Main.LocalPlayer.HasBuff(ModContent.BuffType<RavenousBuff>())&& !NightsBlood.willHurt)
			{
				Texture2D textureToUse = RavenousBuff.textureToUseForLife.Value;
				Main.spriteBatch.Draw(manaTexture, new Vector2(Main.screenWidth/2- ((manaTexture.Width*scaleMult+Player.defaultWidth)/2), Main.screenHeight/2 - ((manaTexture.Height * scaleMult)+(Player.defaultHeight + yOffset))), new Microsoft.Xna.Framework.Rectangle(0, 0, RavenousBuff.textureToUseForMana.Width(), RavenousBuff.textureToUseForMana.Height()), iconColorIfMana,0f, default(Vector2), 1f+scaleMult, SpriteEffects.None,0f);
			}
			else if (Main.LocalPlayer.HasBuff(ModContent.BuffType<RavenousBuff>()) && NightsBlood.willHurt)
            {
				
				Main.spriteBatch.Draw(lifeTexture, new Vector2(Main.screenWidth / 2 - ((lifeTexture.Width*scaleMult+Player.defaultWidth)/2), Main.screenHeight / 2 - ((lifeTexture.Height * scaleMult)+(Player.defaultHeight + yOffset))), new Microsoft.Xna.Framework.Rectangle(0, 0, RavenousBuff.textureToUseForLife.Width(), RavenousBuff.textureToUseForLife.Height()), iconColorIfLife, 0f, default(Vector2), 1f+scaleMult, SpriteEffects.None, 0f);
			}
		}
		public static void drawInventoryHijack(On.Terraria.Main.orig_DrawInventory orig, Main self)
		{
				orig(self);
            if (PlayerModification.backpackEnabled)
            {
				Main.instance.invBottom = QoLPrime.invBottom + invBottomOffset;
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