using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

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

        public static ModKeybind printSpawnRate;
        public static ModKeybind quickStackHotkey;
        public static ModKeybind depositAllHotkey;
        public static ModKeybind lootAllHotkey;



        public static string checkSpawnRate { get; private set; }
        public static QoLPrime Instance { get; private set; }


        public static string[] customDeathMessages;

        public static Dictionary<string, string> settings = new Dictionary<string, string>() { { "replaceDeathMessagesWithCustom", "false" }, { "spawnrate", "1.5" } };
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

            quickStackHotkey = KeybindLoader.RegisterKeybind(this, "Quick Stack/Quick Stack all", "OemSemicolon");

            printSpawnRate = KeybindLoader.RegisterKeybind(this, "Print Spawn Rate", "OemBackslash");
            depositAllHotkey = KeybindLoader.RegisterKeybind(this, "Deposit All", "OemNone");
            lootAllHotkey = KeybindLoader.RegisterKeybind(this, "Loot All", "OemNone");

            MonoModHooks.RequestNativeAccess();
            Hook drawNpcHook = new Hook(typeof(Main).GetMethod("DrawNPCs", BindingFlags.NonPublic | BindingFlags.Instance), typeof(Detours).GetMethod("drawNpcsHijack"));
            drawNpcHook.Apply();
            //Hook chestRangeHook = new Hook(typeof(Player).GetMethod("HandleBeingInChestRange", BindingFlags.NonPublic | BindingFlags.Instance), typeof(PlayerModification).GetMethod("chestRangeHijack"));
            //chestRangeHook.Apply();

            //Hook ChestUIDrawHook = new Hook(typeof(ChestUI).GetMethod("Draw"), typeof(QoLPrime).GetMethod("ChestUIDrawHijack"));
            //On.Terraria.Player.HandleBeingInChestRange += PlayerModification.chestRangeHijack;


            //On.Terraria.DataStructures.PlayerDeathReason.GetDeathText += Detours.DeathReasonHijack;

        }

        public override void Unload()
        {


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