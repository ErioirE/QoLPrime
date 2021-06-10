using Microsoft.Xna.Framework;
using QoLPrime.Content.Players;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace QoLPrime.Items
{
    public class QuillRain : ModItem
    {
        public int BatsSlain = 0;
        private int killsRequired = 100;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("ThisBasedSword"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            //Tooltip.SetDefault($"(Shots from this weapon have 50% less damage and knockback.){Environment.NewLine}Use time: {Item.useTime}{Environment.NewLine}It defines \"bat\" as \"any winged mammal\".{Environment.NewLine}\"Bats\" exterminated: {BatsSlain}");
            DisplayName.SetDefault("Quill Rain, Bat Bane");

        }

        public override string Texture => "QoLPrime/Assets/Textures/Items/QuillRainTerraria";
        public int ammoUsed;
        public bool hasPrinted = false;
        public override void SetDefaults()
        {
            Item.useTime = 4;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item17;
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
            Item.damage = 1;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 26;
            Item.height = 26;
            Item.scale = 1f;
            Item.shootSpeed = 8f;
            Item.crit = 4;
            Item.shoot = AmmoID.Arrow;//ModContent.ProjectileType<Content.Projectiles.RevenantRevengeProjectile>();
            Item.useAmmo = AmmoID.Arrow;
            Item.value = 00160000;
            Item.knockBack = 1f;

            // This Ammo is nonspecific. I want to modify what it shoots, however.
            //Item.useAmmo = ModContent.ItemType<ExampleCustomAmmo>();
        }
        public override bool ConsumeAmmo(Player player)
        {
            return Main.rand.NextFloat() >= .55f;
        }
        public override bool Shoot(Player player, ProjectileSource_Item_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            PlayerModification.mostRecentQuillRain = this;
            /*var ProjectileUsed = Projectile.NewProjectileDirect(source,Vector2.Zero,Vector2.Zero,type,damage,knockback);
			if (ProjectileUsed != null) {
				ProjectileUsed.Kill();
			}*/


            type = ModContent.ProjectileType<Content.Projectiles.QuillRainProjectile>(); // or ProjectileID.FireArrow;

            Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedByRandom(MathHelper.ToRadians(8)); //12 is the spread in degrees, although like with Set Spread it's technically a 24 degree spread due to the fact that it's randomly between 12 degrees above and 12 degrees below your cursor.
            Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, knockback, player.whoAmI); //create the projectile
            return false; //makes sure it doesn't shoot the projectile again after this
                          // NewProjectile returns the index of the projectile it creates in the NewProjectile array.
                          // Here we are using it to gain access to the projectile object.

            //int combinedDamage = Item.damage + source.Item.damage;

            //int projectileID = Projectile.NewProjectile(source, position.X, position.Y,velocity.X,velocity.Y, type, damage, knockback, player.whoAmI);
            //Projectile projectile = Main.projectile[projectileID];


        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {

            int indexForTooltip = 0;
            foreach (TooltipLine line in tooltips)
            {
                if (line.isModifier)
                {

                }
                else
                {
                    indexForTooltip++;
                }
            }
            tooltips.Insert(indexForTooltip, new TooltipLine(QoLPrime.Instance, "batKillCount", $"Use time: {Item.useTime}{Environment.NewLine}(Shots from this weapon gain 50% less damage and knockback from arrows.){Environment.NewLine}\"Hello! Would you like to destroy some \'bats\' today?\"{Environment.NewLine}\"Bats\" exterminated: {(((Double)BatsSlain / (Double)killsRequired) * 100).ToString("0.##")}%"));


            return;

        }
        public override void UpdateInventory(Player player)
        {
            if (this.BatsSlain >= killsRequired && Main.hardMode)
            {
                if (PlayerModification.GetIndexOfItemInInventory(this, player) is int temp)
                {
                    var prefix = this.Item.prefix;
                    player.inventory[temp] = new Item(ModContent.ItemType<NightsBlood>());
                    player.inventory[temp].prefix = prefix;
                    player.inventory[temp].Refresh();
                }

            }
            //this.ModifyTooltips(new List<TooltipLine> { new TooltipLine(QoLPrime.Instance,"name","Blah")});
            //Item.RebuildTooltip();
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.WoodenBow, 1);
            recipe.AddIngredient(ItemID.RainCloud, 20);
            recipe.AddIngredient(ItemID.Stinger, 5);
            recipe.Register();
        }

        public override ModItem Clone(Item item)
        {
            var clone = (QuillRain)base.Clone(item);
            clone.BatsSlain = BatsSlain;
            return clone;
        }
        public override void Load(TagCompound tag)
        {
            BatsSlain = tag.GetInt("BatsSlain");
        }

        public override TagCompound Save()
        {
            return new TagCompound
        {
            { "BatsSlain", BatsSlain}
        };
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(BatsSlain);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BatsSlain = reader.ReadInt32();
        }


    }



}