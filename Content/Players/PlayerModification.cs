using QoLPrime.Content.Buffs;
using QoLPrime.Items;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace QoLPrime.Content.Players
{
    class PlayerModification : ModPlayer
    {
        //public static TagCompound savedData;

        static bool hasPrinted = false;

        public static QuillRain mostRecentQuillRain;
        public static int updateCounter = 0;
        public static float fadeMultipler = 0f;
        public static PlayerModification instance;

        public PlayerModification()
        {
            instance = this;


            /* foreach (string file in Directory.GetFiles(QoLPrime.dataDir))
             {
                 var ob = TagIO.FromFile(file, false);
                 var whoa = TagIO.Deserialize<Item>(ob);
                 if (whoa != null)
                 {
                     QoLPrime.Instance.Logger.Info(whoa);
                 }
             }*/
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
            if (self.chest != -5)
            {


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
                            if (Main.tile[j, k].TileType == 29)
                                num5 = -2;
                            else if (Main.tile[j, k].TileType == 97)
                                num5 = -3;
                            else if (Main.tile[j, k].TileType == 463)
                                num5 = -4;
                            else if (Main.tile[j, k].TileType == 491)
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


        public override void PreUpdate()
        {
            if (QoLPrime.LastPlayer != Player.name)
            {
                QoLPrime.LastPlayer = Player.name;
            }
            if (Main.anglerQuestFinished == true)
            {
                Main.AnglerQuestSwap();
                Main.anglerQuestFinished = false;
            }





            //Mod.Logger.Info($"{string.Join(',',QoLPrime.backpackPublic.Keys)}");
            if (!Player.HasBuff(ModContent.BuffType<RavenousBuff>()))
            {
                RavenousBuff.counter = 0;
            }

            //Player.IsVoidVaultEnabled = true;



            if (!hasPrinted)
            {
                //Player.inventory[4] = new Item(ModContent.ItemType<QuillRain>());
                //Player.inventory[1].prefix = PrefixID.Unreal;
                hasPrinted = true;

            }




            updateCounter++;
            if (Player.HasBuff(ModContent.BuffType<RavenousBuff>()))
            {
                fadeMultipler = 0.1f + (((Math.Abs(updateCounter - 30) * 30) / 90) / 10f);
            }
            //Main.NewText(fadeMultipler.ToString());
            if (updateCounter >= 60)
            {
                updateCounter = 0;
            }

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



            if (QoLPrime.printSpawnRate.JustReleased)
            {
                Main.NewText($"{QoLPrime.checkSpawnRate}");
                QoLPrime.Instance.Logger.Info($"{string.Join(',', QoLPrime.settings)}");
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
