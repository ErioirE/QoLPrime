﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace QoLPrime.Content.Players
{
    class PlayerModification : ModPlayer
    {
		static Projectile projectile;
		static bool backpackEnabled = true;
		static bool hasPrinted;
		float originalMoveSpeed = Main.LocalPlayer.moveSpeed;
		float originalTileSpeed = Main.LocalPlayer.tileSpeed;
		List<Item> backpackInventory = new List<Item>();
		
		//On.Terraria.Player.HandleBeingInChestRange += chestRangeHijack;
		public static void chestRangeHijack(On.Terraria.Player.orig_HandleBeingInChestRange orig, Player self)
        {
			if (self.chest != -5) {


				orig(self);

			}
        }
		public static void quickStackAllHijack(On.Terraria.Player.orig_QuickStackAllChests orig, Player self)
		{

			orig(self);
			if (backpackEnabled)
			{
				int num2 = 17;
				int num3 = (int)(self.Center.X / 16f);
				int num4 = (int)(self.Center.Y / 16f);
				for (int j = num3 - num2; j <= num3 + num2; j++)
				{
					if (j < 0 || j >= Main.maxTilesX)
						continue;

					for (int k = num4 - num2; k <= num4 + num2; k++)
					{
						if (k >= 0 && k < Main.maxTilesY)
						{
							int num5 = 0;
							if (Main.tile[j, k].type == 29)
								num5 = -2;
							else if (Main.tile[j, k].type == 97)
								num5 = -3;
							else if (Main.tile[j, k].type == 463)
								num5 = -4;
							else if (Main.tile[j, k].type == 491)
								num5 = -5;

							if (num5 < 0 && (new Vector2(j * 16 + 8, k * 16 + 8) - self.Center).Length() < 250f)
							{
								int num6 = self.chest;
								self.chest = num5;
								ChestUI.QuickStack();
								self.chest = num6;
							}
						}
					}
				}

				if (Main.netMode == 1)
				{
					for (int l = 10; l < 40; l++)
					{
						if (self.bank4.item[l].type > 0 && self.bank4.item[l].stack > 0 && !self.bank4.item[l].favorited && !self.bank4.item[l].IsACoin)
						{
							NetMessage.SendData(5, -1, -1, null, self.whoAmI, l, (int)self.bank4.item[l].prefix);
							NetMessage.SendData(85, -1, -1, null, l);
							self.inventoryChestStack[l] = true;
						}
					}

					return;
				}

				bool flag = false;
				for (int m = 10; m < 40; m++)
				{
					if (self.bank4.item[m].type > 0 && self.bank4.item[m].stack > 0 && !self.bank4.item[m].favorited && !self.bank4.item[m].IsACoin)
					{
						int type = self.bank4.item[m].type;
						int stack = self.bank4.item[m].stack;
						
						self.bank4.item[m] = Chest.PutItemInNearbyChest(self.bank4.item[m], self.Center);
						if (self.bank4.item[m].type != type || self.bank4.item[m].stack != stack)
							flag = true;
					}
				}
			}
		}
		public static void QuickStackHijack(On.Terraria.UI.ChestUI.orig_QuickStack orig)
        {
			orig();
            if (backpackEnabled)
            {
                Player player = Main.player[Main.myPlayer];
                

                Item[] inventory = player.bank4.item;
                Item[] item = player.bank.item;
                if (player.chest > -1)
                {
                    item = Main.chest[player.chest].item;
                }
                else if (player.chest == -2)
                {
                    item = player.bank.item;
                }
                else if (player.chest == -3)
                {
                    item = player.bank2.item;
                }
                else if (player.chest == -4)
                {
                    item = player.bank3.item;
                }

                List<int> list = new List<int>();
                List<int> list2 = new List<int>();
                List<int> list3 = new List<int>();
                Dictionary<int, int> dictionary = new Dictionary<int, int>();
                List<int> list4 = new List<int>();
                bool[] array = new bool[item.Length];
                for (int i = 0; i < 40; i++)
                {
                    if (item[i].type > 0 && item[i].stack > 0 && (item[i].type < 71 || item[i].type > 74))
                    {
                        list2.Add(i);
                        list.Add(item[i].netID);
                    }

                    if (item[i].type == 0 || item[i].stack <= 0)
                    {
                        list3.Add(i);
                    }
                }

                int num = 40;

                for (int j = 10; j < num; j++)
                {
                    if (list.Contains(inventory[j].netID) && !inventory[j].favorited)
                    {
                        dictionary.Add(j, inventory[j].netID);
                    }
                }

                for (int k = 0; k < list2.Count; k++)
                {
                    int num2 = list2[k];
                    int netID = item[num2].netID;
                    foreach (KeyValuePair<int, int> item2 in dictionary)
                    {
                        if (item2.Value == netID && inventory[item2.Key].netID == netID)
                        {
                            int num3 = inventory[item2.Key].stack;
                            int num4 = item[num2].maxStack - item[num2].stack;
                            if (num4 == 0)
                            {
                                break;
                            }

                            if (num3 > num4)
                            {
                                num3 = num4;
                            }

                            SoundEngine.PlaySound(7);
                            item[num2].stack += num3;
                            inventory[item2.Key].stack -= num3;
                            if (inventory[item2.Key].stack == 0)
                            {
                                inventory[item2.Key].SetDefaults();
                            }

                            array[num2] = true;
                        }
                    }
                }

                foreach (KeyValuePair<int, int> item3 in dictionary)
                {
                    if (inventory[item3.Key].stack == 0)
                    {
                        list4.Add(item3.Key);
                    }
                }

                foreach (int item4 in list4)
                {
                    dictionary.Remove(item4);
                }

                for (int l = 0; l < list3.Count; l++)
                {
                    int num5 = list3[l];
                    bool flag = true;
                    int num6 = item[num5].netID;
                    if (num6 >= 71 && num6 <= 74)
                    {
                        continue;
                    }

                    foreach (KeyValuePair<int, int> item5 in dictionary)
                    {
                        if ((item5.Value != num6 || inventory[item5.Key].netID != num6) && (!flag || inventory[item5.Key].stack <= 0))
                        {
                            continue;
                        }

                        SoundEngine.PlaySound(7);
                        if (flag)
                        {
                            num6 = item5.Value;
                            item[num5] = inventory[item5.Key];
                            inventory[item5.Key] = new Item();
                        }
                        else
                        {
                            int num7 = inventory[item5.Key].stack;
                            int num8 = item[num5].maxStack - item[num5].stack;
                            if (num8 == 0)
                            {
                                break;
                            }

                            if (num7 > num8)
                            {
                                num7 = num8;
                            }

                            item[num5].stack += num7;
                            inventory[item5.Key].stack -= num7;
                            if (inventory[item5.Key].stack == 0)
                            {
                                inventory[item5.Key] = new Item();
                            }
                        }

                        array[num5] = true;
                        flag = false;
                    }
                }

                if (Main.netMode == 1 && player.chest >= 0)
                {
                    for (int m = 0; m < array.Length; m++)
                    {
                        NetMessage.SendData(32, -1, -1, null, player.chest, m);
                    }
                }

                list.Clear();
                list2.Clear();
                list3.Clear();
                dictionary.Clear();
                list4.Clear();
            }
        }
		public override void PreUpdate()
        {
			backpackInventory = Player.bank4.item.ToList();
			if (Terraria.Main.playerInventory &&Player.chest == -1 && Player.talkNPC < 0 && backpackEnabled)
			{
				Player.chest = -5;
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
					Main.NewText($"{string.Join(',', backpackInventory[0].Name)}");
					Player.QuickStackAllChests();
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
