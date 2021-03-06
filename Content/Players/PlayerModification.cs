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
        public static int? GetIndexOfItemInInventory(Item item, Player player)
        {
            int i = 0;
            foreach (Item invItem in player.inventory)
            {
                if (invItem == item)
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
