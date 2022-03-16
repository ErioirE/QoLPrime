using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QoLPrime.Content.Players;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace QoLPrime
{
    public class QoLPrime : Mod
    {
        public static string LastPlayer = "UNSET";
        public const string AssetPath = "QoLPrime/Assets/";
        public static int currentBiome;
        static Texture2D value;
        public static int invBottom;
        public static int invBottomOffset = 165;
        public static ModKeybind backpackToggle;
        public static ModKeybind printSpawnRate;
        public static ModKeybind quickStackHotkey;
        public static ModKeybind depositAllHotkey;
        public static ModKeybind lootAllHotkey;
        public static bool inventoryOffsetAdjusted = false;

        public static MethodInfo PickupItem;
        public static MethodInfo PullItem_Pickup;
        public static string checkSpawnRate { get; private set; }
        public static QoLPrime Instance { get; private set; }
        public static Hook pickupItemHook;
        public static Dictionary<string, Item[]> backpackPublic;
        public static string[] customDeathMessages;

        public static Dictionary<string, string> settings = new Dictionary<string, string>() { { "enableBackpack", "true" }, { "autoQuickStackToBackpack", "true" }, { "replaceDeathMessagesWithCustom", "false" }, { "spawnrate", "1.5" } };
        string configPath = ModLoader.ModPath + "/QoLPrimeConf/config.ini";
        string deathMessagePath = ModLoader.ModPath + "/QoLPrimeConf/deathMessages.txt";
        public static string dataDir = ModLoader.ModPath + "/QoLPrimeConf/bp/";
        string dataLoc = ModLoader.ModPath + "/QoLPrimeConf/bp/";
        public QoLPrime()
        {
            if (TextureAssets.InventoryBack != null)
            {
                value = TextureAssets.InventoryBack.Value;
                invBottom = (int)((value.Bounds.Height * 6.5f) * Main.inventoryScale);
            }
            
            if (!Directory.Exists(Path.GetDirectoryName(deathMessagePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(deathMessagePath));
            }
            if (File.Exists(configPath) && new FileInfo(configPath).Length / 64 >= 1)
            {
                settings = GetSettingsFromIni(configPath);
            }
            else
            {
                File.WriteAllText(configPath, InitializeDefaultSettings(settings));
            }

            File.Copy(@"C:\Users\dsteinberg\Documents\My Games\Terraria\ModLoader\Beta\Mod Sources\QoLPrime\deathMessages.txt", deathMessagePath, true);
            string deathMessagesConfig = File.ReadAllText(deathMessagePath);
            customDeathMessages = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(deathMessagesConfig);
            QoLPrime.Instance = this;
            
            /*if (backpackPublic == null)
            {
                    backpackPublic = new Dictionary<string, Item[]>();
                    List<Item> itemsToLoad = new List<Item>();
                    foreach (string file in Directory.GetFiles(dataDir))
                    {
                        TagCompound temp = TagIO.FromFile(file,false);
                        itemsToLoad.Add(TagIO.Deserialize<Item>(temp));
                    }

                    backpackPublic[LastPlayer] = itemsToLoad.ToArray();
            }*/
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
        public override void Close()
        {






            //TagIO.ToFile(list,dataLoc);

            base.Close();
        }
        public override void Load()
        {
            if (backpackPublic == null)
            {
                backpackPublic = new Dictionary<string, Item[]>();
                List<Item> itemsToLoad = new List<Item>();
                string nameOfSourceChar = "";
                foreach (string file in Directory.GetFiles(dataDir))
                {
                    var ob = TagIO.FromFile(file, false);
                    var whoa = TagIO.Deserialize<Item>(ob);
                    if (whoa != null)
                    {
                        itemsToLoad.Add(whoa);
                    }
                    string s = file.Substring(file.LastIndexOf("/") + 1);
                    s = s.Substring(0, s.LastIndexOf("_"));
                    nameOfSourceChar = s;
                    
                }
                backpackPublic[nameOfSourceChar] = itemsToLoad.ToArray();
            }
            quickStackHotkey = KeybindLoader.RegisterKeybind(this,"Quick Stack/Quick Stack all", "OemSemicolon");
            backpackToggle = KeybindLoader.RegisterKeybind(this,"Toggle Backpack", "OemTilde");
            printSpawnRate = KeybindLoader.RegisterKeybind(this,"Print Spawn Rate", "OemBackslash");
            depositAllHotkey = KeybindLoader.RegisterKeybind(this,"Deposit All", "OemNone");
            lootAllHotkey = KeybindLoader.RegisterKeybind(this,"Loot All", "OemNone");
            if (backpackPublic == null)
            {
                ///backpackPublic = new Dictionary<string, Item[]>();
            }
            MonoModHooks.RequestNativeAccess();

            //Hook chestRangeHook = new Hook(typeof(Player).GetMethod("HandleBeingInChestRange", BindingFlags.NonPublic | BindingFlags.Instance), typeof(PlayerModification).GetMethod("chestRangeHijack"));
            //chestRangeHook.Apply();
            Hook drawInvHook = new Hook(typeof(Main).GetMethod("DrawInventory", BindingFlags.NonPublic | BindingFlags.Instance), typeof(Detours).GetMethod("drawInventoryHijack"));
            drawInvHook.Apply();
            Hook drawNpcHook = new Hook(typeof(Main).GetMethod("DrawNPCs", BindingFlags.NonPublic | BindingFlags.Instance), typeof(Detours).GetMethod("drawNpcsHijack"));
            drawNpcHook.Apply();
            MethodInfo trashMethodInfo = typeof(Main).GetMethod("DrawTrashItemSlot", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
            if (trashMethodInfo != null)
            {

                Hook drawTrashHook = new Hook(trashMethodInfo, typeof(Detours).GetMethod("drawTrashHijack"));
                drawTrashHook.Apply();
                On.Terraria.Main.DrawTrashItemSlot += Detours.drawTrashHijack;
            }
            MethodInfo bestMethodInfo = typeof(Main).GetMethod("DrawBestiaryIcon", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
            if (bestMethodInfo != null)
            {

                Hook drawBestHook = new Hook(bestMethodInfo, typeof(Detours).GetMethod("drawBestHijack"));
                drawBestHook.Apply();
                On.Terraria.Main.DrawBestiaryIcon += Detours.drawBestHijack;
            }
            MethodInfo emoteMethodInfo = typeof(Main).GetMethod("DrawEmoteBubblesButton", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
            if (emoteMethodInfo != null)
            {

                Hook drawEmoteHook = new Hook(emoteMethodInfo, typeof(Detours).GetMethod("drawEmoteHijack"));
                drawEmoteHook.Apply();
                On.Terraria.Main.DrawEmoteBubblesButton += Detours.drawEmoteHijack;
            }
            MethodInfo grabItemsInfo = typeof(Player).GetMethod("GrabItems", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
            if (grabItemsInfo != null)
            {

                Hook grabHook = new Hook(grabItemsInfo, typeof(Detours).GetMethod("grabItemsHijack"));
                grabHook.Apply();
                On.Terraria.Player.GrabItems += Detours.grabItemsHijack;
            }
            PullItem_Pickup = typeof(Player).GetMethod("PullItem_Pickup", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
            if (PullItem_Pickup != null)
            {
                Hook pullItemPickupHook = new Hook(PullItem_Pickup, typeof(Detours).GetMethod("pullItemPickupHijack"));
                pullItemPickupHook.Apply();
                On.Terraria.Player.PullItem_Pickup += Detours.pullItemPickupHijack;
            }
            PickupItem = typeof(Player).GetMethod("PickupItem", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
            if (PickupItem != null)
            {
                pickupItemHook = new Hook(PickupItem, typeof(Detours).GetMethod("PickupItemHijack"));
                pickupItemHook.Apply();
                On.Terraria.Player.PickupItem += Detours.PickupItemHijack;
            }
            //Hook ChestUIDrawHook = new Hook(typeof(ChestUI).GetMethod("Draw"), typeof(QoLPrime).GetMethod("ChestUIDrawHijack"));
            //On.Terraria.Player.HandleBeingInChestRange += PlayerModification.chestRangeHijack;
            On.Terraria.UI.ChestUI.QuickStack += PlayerModification.QuickStackHijack;
            On.Terraria.Player.QuickStackAllChests += PlayerModification.quickStackAllHijack;
            On.Terraria.Main.DrawInventory += Detours.drawInventoryHijack;
            On.Terraria.Main.DrawNPCs += Detours.drawNpcsHijack;
            On.Terraria.UI.ChestUI.Draw += Detours.ChestUIDrawHijack;
            On.Terraria.UI.ChestUI.DepositAll += Detours.DepositAllHijack;
            On.Terraria.UI.ChestUI.LootAll += Detours.LootAllHijack;
            On.Terraria.DataStructures.PlayerDeathReason.GetDeathText += Detours.DeathReasonHijack;
            On.Terraria.UI.ItemSlot.OverrideHover_ItemArray_int_int += Detours.OverrideHoverHijack;
            On.Terraria.UI.ItemSlot.OverrideHover_refItem_int += Detours.OverrideHoverHijack;
            On.Terraria.UI.ItemSlot.OverrideLeftClick += Detours.OverrideLeftClick;

        }

        public override void Unload()
        {
            List<TagCompound> tempObj = new List<TagCompound>();
            if (backpackPublic.ContainsKey(LastPlayer)) {
                foreach (Item currentItem in backpackPublic[LastPlayer])
                {
                    if (currentItem.type != 0)
                        tempObj.Add(ItemIO.Save(currentItem));
                }
            }
            

            if (tempObj.Count > 0) {
                int i = 0;
                foreach (TagCompound tag in tempObj)
                {
                    TagIO.ToFile(tag, dataLoc+LastPlayer+"_"+i, false);
                    i++;
                }
                



                QoLPrime.Instance = null;

            }
            base.Unload();
        }
        public static byte[] ObjectToByteArray(Object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
        public static Item[] newEmptyChest()
        {
            Item[] toReturn = new Item[40];
            for (int i = 0; i < toReturn.Length; i++)
            {
                toReturn[0] = new Item();
            }
            return toReturn;
        }
        static string InitializeDefaultSettings(Dictionary<string, string> settings)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(settings);
        }
        public static Dictionary<string, string> GetSettingsFromIni(string path)
        {
            Dictionary<string, string> toReturn = new Dictionary<string, string>();
            toReturn = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(path));
            return toReturn;
        }

    }
    internal enum ExampleModMessageType : byte
    {
        ExamplePlayerSyncPlayer,
        ExampleTeleportToStatue
    }


    class SpawnRateMultiplierGlobalNPC : GlobalNPC
    {
        static float multiplier = 1.5f;
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            spawnRate = (int)(spawnRate / multiplier);

            maxSpawns = (int)(maxSpawns * multiplier);
            QoLPrime.setSpawnRateLabel($"Spawnrate: {spawnRate} Max spawns: {maxSpawns}");
        }
    }
}