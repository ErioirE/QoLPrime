using QoLPrime.Common.GlobalProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using QoLPrime.Content.Items.Ammo;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;

namespace QoLPrime.Items
{
	public class RevenantsRevenge : ModItem
	{
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("ThisBasedSword"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			Tooltip.SetDefault($"Converts arrows into high-velocity revenant arrows that are able to phase through thin barriers and seek out enemies.{Environment.NewLine}This seeking has greater range with each successive hit.");
			DisplayName.SetDefault("The Revenant's Revenge");

		}

		public override string Texture => "QoLPrime/Assets/Textures/Items/RevenantsRevenge";
		public int ammoUsed;
		public override void SetDefaults()
		{
			bool canShoot = true;
			Item.useTime = 26;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.UseSound = SoundID.Item5;
			Item.rare = ItemRarityID.LightPurple;
			Item.autoReuse = true;
			Item.damage = 80;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 26;
			Item.height = 26;
			Item.scale = 1f;
			Item.shootSpeed = 85f;
			Item.crit = 36;
			Item.shoot = AmmoID.Arrow;//ModContent.ProjectileType<Content.Projectiles.HomingMarrowProjectile>();
			Item.useAmmo = AmmoID.Arrow;
	
			

			// This Ammo is nonspecific. I want to modify what it shoots, however.
			//Item.useAmmo = ModContent.ItemType<ExampleCustomAmmo>();
		}

		public override bool Shoot(Player player, ProjectileSource_Item_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{

			
			var ProjectileUsed = Projectile.NewProjectileDirect(source,Vector2.Zero,Vector2.Zero,type,damage,knockback);
			if (ProjectileUsed != null) {
				//Main.NewText($"Shooted: {ProjectileUsed.Name}");
			}
			ProjectileUsed.Kill();

			type = ModContent.ProjectileType<Content.Projectiles.HomingMarrowProjectile>(); // or ProjectileID.FireArrow;
			
			// NewProjectile returns the index of the projectile it creates in the NewProjectile array.
			// Here we are using it to gain access to the projectile object.

			int combinedDamage = Item.damage + source.Item.damage;
			
			int projectileID = Projectile.NewProjectile(source, position.X, position.Y,velocity.X,velocity.Y, type, damage, knockback, player.whoAmI);
			Projectile projectile = Main.projectile[projectileID];
			
			// We do not want vanilla to spawn a duplicate projectile.
			return false;
		}

		public override void AddRecipes() 
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.DirtBlock, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}