using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour;
using QoLPrime.Content.Players;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace QoLPrime
{
    public class QoLPrime : Mod
    {
        public const string AssetPath = "QoLPrime/Assets/";
        public static int currentBiome;
        static Texture2D value = TextureAssets.InventoryBack.Value;
        public static int invBottom = (int)((value.Bounds.Height * 6.5f) * Main.inventoryScale);
        public static int invBottomOffset = 165;
        public static ModHotKey backpackToggle;
        public static ModHotKey printSpawnRate;
        public static ModHotKey quickStackHotkey;
        public static ModHotKey depositAllHotkey;
        public static ModHotKey lootAllHotkey;
        public static bool inventoryOffsetAdjusted = false;

        public static MethodInfo PickupItem;
        public static MethodInfo PullItem_Pickup;
        public static string checkSpawnRate { get; private set; }
        public static QoLPrime Instance { get; private set; }
        public static Hook pickupItemHook;
        public static Dictionary<string, Item[]> backpackPublic;
        public static string[] customDeathMessages;

        public static Dictionary<string, string> settings = new Dictionary<string, string>() { { "enableBackpack", "true" }, { "autoQuickStackToBackpack", "true" }, { "replaceDeathMessagesWithCustom", "false" }, { "spawnrate", "1.5" } };

        public QoLPrime()
        {
            string configPath = ModLoader.ModPath + "/QoLPrimeConf/config.ini";
            string deathMessagePath = ModLoader.ModPath + "/QoLPrimeConf/deathMessages.txt";
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
            if (backpackPublic == null)
            {
                backpackPublic = new Dictionary<string, Item[]>();
            }
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
            depositAllHotkey = RegisterHotKey("Deposit All", "");
            lootAllHotkey = RegisterHotKey("Loot All", "");
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
            QoLPrime.Instance = null;
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