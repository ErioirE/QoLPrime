using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace QoLPrime.Content.Buffs
{
    public class RavenousBuff : ModBuff
    {
        public static int counter = 0;
        public static string texturePath = "QoLPrime/Assets/Textures/Buffs/RavenousBuff";
        public static string texturePathMana = "QoLPrime/Assets/Textures/Buffs/RavenousBuffMana";
        public static string texturePathLife = "QoLPrime/Assets/Textures/Buffs/RavenousBuffLife";
        public static Asset<Texture2D> textureToUseForMana = (Asset<Texture2D>)ModContent.GetTexture(RavenousBuff.texturePathMana);
        public static Asset<Texture2D> textureToUseForLife = (Asset<Texture2D>)ModContent.GetTexture(RavenousBuff.texturePathLife);
        public override string Texture => texturePath;
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Night's Blood is ravenous...");
            Description.SetDefault("Firing Night's blood consumes a growing amout of mana per shot, and gains bonus damage proportional to the cost. Consumes LIFE once mana is gone!");

            Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = false; //Add this so the nurse doesn't remove the buff when healing

        }
        public RavenousBuff()
        {

        }

        public override void Update(Player player, ref int buffIndex)
        {

        }

    }
}
