using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using QoLPrime.Content.Buffs;
using QoLPrime.Content.Players;
using QoLPrime.Items;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.Player;

namespace QoLPrime
{
    class Detours
    {
        public static Player qolPlayer = Main.LocalPlayer;
        public static int myPlayer = Main.myPlayer;









        public static void drawNpcsHijack(On.Terraria.Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            orig(self, behindTiles);
            int xOffset = 0;
            int yOffset = 15;
            float scaleMult = NightsBlood.scaleToAddToIcon;
            Texture2D manaTexture = RavenousBuff.textureToUseForMana.Value;
            Texture2D lifeTexture = RavenousBuff.textureToUseForLife.Value;
            Color iconColorIfMana = new Color(255, 255, 255) * PlayerModification.fadeMultipler;
            Color iconColorIfLife = new Color(255, 255, 255) * PlayerModification.fadeMultipler; ;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);
            if (Main.LocalPlayer.HasBuff(ModContent.BuffType<RavenousBuff>()) && !NightsBlood.willHurt)
            {
                Texture2D textureToUse = RavenousBuff.textureToUseForLife.Value;
                Main.spriteBatch.Draw(manaTexture, new Vector2(Main.screenWidth / 2 - ((manaTexture.Width * scaleMult + Player.defaultWidth) / 2), Main.screenHeight / 2 - ((manaTexture.Height * scaleMult) + (Player.defaultHeight + yOffset))), new Microsoft.Xna.Framework.Rectangle(0, 0, RavenousBuff.textureToUseForMana.Width(), RavenousBuff.textureToUseForMana.Height()), iconColorIfMana, 0f, default(Vector2), 1f + scaleMult, SpriteEffects.None, 0f);
            }
            else if (Main.LocalPlayer.HasBuff(ModContent.BuffType<RavenousBuff>()) && NightsBlood.willHurt)
            {

                Main.spriteBatch.Draw(lifeTexture, new Vector2(Main.screenWidth / 2 - ((lifeTexture.Width * scaleMult + Player.defaultWidth) / 2), Main.screenHeight / 2 - ((lifeTexture.Height * scaleMult) + (Player.defaultHeight + yOffset))), new Microsoft.Xna.Framework.Rectangle(0, 0, RavenousBuff.textureToUseForLife.Width(), RavenousBuff.textureToUseForLife.Height()), iconColorIfLife, 0f, default(Vector2), 1f + scaleMult, SpriteEffects.None, 0f);
            }
        }


        public static NetworkText DeathReasonHijack(On.Terraria.DataStructures.PlayerDeathReason.orig_GetDeathText orig, PlayerDeathReason self, string deadPlayerName)
        {
            NetworkText originalMessage = orig(self, deadPlayerName);
            
            if (!QoLPrime.customDeathMessages.ToList().Contains(originalMessage.ToString()))
            {
                int roll = Main.rand.Next(QoLPrime.customDeathMessages.Length);
                return NetworkText.FromLiteral(deadPlayerName + QoLPrime.customDeathMessages[roll]);
            }
            else
            {
                return originalMessage;
            }
            

        }
        
       

    }
}
