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
