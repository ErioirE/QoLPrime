﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using QoLPrime.Content.Players;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.UI.Gamepad;

namespace QoLPrime.Content.UI
{
    class DrawCustomChestUI
    {
        public static int customInvBottom = QoLPrime.invBottom;
        public class ButtonID
        {
            public const int LootAll = 0;
            public const int DepositAll = 1;
            public const int QuickStack = 2;
            public const int Restock = 3;
            public const int Sort = 4;
            public const int RenameChest = 5;
            public const int RenameChestCancel = 6;
            public const int ToggleVacuum = 7;
            public const int Count = 7;
        }

        public Item[] offsetVersion = new Item[50];

        public const float buttonScaleMinimum = 0.75f;
        public const float buttonScaleMaximum = 1f;
        public static float[] ButtonScale = new float[7];
        public static bool[] ButtonHovered = new bool[7];

        public static void UpdateHover(int ID, bool hovering)
        {
            if (hovering)
            {
                if (!ButtonHovered[ID])
                    SoundEngine.PlaySound(12);

                ButtonHovered[ID] = true;
                ButtonScale[ID] += 0.05f;
                if (ButtonScale[ID] > 1f)
                    ButtonScale[ID] = 1f;
            }
            else
            {
                ButtonHovered[ID] = false;
                ButtonScale[ID] -= 0.05f;
                if (ButtonScale[ID] < 0.75f)
                    ButtonScale[ID] = 0.75f;
            }
        }

        public static void Draw(SpriteBatch spritebatch)
        {
            //customInvBottom = QoLPrime.invBottom;
            if (PlayerModification.backpackEnabled && !Main.recBigList)
            {
                Main.inventoryScale = 0.755f;
                if (Utils.FloatIntersect(Main.mouseX, Main.mouseY, 0f, 0f, 73f, customInvBottom, 560f * Main.inventoryScale, 224f * Main.inventoryScale))
                    Main.player[Main.myPlayer].mouseInterface = true;

                DrawName(spritebatch);
                if (Main.LocalPlayer.chest == -1)
                {
                    DrawButtons(spritebatch);
                }
                DrawSlots(spritebatch);
            }
            else
            {
                for (int i = 0; i < 7; i++)
                {
                    ButtonScale[i] = 0.75f;
                    ButtonHovered[i] = false;
                }
            }
        }

        private static void DrawName(SpriteBatch spritebatch)
        {
            Player player = Main.player[Main.myPlayer];
            string text = "Void Backpack";





            Color color = new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor);
            color = Color.White * (1f - (255f - (float)(int)Main.mouseTextColor) / 255f * 0.5f);
            color.A = byte.MaxValue;
            Utils.WordwrapString(text, FontAssets.MouseText.Value, 200, 1, out int lineAmount);
            lineAmount++;
            for (int i = 0; i < lineAmount; i++)
            {
                ChatManager.DrawColorCodedStringWithShadow(spritebatch, FontAssets.MouseText.Value, text, new Vector2(504f, customInvBottom + i * 26 + 20), color, 0f, Vector2.Zero, Vector2.One, -1f, 1.5f);
            }
        }

        private static void DrawButtons(SpriteBatch spritebatch)
        {
            for (int i = 0; i < 5; i++)
            {
                DrawButton(spritebatch, i, 506, customInvBottom + 50);
            }
        }

