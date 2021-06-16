using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using static Terraria.Player;

namespace QoLPrime
{
    class Detours
    {
        public static Player qolPlayer = Main.LocalPlayer;
        public static int myPlayer = Main.myPlayer;

        static List<Item> itemsToPack = new List<Item>();
        public static void drawBestHijack(On.Terraria.Main.orig_DrawBestiaryIcon orig, int pivotTopLeftX, int pivotTopLeftY)
        {
            if (!PlayerModification.backpackEnabled)
            {
                orig(pivotTopLeftX, pivotTopLeftY);
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
                num2 += QoLPrime.invBottomOffset;
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











                int itemGrabRange = GetItemGrabRange(item, self);
                ItemLoader.GrabRange(Main.item[j], self, ref itemGrabRange);
                Rectangle hitbox = item.Hitbox;
                if (self.Hitbox.Intersects(hitbox))
                {
                    if (i == Main.myPlayer && (self.inventory[self.selectedItem].type != 0 || self.itemAnimation <= 0))
                    {
                        if (!ItemLoader.OnPickup(Main.item[j], self))
                        {
                            Main.item[j] = new Item();
                            if (Main.netMode == 1)
                                NetMessage.SendData(21, -1, -1, null, j);
                            continue;
                        }
                        item = (Item)QoLPrime.PickupItem.Invoke(self, new object[] { i, j, item });
                    }
                }
                else
                {
                    if (!new Rectangle((int)self.position.X - itemGrabRange, (int)self.position.Y - itemGrabRange, self.width + itemGrabRange * 2, self.height + itemGrabRange * 2).Intersects(hitbox))
                        continue;

                    ItemSpaceStatus status = self.ItemSpace(item);
                    ItemSpaceStatus statusBackpack = ItemSpace(item, PlayerModification.backpack.item);
                    if (self.CanPullItem(item, status))
                    {
                        item.beingGrabbed = true;
                        if (ItemLoader.GrabStyle(item, self))
                        {

                        }

                        QoLPrime.PullItem_Pickup.Invoke(self, new object[] { item, 12f, 5 });

                    }
                    else if (self.CanPullItem(item, statusBackpack))
                    {
                        item.beingGrabbed = true;
                        if (ItemLoader.GrabStyle(item, self))
                        {

                        }
                        itemsToPack.Add(item);
                        QoLPrime.PullItem_Pickup.Invoke(self, new object[] { item, 12f, 5 });
                    }
                }
            }

        }
        public static void pullItemPickupHijack(On.Terraria.Player.orig_PullItem_Pickup orig, Player self, Item itemToPickUp, float speed, int acc)
        {

            orig(self, itemToPickUp, speed, acc);

        }
        public static void DepositAllHijack(On.Terraria.UI.ChestUI.orig_DepositAll orig)
        {

            if (PlayerModification.backpackEnabled)
            {
                DrawCustomChestUI.DepositAll();
            }
            orig();
        }
        public static void LootAllHijack(On.Terraria.UI.ChestUI.orig_LootAll orig)
        {
            orig();
            if (PlayerModification.backpackEnabled)
            {
                DrawCustomChestUI.LootAll();
            }
        }
        private static int GetItemGrabRange(Item item, Player self)
        {
            int num = Player.defaultItemGrabRange;
            if (self.goldRing && item.IsACoin)
                num += Item.coinGrabRange;

            if (self.manaMagnet && (item.type == 184 || item.type == 1735 || item.type == 1868))
                num += Item.manaGrabRange;

            if (item.type == 4143)
                num += Item.manaGrabRange;

            if (self.lifeMagnet && (item.type == 58 || item.type == 1734 || item.type == 1867))
                num += Item.lifeGrabRange;

            if (self.treasureMagnet)
                num += Item.treasureGrabRange;

            if (item.type == 3822)
                num += 50;

            if (ItemID.Sets.NebulaPickup[item.type])
                num += 100;

            return num;
        }
        public static Item PickupItemHijack(On.Terraria.Player.orig_PickupItem orig, Player self, int playerIndex, int worldItemArrayIndex, Item itemToPickUp)
        {


            if (itemsToPack.Contains(itemToPickUp))
            {

                for (int k = 0; k < 40; k++)
                {

                    if (PlayerModification.backpack.item[k].type == ItemID.None)
                    {
                        if (DrawCustomChestUI.TryPlacingInChest(itemToPickUp, PlayerModification.backpack))
                        {
                            //QoLPrime.Instance.Logger.Info($"{string.Join(',', PlayerModification.backpack.item[k].type)}");
                            itemsToPack.Remove(itemToPickUp);
                            PopupText.NewText(PopupTextContext.ItemPickupToVoidContainer, itemToPickUp, itemToPickUp.stack);
                            return new Item();
                        }


                    }
                    else
                    {
                        if (PlayerModification.updateCounter == 30)
                        {
                            //QoLPrime.Instance.Logger.Info($"{string.Join(',', PlayerModification.backpack.item[k].type)}");
                        }
                    }
                }

                return orig(self, playerIndex, worldItemArrayIndex, itemToPickUp);

            }
            else
            {

                return orig(self, playerIndex, worldItemArrayIndex, itemToPickUp);
            }

        }
        public static NetworkText DeathReasonHijack(On.Terraria.DataStructures.PlayerDeathReason.orig_GetDeathText orig, PlayerDeathReason self, string deadPlayerName)
        {
            int roll = Main.rand.Next(QoLPrime.customDeathMessages.Length);
            return NetworkText.FromLiteral(deadPlayerName + QoLPrime.customDeathMessages[roll]);

        }
        public static ItemSpaceStatus ItemSpace(Item newItem, Item[] inventory)
        {

            if (ItemID.Sets.IsAPickup[newItem.type])
                return new ItemSpaceStatus(CanTakeItem: true);

            if (newItem.uniqueStack && HasItem(newItem.type, inventory))
                return new ItemSpaceStatus(CanTakeItem: false);


            int num = 40;


            for (int i = 0; i < num; i++)
            {
                if (Main.LocalPlayer.CanItemSlotAccept(inventory[i], newItem))//Calling Player method, but using inventory of Backpack.
                    return new ItemSpaceStatus(CanTakeItem: true);
            }


            return new ItemSpaceStatus(CanTakeItem: false);
        }
        public static bool HasItem(int type, Item[] inventory)
        {
            for (int i = 0; i < 58; i++)
            {
                if (type == inventory[i].type && inventory[i].stack > 0)
                    return true;
            }

            return false;
        }
        public static int CountItems(Item[] inventoryToCount)
        {
            int count = 0;
            for (int i = 0; i < inventoryToCount.Length; i++)
            {
                if (inventoryToCount[i].type != 0)
                    count++;

            }
            return count;
        }

    }
}
