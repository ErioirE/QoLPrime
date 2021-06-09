using Microsoft.Xna.Framework;
using QoLPrime.Content.Buffs;
using QoLPrime.Items;
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
using System.Linq;
using Terraria.ModLoader.IO;

namespace QoLPrime.Content.Players
{
	class PlayerModification : ModPlayer
	{
		//public static TagCompound savedData;
		static bool firstRun = true;
		public static bool backpackEnabled = true;
		static bool hasPrinted = false;
		
		public static QuillRain mostRecentQuillRain;
		public static int updateCounter = 0;
		public static float fadeMultipler = 0f;
		public static PlayerModification instance;
		public static string name;
		public static Chest backpack;
        public PlayerModification()
        {
			instance = this;
			
			//backpack = new Chest();
			//backpack.item = QoLPrime.newEmptyChest();
		}
		public override void Initialize()
		{




		}
		public override void Load()
        {

			
		}



		//On.Terraria.Player.HandleBeingInChestRange += chestRangeHijack;
		/*public static void chestRangeHijack(On.Terraria.Player.orig_HandleBeingInChestRange orig, Player self)
        {
			if (self.chest != -5) {


				orig(self);

			}
        }*/
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
								//Main.NewText($"Items to QS from backpack, attempting detour...");
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
					for (int l = 0; l < 40; l++)
					{
						if (PlayerModification.backpack.item[l].type > 0 && PlayerModification.backpack.item[l].stack > 0 && !PlayerModification.backpack.item[l].IsACoin)
						{
							NetMessage.SendData(5, -1, -1, null, self.whoAmI, l, (int)PlayerModification.backpack.item[l].prefix);
							NetMessage.SendData(85, -1, -1, null, l);
							self.inventoryChestStack[l] = true;
						}
					}

					return;
				}

				bool flag = false;
				for (int m = 0; m < 40; m++)
				{
					
					if (PlayerModification.backpack.item[m].type > 0 && PlayerModification.backpack.item[m].stack > 0 && !PlayerModification.backpack.item[m].IsACoin)
					{
						int type = PlayerModification.backpack.item[m].type;
						int stack = PlayerModification.backpack.item[m].stack;
						//Main.NewText($"Items to QS from backpack, attempting detour...{PlayerModification.backpack.item[m].
						//
						//} * {PlayerModification.backpack.item[m].stack}");

						Item itemTransferred = Chest.PutItemInNearbyChest(PlayerModification.backpack.item[m], self.Center);
						//Main.NewText($"Item after transfer(?)...{itemTransferred.Name} * {itemTransferred.stack}");
						PlayerModification.backpack.item[m] = itemTransferred;
						if (PlayerModification.backpack.item[m].type != type || PlayerModification.backpack.item[m].stack != stack)
							flag = true;
					}
				}
				if (flag)
					SoundEngine.PlaySound(7);
			}
		}
		public static void QuickStackHijack(On.Terraria.UI.ChestUI.orig_QuickStack orig)
        {

			Player player = Main.player[Main.myPlayer];
			int startingChest = player.chest;
			orig();
			player.chest = startingChest;
			if (backpackEnabled)
            {
                Item[] inventory = PlayerModification.backpack.item;
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
                    if (list.Contains(inventory[j].netID))
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
            if (firstRun)
            {
				if (!QoLPrime.backpackPublic.ContainsKey(this.Player.name))
				{
					backpack = new Chest();
					for (int i = 0; i < backpack.item.Length; i++)
					{
						backpack.item[i] = new Item();
					}

				}
				else
				{				
					backpack.item = QoLPrime.backpackPublic.GetValueOrDefault(this.Player.name);
				}
				firstRun = false;
			}
			
			QoLPrime.backpackPublic[this.Player.name] = backpack.item;
			//Mod.Logger.Info($"{string.Join(',',QoLPrime.backpackPublic.Keys)}");
			if (!Player.HasBuff(ModContent.BuffType<RavenousBuff>()))
			{
				RavenousBuff.counter = 0;
			}
			if (backpackEnabled && updateCounter == 30)
            {
				int temp = Player.chest;
				//Player.chest = -5;
				//ChestUI.QuickStack();
				Player.chest = temp;
				if (QoLPrime.invBottom == 0) {
					//QoLPrime.invBottom = Main.instance.invBottom;
				}
				
            }
			//Player.IsVoidVaultEnabled = true;
			

			if (PlayerInput.Triggers.JustPressed.Inventory && Player.chest == -1 && Player.talkNPC < 0 && backpackEnabled)
			{
				//Player.chest = -5;
			}
			if (!hasPrinted)
			{
				//Player.inventory[4] = new Item(ModContent.ItemType<QuillRain>());
				//Player.inventory[1].prefix = PrefixID.Unreal;
				hasPrinted = true;
				
			}


			

			updateCounter++;
			if (Player.HasBuff(ModContent.BuffType<RavenousBuff>())) {
				fadeMultipler = 0.1f+(((Math.Abs(updateCounter - 30) * 30) / 90) / 10f);
			}
			//Main.NewText(fadeMultipler.ToString());
			if (updateCounter >= 60)
            {
				updateCounter = 0;
            }
			firstRun = false;
		}
		public static int? GetIndexOfItemInInventory(ModItem item, Player player)
        {
			int i = 0;
            foreach (Item invItem in player.inventory)
            {
                if (invItem == item.Item)
                {
					return i;
                }
				i++;
            }
			return null;
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
			//DebugHotkeys(triggersSet);

			
			bool backpackTogglePressed = QoLPrime.backpackToggle.JustPressed;
            if (backpackTogglePressed)
            {
				backpackEnabled = !backpackEnabled;
				Main.NewText($"Backpack toggled to {backpackEnabled}");

				if (backpackEnabled)
                {
					//Player.chest = -5;
					//Main.NewText($"{string.Join(',', backpackInventory[0].Name)}");
					
				}
                else
                {
					Player.chest = -1;
				}
            }
            if (QoLPrime.printSpawnRate.JustReleased)
            {
				Main.NewText($"{QoLPrime.checkSpawnRate}");
				Main.NewText(QoLPrime.backpackPublic.Count());
			}
			if (QoLPrime.quickStackHotkey.JustPressed)
			{
				if (Main.LocalPlayer.chest != -1)
				{
					ChestUI.QuickStack();
				}
				else
				{
					Player.QuickStackAllChests();
				}
			}
			bool depositAllPressed = QoLPrime.depositAllHotkey.JustPressed;
			if (depositAllPressed)
			{ ChestUI.DepositAll(); }
			bool lootAllPressed = QoLPrime.lootAllHotkey.JustPressed;
			if (lootAllPressed)
			{ ChestUI.LootAll(); }
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