        private static void DrawButton(SpriteBatch spriteBatch, int ID, int X, int Y)
        {
            Player player = Main.player[Main.myPlayer];
            if ((ID == 5 && player.chest < -1) || (ID == 6 && !Main.editChest))
            {
                UpdateHover(ID, hovering: false);
                return;
            }

            if (ID == 7 && player.chest != -5)
            {
                UpdateHover(ID, hovering: false);
                return;
            }

            int num = ID;
            if (ID == 7)
                num = 5;

            Y += num * 25;
            float num2 = ButtonScale[ID];
            string text = "";
            switch (ID)
            {
                case 0:
                    text = Lang.inter[29].Value;
                    break;
                case 1:
                    text = Lang.inter[30].Value;
                    break;
                case 2:
                    text = Lang.inter[31].Value;
                    break;
                case 3:
                    text = Lang.inter[82].Value;
                    break;
                case 5:
                    text = Lang.inter[63].Value;
                    break;
                case 4:
                    text = Lang.inter[122].Value;
                    break;
            }

            Vector2 value = FontAssets.MouseText.Value.MeasureString(text);
            Color color = new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor) * num2;
            color = Color.White * 0.97f * (1f - (255f - (float)(int)Main.mouseTextColor) / 255f * 0.5f);
            color.A = byte.MaxValue;
            X += (int)(value.X * num2 / 2f);
            bool flag = Utils.FloatIntersect(Main.mouseX, Main.mouseY, 0f, 0f, (float)X - value.X / 2f, Y - 12, value.X, 24f);
            if (ButtonHovered[ID])
                flag = Utils.FloatIntersect(Main.mouseX, Main.mouseY, 0f, 0f, (float)X - value.X / 2f - 10f, Y - 12, value.X + 16f, 24f);

