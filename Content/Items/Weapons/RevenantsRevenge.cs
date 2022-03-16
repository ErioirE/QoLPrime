using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace QoLPrime.Items
{
    public class RevenantsRevenge : ModItem
    {

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("ThisBasedSword"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            //Tooltip.SetDefault($"Use time: {Item.useTime}{Environment.NewLine}Applies a short ichor debuff.{Environment.NewLine}Converts arrows into high-velocity revenant arrows that are able to phase through thin barriers and seek out enemies.{Environment.NewLine}Revenant arrows seeking range increases for each consecutive enemy hit, at the cost of 5% reduced damage.");
            DisplayName.SetDefault("The Revenant's Revenge");

        }

        public override string Texture => "QoLPrime/Assets/Textures/Items/RevenantsRevenge";
        public int ammoUsed;
        public override void SetDefaults()
        {
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item5;
            Item.rare = ItemRarityID.Lime;
            Item.autoReuse = true;
            Item.damage = 85;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 26;
            Item.height = 26;
            Item.scale = 1f;
            Item.shootSpeed = 20f;
            Item.crit = 20;
            Item.shoot = AmmoID.Arrow;//ModContent.ProjectileType<Content.Projectiles.RevenantRevengeProjectile>();
            Item.useAmmo = AmmoID.Arrow;
            Item.value = 01600000;
            Item.knockBack = 1f;

            // This Ammo is nonspecific. I want to modify what it shoots, however.
            //Item.useAmmo = ModContent.ItemType<ExampleCustomAmmo>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {


            /*var ProjectileUsed = Projectile.NewProjectileDirect(source,Vector2.Zero,Vector2.Zero,type,damage,knockback);
			if (ProjectileUsed != null) {
				ProjectileUsed.Kill();
			}*/


            type = ModContent.ProjectileType<Content.Projectiles.RevenantRevengeProjectile>(); // or ProjectileID.FireArrow;

            // NewProjectile returns the index of the projectile it creates in the NewProjectile array.
            // Here we are using it to gain access to the projectile object.

            //int combinedDamage = Item.damage + source.Item.damage;

            int projectileID = Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI);
            Projectile projectile = Main.projectile[projectileID];

            // We do not want vanilla to spawn a duplicate projectile.
            return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Marrow, 1);
            recipe.AddIngredient(ItemID.StakeLauncher, 1);
            recipe.AddIngredient(ModContent.ItemType<NightsBlood>(), 1);
            recipe.AddIngredient(ItemID.Ectoplasm, 20);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
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
            tooltips.Insert(indexForTooltip, new TooltipLine(QoLPrime.Instance, "RevenantsRevengeTooltip", $"Use time: {Item.useTime}{Environment.NewLine}Gains 5 bonus damage per 500 enemy maximum life, up to a cap of 50.{Environment.NewLine}Applies a short ichor debuff.{Environment.NewLine}Converts arrows into high-velocity revenant arrows that are able to phase through thin barriers and seek out enemies.{Environment.NewLine}Revenant arrows seeking range increases for each consecutive enemy hit, at the cost of 5% reduced damage."));


            return;

        }
    }
}