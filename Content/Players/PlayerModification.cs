using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace QoLPrime.Content.Players
{
    class PlayerModification : ModPlayer
    {
		static Projectile projectile;
		static bool backpackEnabled = true;
		static bool hasPrinted;
		float originalMoveSpeed = Main.LocalPlayer.moveSpeed;
		float originalTileSpeed = Main.LocalPlayer.tileSpeed;
		
		//On.Terraria.Player.HandleBeingInChestRange += chestRangeHijack;
		public static void chestRangeHijack(On.Terraria.Player.orig_HandleBeingInChestRange orig, Terraria.Player self)
        {
			int chestToForce = 0;

            if (!(Main.LocalPlayer.talkNPC >=0))
            {
				chestToForce = -5;
            }
            else
            {
				chestToForce = Main.LocalPlayer.chest;
            }

			orig(self);

			if (chestToForce == -5)
            {
				Main.LocalPlayer.chest = chestToForce;
				Main.NewText($"{"Sneakedy snek'd"}");
			}

        }
		public override void PreUpdate()
        {
			if (Player.chest == -1 && Player.talkNPC < 0 && Player.controlInv)
			{
				//Player.chest = -5;
			}
			if (!hasPrinted)
			{
				/*Player.inventory[1] = new Item(ItemID.Marrow);
				Player.inventory[1].prefix = PrefixID.Unreal;
				hasPrinted = true;
				*/
			}
			

			//if (Player.moveSpeed <= originalMoveSpeed + 5)
			//{
			//	Player.moveSpeed = originalMoveSpeed * 7;
			//}
			//Player.QuickStackAllChests();
			/*if (backpackEnabled && Player.talkNPC < 0) {
				if (backpackEnabled && projectile == null || projectile.type != ProjectileID.VoidLens)
				{

					projectile = Projectile.NewProjectileDirect(Player.GetProjectileSource_Misc(Player.whoAmI), Player.position, Vector2.Zero, ProjectileID.VoidLens, 0, 0);

					projectile.ignoreWater = true;
				}
				else
				{
					if (Player.position.Y > 2200)
					{
						projectile.position.Y = Player.position.Y - 1500;
					}
					else
					{
						//Main.NewText("POS#2");
						projectile.position.Y = Player.position.Y;
					}

					projectile.timeLeft = int.MaxValue;
					projectile.position.X = Player.position.X;
					//projectile.position.Y = Player.position.Y-1000;
					projectile.height = 1510;
					projectile.width = 1;
					projectile.alpha = 255;
					projectile.light = -1;
					projectile.Opacity = 0;
					projectile.ignoreWater = true;
					//projectile.Center = new Vector2(Player.position.X,Player.position.Y-1500);
				}
				if (projectile.active == false && backpackEnabled)
				{
					projectile.active = true;
				}

				float distancex = Math.Abs(Main.MouseWorld.X - Player.Center.X);
				float distancey = Math.Abs(Main.MouseWorld.Y - Player.Center.Y);
				Rectangle mouseBox = new Rectangle((int)Main.MouseWorld.X - 10, (int)Main.MouseWorld.Y - 10, 20, 20);
				if (projectile.Hitbox.Intersects(mouseBox))
				{
					projectile.position.X += 50;

				}
				else
				{
					projectile.position.X = Player.position.X;
				}
				if (Player.chest == -1)
				{
					Player.chest = -5;
				}



				if (Player.chest == -5)
				{
					//Player.chest = -5;

					Player.voidLensChest = projectile.identity;

					Main.projectile[Player.voidLensChest] = projectile;

					//Player.chestX = (int)(((double)Player.position.X + (double)Player.width * 0.5) / 16.0);
					//Player.chestY = (int)(((double)Player.position.Y + (double)Player.height * 0.5) / 16.0);
					//IProjectileSource source;
					//source = Player.GetProjectileSource_Item(Item);
					//Projectile.NewProjectile(source,Player.position,Vector2.Zero,734,0,0);
					//if (Main.projectile[Player.voidLensChest]!= null) {
					//	Main.projectile[Player.voidLensChest].active = true;
					//	Main.projectile[Player.voidLensChest].type = 734;
					//}
					//Player.voidLensChest = 0;

					Player.IsVoidVaultEnabled = true;

					//Player.voidLensChest = 1;
					//hasPrinted = true;
				}
			}*/
            else
            {
				//Player.chest = -1;
            }


		}
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
			//DebugHotkeys(triggersSet);

			
			bool backpackTogglePressed = QoLPrime.backpackToggle.JustReleased;
            if (backpackTogglePressed)
            {
				backpackEnabled = !backpackEnabled;
				Main.NewText($"Backpack toggled to {backpackEnabled}");

				if (backpackEnabled)
                {
					Player.chest = -5;
				}
                else
                {
					Player.chest = -1;
                }
            }
            if (QoLPrime.printSpawnRate.JustReleased)
            {
				Main.NewText($"{QoLPrime.checkSpawnRate}");
			}
            base.ProcessTriggers(triggersSet);
        }
		public static void DebugHotkeys(TriggersSet triggersSet)
        {
			List<string> keysPressed = new List<string>();
			foreach (string key in triggersSet.KeyStatus.Keys)
			{
				if (triggersSet.KeyStatus[key])
				{
					keysPressed.Add(key);
				}
			}
			Main.NewText($"{string.Join(',', keysPressed)}");
		}

    }
}