            if (flag)
                color = Main.OurFavoriteColor;

            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text, new Vector2(X, Y), color, 0f, value / 2f, new Vector2(num2), -1f, 1.5f);
            value *= num2;
            switch (ID)
            {
                case 0:
                    UILinkPointNavigator.SetPosition(500, new Vector2((float)X - value.X * num2 / 2f * 0.8f, Y));
                    break;
                case 1:
                    UILinkPointNavigator.SetPosition(501, new Vector2((float)X - value.X * num2 / 2f * 0.8f, Y));
                    break;
                case 2:
                    UILinkPointNavigator.SetPosition(502, new Vector2((float)X - value.X * num2 / 2f * 0.8f, Y));
                    break;
                case 5:
                    UILinkPointNavigator.SetPosition(504, new Vector2(X, Y));
                    break;
                case 6:
                    UILinkPointNavigator.SetPosition(504, new Vector2(X, Y));
                    break;
                case 3:
                    UILinkPointNavigator.SetPosition(503, new Vector2((float)X - value.X * num2 / 2f * 0.8f, Y));
                    break;
                case 4:
                    UILinkPointNavigator.SetPosition(505, new Vector2((float)X - value.X * num2 / 2f * 0.8f, Y));
                    break;
                case 7:
                    UILinkPointNavigator.SetPosition(506, new Vector2((float)X - value.X * num2 / 2f * 0.8f, Y));
                    break;
            }

            if (!flag)
            {
                UpdateHover(ID, hovering: false);
                return;
            }

            UpdateHover(ID, hovering: true);
            if (PlayerInput.IgnoreMouseInterface)
                return;

            player.mouseInterface = true;
            if (Main.mouseLeft && Main.mouseLeftRelease)
            {
                switch (ID)
                {
                    case 0:
                        LootAll();
                        break;
                    case 1:
                        DepositAll();
                        break;
                    case 2:
                        QuickStack();
                        break;
                    case 5:
                        RenameChest();
                        break;
                    case 6:
                        RenameChestCancel();
                        break;
                    case 3:
                        Restock();
                        break;
                    case 4:
                        ItemSorting.SortChest();
                        break;
                    case 7:
                        ToggleVacuum();
                        break;
                }

                Recipe.FindRecipes();
            }
        }

        private static void ToggleVacuum()
        {
            Player obj = Main.player[Main.myPlayer];
            obj.IsVoidVaultEnabled = !obj.IsVoidVaultEnabled;
        }

        private static void DrawSlots(SpriteBatch spriteBatch)
        {
            int context = 0;
            Player player = Main.player[Main.myPlayer];
            if (player.chest == -1)
            {
                context = 4;
            }
            Item[] inv = null;
            int offset;


            inv = PlayerModification.backpack.item;

            Main.inventoryScale = 0.755f;
            if (Utils.FloatIntersect(Main.mouseX, Main.mouseY, 0f, 0f, 73f, customInvBottom, 560f * Main.inventoryScale, 224f * Main.inventoryScale) && !PlayerInput.IgnoreMouseInterface)
                player.mouseInterface = true;

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    int num = (int)(73f + (float)(i * 56) * Main.inventoryScale);
                    int num2 = (int)((float)customInvBottom + (float)(j * 56) * Main.inventoryScale);
                    int slot = i + j * 10;
                    new Color(100, 100, 100, 100);
                    if (Utils.FloatIntersect(Main.mouseX, Main.mouseY, 0f, 0f, num, num2, (float)TextureAssets.InventoryBack.Width() * Main.inventoryScale, (float)TextureAssets.InventoryBack.Height() * Main.inventoryScale) && !PlayerInput.IgnoreMouseInterface)
                    {

                        player.mouseInterface = true;
                        CustomItemSlot.Handle(inv, context, slot);
                    }
                    CustomItemSlot.Draw(spriteBatch, inv, context, slot, new Vector2(num, num2));
                }
            }
        }


        public static void LootAll()
        {
            GetItemSettings lootAllSettings = GetItemSettings.LootAllSettings;
            Player player = Main.player[Main.myPlayer];

            if (player.chest == -1)
            {
                for (int l = 0; l < 40; l++)
                {
                    if (PlayerModification.backpack.item[l].type > 0)
                    {
                        PlayerModification.backpack.item[l].position = player.Center;
                        PlayerModification.backpack.item[l] = player.GetItem(Main.myPlayer, PlayerModification.backpack.item[l], lootAllSettings);
                    }
                }

                return;
            }
            else
            {
                Chest chestToLootFrom = null;
                if (player.chest < -1)
                {
                    if (player.chest == -2)
                    {
                        chestToLootFrom = player.bank;

                    }

                    if (player.chest == -4)
                    {
                        chestToLootFrom = player.bank3;

                    }

                    if (player.chest == -5)
                    {
                        chestToLootFrom = player.bank4;

                    }

                    if (player.chest == -3)
                    {
                        chestToLootFrom = player.bank2;

                    }
                }
                else if (player.chest > -1)
                {
                    chestToLootFrom = Main.chest[player.chest];
                }
                for (int m = 0; m < 40; m++)
                {

                    for (int k = 0; k < 40; k++)
                    {
                        if (chestToLootFrom.item[m].type != 0)
                        {
                            if (PlayerModification.backpack.item[k].type == 0)
                            {
                                SoundEngine.PlaySound(7);
                                PopupText.NewText(PopupTextContext.ItemPickupToVoidContainer, chestToLootFrom.item[m], chestToLootFrom.item[m].stack);
                                PlayerModification.backpack.item[k] = chestToLootFrom.item[m].Clone();
                                chestToLootFrom.item[m].SetDefaults();
                                if (player.chest > -1)
                                {
                                    Main.chest[player.chest] = chestToLootFrom;
                                }
                                break;
                            }
                            else if (TryPlacingInChest(chestToLootFrom.item[m], PlayerModification.backpack))
                            {
                                SoundEngine.PlaySound(7);
                                if (player.chest > -1)
                                {
                                    Main.chest[player.chest] = chestToLootFrom;
                                }
                                break;
                            }
                        }

                    }


                }

            }



        }

        public static void DepositAll()
        {
            Player player = Main.player[Main.myPlayer];



            for (int num = 39; num >= 0; num--)
            {
                if (PlayerModification.backpack.item[num].stack > 0 && PlayerModification.backpack.item[num].type > 0 && !PlayerModification.backpack.item[num].favorited)
                {
                    if (player.chest != -1)
                    {
                        if (player.chest == -2)
                        {
                            Chest chest = player.bank;
                            for (int j = 0; j < 40; j++)
                            {

                                if (TryPlacingInChest(PlayerModification.backpack.item[num], chest))
                                {
                                    SoundEngine.PlaySound(7);

                                }

                            }

                        }

                        if (player.chest == -4)
                        {
                            Chest chest2 = player.bank3;
                            for (int k = 0; k < 40; k++)
                            {
                                if (TryPlacingInChest(PlayerModification.backpack.item[num], chest2))
                                {
                                    SoundEngine.PlaySound(7);

                                }

                            }

                        }

                        if (player.chest == -5)
                        {
                            Chest chest3 = player.bank4;
                            for (int l = 0; l < 40; l++)
                            {
                                if (TryPlacingInChest(PlayerModification.backpack.item[num], chest3))
                                {
                                    SoundEngine.PlaySound(7);

                                }

                            }

                        }

                        if (player.chest == -3)
                        {
                            Chest chest4 = player.bank2;
                            for (int m = 0; m < 40; m++)
                            {
                                if (TryPlacingInChest(PlayerModification.backpack.item[num], chest4))
                                {
                                    SoundEngine.PlaySound(7);
                                }
                            }


                        }
                        if (PlayerModification.backpack.item[num].maxStack > 1)
                        {
                            for (int i = 0; i < 40; i++)
                            {




                                if (Main.chest[player.chest].item[i].stack < Main.chest[player.chest].item[i].maxStack && PlayerModification.backpack.item[num].IsTheSameAs(Main.chest[player.chest].item[i]))
                                {
                                    int num5 = PlayerModification.backpack.item[num].stack;
                                    if (PlayerModification.backpack.item[num].stack + Main.chest[player.chest].item[i].stack > Main.chest[player.chest].item[i].maxStack)
                                        num5 = Main.chest[player.chest].item[i].maxStack - Main.chest[player.chest].item[i].stack;

                                    PlayerModification.backpack.item[num].stack -= num5;
                                    PlayerModification.backpack.item[i].stack += num5;
                                    SoundEngine.PlaySound(7);
                                    if (PlayerModification.backpack.item[num].stack <= 0)
                                    {
                                        PlayerModification.backpack.item[num].SetDefaults();
                                        break;
                                    }

                                    if (Main.chest[player.chest].item[i].type == 0)
                                    {
                                        Main.chest[player.chest].item[i] = PlayerModification.backpack.item[num].Clone();
                                        PlayerModification.backpack.item[num].SetDefaults();
                                    }
                                }
                                else if (Main.chest[player.chest].item[i].type == ItemID.None)
                                {
                                    Main.chest[player.chest].item[i] = PlayerModification.backpack.item[num].Clone();
                                    PlayerModification.backpack.item[num].SetDefaults();
                                }


                            }
                        }
                        else if (PlayerModification.backpack.item[num].stack > 0)
                        {

                            for (int m = 0; m < 40; m++)
                            {
                                if (Main.chest[player.chest].item[m].stack == 0)
                                {
                                    SoundEngine.PlaySound(7);
                                    Main.chest[player.chest].item[m] = PlayerModification.backpack.item[num].Clone();
                                    PlayerModification.backpack.item[num].SetDefaults();
                                    break;
                                }
                            }


                        }
                    }

                }

            }
            for (int num = 49; num >= 10; num--)
            {
                if (player.inventory[num].stack > 0 && player.chest == -1)
                {

                    if (PlayerModification.backpackEnabled)
                    {
                        for (int m = 0; m < 40; m++)
                        {
                            if (PlayerModification.backpack.item[m].stack == 0 && !player.inventory[num].favorited)
                            {
                                SoundEngine.PlaySound(7);
                                TryPlacingInChest(player.inventory[num], PlayerModification.backpack);
                                //PlayerModification.backpack.item[m] = player.inventory[num].Clone();
                                //player.inventory[num].SetDefaults();
                                break;
                            }
                        }
                    }

                }
            }
        }

        public static void QuickStack()
        {
            Player player = Main.player[Main.myPlayer];
            if (player.chest == -5)
                MoveCoins(player.inventory, PlayerModification.backpack.item);
            else if (player.chest == -4)
                MoveCoins(player.inventory, player.bank3.item);
            else if (player.chest == -3)
                MoveCoins(player.inventory, player.bank2.item);
            else if (player.chest == -2)
                MoveCoins(player.inventory, player.bank.item);

            Item[] inventory = player.inventory;
            Item[] item = PlayerModification.backpack.item;

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
                    list3.Add(i);
            }

            int num = 50;
            if (player.chest <= -2)
                num += 4;

            for (int j = 10; j < num; j++)
            {
                if (list.Contains(inventory[j].netID) && !inventory[j].favorited)
                    dictionary.Add(j, inventory[j].netID);
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
                            break;

                        if (num3 > num4)
                            num3 = num4;

                        SoundEngine.PlaySound(7);
                        item[num2].stack += num3;
                        inventory[item2.Key].stack -= num3;
                        if (inventory[item2.Key].stack == 0)
                            inventory[item2.Key].SetDefaults();

                        array[num2] = true;
                    }
                }
            }

            foreach (KeyValuePair<int, int> item3 in dictionary)
            {
                if (inventory[item3.Key].stack == 0)
                    list4.Add(item3.Key);
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
                    continue;

                foreach (KeyValuePair<int, int> item5 in dictionary)
                {
                    if ((item5.Value != num6 || inventory[item5.Key].netID != num6) && (!flag || inventory[item5.Key].stack <= 0))
                        continue;

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
                            break;

                        if (num7 > num8)
                            num7 = num8;

                        item[num5].stack += num7;
                        inventory[item5.Key].stack -= num7;
                        if (inventory[item5.Key].stack == 0)
                            inventory[item5.Key] = new Item();
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

        public static void RenameChest()
        {
            Player player = Main.player[Main.myPlayer];
            if (!Main.editChest)
                IngameFancyUI.OpenVirtualKeyboard(2);
            else
                RenameChestSubmit(player);
        }

        public static void RenameChestSubmit(Player player)
        {
            SoundEngine.PlaySound(12);
            Main.editChest = false;
            int chest = player.chest;
            if (chest < 0)
                return;

            if (Main.npcChatText == Main.defaultChestName)
                Main.npcChatText = "";

            if (Main.chest[chest].name != Main.npcChatText)
            {
                Main.chest[chest].name = Main.npcChatText;
                if (Main.netMode == 1)
                    player.editedChestName = true;
            }
        }

        public static void RenameChestCancel()
        {
            SoundEngine.PlaySound(12);
            Main.editChest = false;
            Main.npcChatText = string.Empty;
            Main.blockKey = Keys.Escape.ToString();
        }

        public static void Restock()
        {
            Player player = Main.player[Main.myPlayer];
            Item[] inventory = player.inventory;
            Item[] item = PlayerModification.backpack.item;

            HashSet<int> hashSet = new HashSet<int>();
            List<int> list = new List<int>();
            List<int> list2 = new List<int>();
            for (int num = 57; num >= 0; num--)
            {
                if ((num < 50 || num >= 54) && (inventory[num].type < 71 || inventory[num].type > 74))
                {
                    if (inventory[num].stack > 0 && inventory[num].maxStack > 1 && inventory[num].prefix == 0)
                    {
                        hashSet.Add(inventory[num].netID);
                        if (inventory[num].stack < inventory[num].maxStack)
                            list.Add(num);
                    }
                    else if (inventory[num].stack == 0 || inventory[num].netID == 0 || inventory[num].type == 0)
                    {
                        list2.Add(num);
                    }
                }
            }

            bool flag = false;
            for (int i = 0; i < item.Length; i++)
            {
                if (item[i].stack < 1 || item[i].prefix != 0 || !hashSet.Contains(item[i].netID))
                    continue;

                bool flag2 = false;
                for (int j = 0; j < list.Count; j++)
                {
                    int num2 = list[j];
                    int context = 0;
                    if (num2 >= 50)
                        context = 2;

                    if (inventory[num2].netID != item[i].netID || CustomItemSlot.PickItemMovementAction(inventory, context, num2, item[i]) == -1)
                        continue;

                    int num3 = item[i].stack;
                    if (inventory[num2].maxStack - inventory[num2].stack < num3)
                        num3 = inventory[num2].maxStack - inventory[num2].stack;

                    inventory[num2].stack += num3;
                    item[i].stack -= num3;
                    flag = true;
                    if (inventory[num2].stack == inventory[num2].maxStack)
                    {
                        if (Main.netMode == 1 && Main.player[Main.myPlayer].chest > -1)
                            NetMessage.SendData(32, -1, -1, null, Main.player[Main.myPlayer].chest, i);

                        list.RemoveAt(j);
                        j--;
                    }

                    if (item[i].stack == 0)
                    {
                        item[i] = new Item();
                        flag2 = true;
                        if (Main.netMode == 1 && Main.player[Main.myPlayer].chest > -1)
                            NetMessage.SendData(32, -1, -1, null, Main.player[Main.myPlayer].chest, i);

                        break;
                    }
                }

                if (flag2 || list2.Count <= 0 || item[i].ammo == 0)
                    continue;

                for (int k = 0; k < list2.Count; k++)
                {
                    int context2 = 0;
                    if (list2[k] >= 50)
                        context2 = 2;

                    if (CustomItemSlot.PickItemMovementAction(inventory, context2, list2[k], item[i]) != -1)
                    {
                        Utils.Swap(ref inventory[list2[k]], ref item[i]);
                        if (Main.netMode == 1 && Main.player[Main.myPlayer].chest > -1)
                            NetMessage.SendData(32, -1, -1, null, Main.player[Main.myPlayer].chest, i);

                        list.Add(list2[k]);
                        list2.RemoveAt(k);
                        flag = true;
                        break;
                    }
                }
            }

            if (flag)
                SoundEngine.PlaySound(7);
        }

        public static void MoveCoins(Item[] pInv, Item[] cInv)
        {
            bool flag = false;
            int[] array = new int[4];
            List<int> list = new List<int>();
            List<int> list2 = new List<int>();
            bool flag2 = false;
            int[] array2 = new int[40];
            for (int i = 0; i < cInv.Length; i++)
            {
                array2[i] = -1;
                if (cInv[i].stack < 1 || cInv[i].type < 1)
                {
                    list2.Add(i);
                    cInv[i] = new Item();
                }

                if (cInv[i] != null && cInv[i].stack > 0)
                {
                    int num = 0;
                    if (cInv[i].type == 71)
                        num = 1;

                    if (cInv[i].type == 72)
                        num = 2;

                    if (cInv[i].type == 73)
                        num = 3;

                    if (cInv[i].type == 74)
                        num = 4;

                    array2[i] = num - 1;
                    if (num > 0)
                    {
                        array[num - 1] += cInv[i].stack;
                        list2.Add(i);
                        cInv[i] = new Item();
                        flag2 = true;
                    }
                }
            }

            if (!flag2)
                return;

            for (int j = 0; j < pInv.Length; j++)
            {
                if (j != 58 && pInv[j] != null && pInv[j].stack > 0 && !pInv[j].favorited)
                {
                    int num2 = 0;
                    if (pInv[j].type == 71)
                        num2 = 1;

                    if (pInv[j].type == 72)
                        num2 = 2;

                    if (pInv[j].type == 73)
                        num2 = 3;

                    if (pInv[j].type == 74)
                        num2 = 4;

                    if (num2 > 0)
                    {
                        flag = true;
                        array[num2 - 1] += pInv[j].stack;
                        list.Add(j);
                        pInv[j] = new Item();
                    }
                }
            }

            for (int k = 0; k < 3; k++)
            {
                while (array[k] >= 100)
                {
                    array[k] -= 100;
                    array[k + 1]++;
                }
            }

            for (int l = 0; l < 40; l++)
            {
                if (array2[l] < 0 || cInv[l].type != 0)
                    continue;

                int num3 = l;
                int num4 = array2[l];
                if (array[num4] > 0)
                {
                    cInv[num3].SetDefaults(71 + num4);
                    cInv[num3].stack = array[num4];
                    if (cInv[num3].stack > cInv[num3].maxStack)
                        cInv[num3].stack = cInv[num3].maxStack;

                    array[num4] -= cInv[num3].stack;
                    array2[l] = -1;
                }

                if (Main.netMode == 1 && Main.player[Main.myPlayer].chest > -1)
                    NetMessage.SendData(32, -1, -1, null, Main.player[Main.myPlayer].chest, num3);

                list2.Remove(num3);
            }

            for (int m = 0; m < 40; m++)
            {
                if (array2[m] < 0 || cInv[m].type != 0)
                    continue;

                int num5 = m;
                int num6 = 3;
                while (num6 >= 0)
                {
                    if (array[num6] > 0)
                    {
                        cInv[num5].SetDefaults(71 + num6);
                        cInv[num5].stack = array[num6];
                        if (cInv[num5].stack > cInv[num5].maxStack)
                            cInv[num5].stack = cInv[num5].maxStack;

                        array[num6] -= cInv[num5].stack;
                        array2[m] = -1;
                        break;
                    }

                    if (array[num6] == 0)
                        num6--;
                }

                if (Main.netMode == 1 && Main.player[Main.myPlayer].chest > -1)
                    NetMessage.SendData(32, -1, -1, null, Main.player[Main.myPlayer].chest, num5);

                list2.Remove(num5);
            }

            while (list2.Count > 0)
            {
                int num7 = list2[0];
                int num8 = 3;
                while (num8 >= 0)
                {
                    if (array[num8] > 0)
                    {
                        cInv[num7].SetDefaults(71 + num8);
                        cInv[num7].stack = array[num8];
                        if (cInv[num7].stack > cInv[num7].maxStack)
                            cInv[num7].stack = cInv[num7].maxStack;

                        array[num8] -= cInv[num7].stack;
                        break;
                    }

                    if (array[num8] == 0)
                        num8--;
                }

                if (Main.netMode == 1 && Main.player[Main.myPlayer].chest > -1)
                    NetMessage.SendData(32, -1, -1, null, Main.player[Main.myPlayer].chest, list2[0]);

                list2.RemoveAt(0);
            }

            int num9 = 3;
            while (num9 >= 0 && list.Count > 0)
            {
                int num10 = list[0];
                if (array[num9] > 0)
                {
                    pInv[num10].SetDefaults(71 + num9);
                    pInv[num10].stack = array[num9];
                    if (pInv[num10].stack > pInv[num10].maxStack)
                        pInv[num10].stack = pInv[num10].maxStack;

                    array[num9] -= pInv[num10].stack;
                    flag = false;
                    list.RemoveAt(0);
                }

                if (array[num9] == 0)
                    num9--;
            }

            if (flag)
                SoundEngine.PlaySound(7);
        }

        public static bool TryPlacingInChest(Item I, Chest chest, bool justCheck = false)
        {
            GetContainerUsageInfo(chest, out bool sync, out Item[] chestinv);
            if (IsBlockedFromTransferIntoChest(I, chestinv))
                return false;

            Player player = Main.player[Main.myPlayer];
            bool flag = false;
            if (I.maxStack > 1)
            {
                for (int i = 0; i < 40; i++)
                {
                    if (chestinv[i].stack >= chestinv[i].maxStack || !I.IsTheSameAs(chestinv[i]))
                        continue;

                    int num = I.stack;
                    if (I.stack + chestinv[i].stack > chestinv[i].maxStack)
                        num = chestinv[i].maxStack - chestinv[i].stack;

                    if (justCheck)
                    {
                        flag = (flag || num > 0);
                        break;
                    }

                    I.stack -= num;
                    chestinv[i].stack += num;
                    SoundEngine.PlaySound(7);
                    if (I.stack <= 0)
                    {
                        I.SetDefaults();
                        if (sync)
                            NetMessage.SendData(32, -1, -1, null, player.chest, i);

                        break;
                    }

                    if (chestinv[i].type == 0)
                    {
                        chestinv[i] = I.Clone();
                        if (chest == PlayerModification.backpack)
                        {
                            PopupText.NewText(PopupTextContext.ItemPickupToVoidContainer, I, I.stack);
                        }
                        I.SetDefaults();
                    }

                    if (sync)
                        NetMessage.SendData(32, -1, -1, null, player.chest, i);
                }
            }

            if (I.stack > 0)
            {
                for (int j = 0; j < 40; j++)
                {
                    if (chestinv[j].stack != 0)
                        continue;

                    if (justCheck)
                    {
                        flag = true;
                        break;
                    }

                    SoundEngine.PlaySound(7);
                    chestinv[j] = I.Clone();
                    if (chest == PlayerModification.backpack)
                    {
                        PopupText.NewText(PopupTextContext.ItemPickupToVoidContainer, I, I.stack);
                    }
                    I.SetDefaults();
                    if (sync)
                        NetMessage.SendData(32, -1, -1, null, player.chest, j);

                    break;
                }
            }

            return flag;
        }

        public static void GetContainerUsageInfo(Chest chestToCheck, out bool sync, out Item[] chestinv)
        {
            sync = false;
            Player player = Main.player[Main.myPlayer];
            chestinv = chestToCheck.item;

        }

        public static bool IsBlockedFromTransferIntoChest(Item item, Item[] container)
        {
            if (item.type == 3213 && item.favorited && container == Main.LocalPlayer.bank.item)
                return true;

            if (item.type == 4131 && item.favorited && container == Main.LocalPlayer.bank4.item)
                return true;

            return false;
        }

        public static bool TryPlacingInPlayer(int slot, bool justCheck)
        {
            bool flag = false;
            Player player = Main.player[Main.myPlayer];
            Item[] inventory = player.inventory;
            Item[] item = PlayerModification.backpack.item;


            Item item2 = item[slot];
            bool flag2 = false;
            if (item2.maxStack > 1)
            {
                for (int num = 49; num >= 0; num--)
                {
                    if (inventory[num].stack < inventory[num].maxStack && item2.IsTheSameAs(inventory[num]))
                    {
                        int num2 = item2.stack;
                        if (item2.stack + inventory[num].stack > inventory[num].maxStack)
                            num2 = inventory[num].maxStack - inventory[num].stack;

                        if (justCheck)
                        {
                            flag2 = (flag2 || num2 > 0);
                            break;
                        }

                        item2.stack -= num2;
                        inventory[num].stack += num2;
                        SoundEngine.PlaySound(7);
                        if (item2.stack <= 0)
                        {
                            item2.SetDefaults();
                            if (flag)
                                NetMessage.SendData(32, -1, -1, null, player.chest, num);

                            break;
                        }

                        if (inventory[num].type == 0)
                        {
                            inventory[num] = item2.Clone();
                            item2.SetDefaults();
                        }

                        if (flag)
                            NetMessage.SendData(32, -1, -1, null, player.chest, num);
                    }
                }
            }

            if (item2.stack > 0)
            {
                for (int num3 = 49; num3 >= 0; num3--)
                {
                    if (inventory[num3].stack == 0)
                    {
                        if (justCheck)
                        {
                            flag2 = true;
                            break;
                        }

                        SoundEngine.PlaySound(7);
                        inventory[num3] = item2.Clone();
                        item2.SetDefaults();
                        if (flag)
                            NetMessage.SendData(32, -1, -1, null, player.chest, num3);

                        break;
                    }
                }
            }

            return flag2;
        }

    }
}
