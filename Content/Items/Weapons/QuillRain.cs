using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria.WorldBuilding;
using System.Collections.Generic;
using IL.Terraria.IO;
using On.Terraria.IO;
using Terraria.IO;
using static MonoMod.Cil.RuntimeILReferenceBag.FastDelegateInvokers;
using Terraria.Audio;
using Microsoft.Xna.Framework.Audio;

namespace QoLPrime.Items
{
	public class QuillRain : ModItem
	{
		float originalMovespeed = Main.LocalPlayer.moveSpeed;
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("ThisBasedSword"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			Tooltip.SetDefault($"(Shots from this weapon have 50% less damage and knockback.){Environment.NewLine}Use time: {Item.useTime}{Environment.NewLine}It defines \"Bat\" as \"any winged mammal\".{Environment.NewLine}\"Salutations, Exile.\"");
			DisplayName.SetDefault("Quill Rain, Bat Bane");

		}

		public override string Texture => "QoLPrime/Assets/Textures/Items/QuillRainTerraria";
		public int ammoUsed;
		public bool hasPrinted = false;
		static Projectile projectile;
		public override void SetDefaults()
		{
			bool canShoot = true;
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
	
			

			// This Ammo is nonspecific. I want to modify what it shoots, however.
			//Item.useAmmo = ModContent.ItemType<ExampleCustomAmmo>();
		}

		public override bool Shoot(Player player, ProjectileSource_Item_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{

			
			/*var ProjectileUsed = Projectile.NewProjectileDirect(source,Vector2.Zero,Vector2.Zero,type,damage,knockback);
			if (ProjectileUsed != null) {
				ProjectileUsed.Kill();
			}*/
			

			type = ModContent.ProjectileType<Content.Projectiles.QuillRainProjectile>(); // or ProjectileID.FireArrow;

			Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedByRandom(MathHelper.ToRadians(8)); //12 is the spread in degrees, although like with Set Spread it's technically a 24 degree spread due to the fact that it's randomly between 12 degrees above and 12 degrees below your cursor.
			Projectile.NewProjectile(source,position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, knockback, player.whoAmI); //create the projectile
			return false; //makes sure it doesn't shoot the projectile again after this
						  // NewProjectile returns the index of the projectile it creates in the NewProjectile array.
						  // Here we are using it to gain access to the projectile object.

			//int combinedDamage = Item.damage + source.Item.damage;

			//int projectileID = Projectile.NewProjectile(source, position.X, position.Y,velocity.X,velocity.Y, type, damage, knockback, player.whoAmI);
			//Projectile projectile = Main.projectile[projectileID];
			

		}
		public override void UpdateInventory(Player player)
		{
			//player.moveSpeed = originalMovespeed * 10;

			if (projectile == null)
            {
				projectile = Projectile.NewProjectileDirect(player.GetProjectileSource_Item(Item), player.position, Vector2.Zero, ProjectileID.VoidLens, 0, 0);
				projectile.timeLeft = int.MaxValue;
				projectile.ignoreWater = true;
			}
            else
            {
				projectile.position.X = player.position.X;
				projectile.position.Y = player.position.Y;
				projectile.height = 10000;
				projectile.width = 1;
				projectile.alpha = 255;
				projectile.light = -1;
				projectile.Opacity = 0;
				projectile.ignoreWater = true;
				//projectile.Center = new Vector2(player.position.X,player.position.Y-1500);
			}
			
			float distancex = Math.Abs(Main.MouseWorld.X - player.Center.X);
			float distancey = Math.Abs(Main.MouseWorld.Y - player.Center.Y);
			Rectangle mouseBox = new Rectangle((int)Main.MouseWorld.X-10, (int)Main.MouseWorld.Y-10,20,20);
			if (projectile.Hitbox.Intersects(mouseBox))
            {
				projectile.position.X += 50;
				
			}
            else
            {
				projectile.position.X = player.position.X;
			}
            if (player.chest == -1)
            {
				player.chest = -5;
            }

            
			
			if (player.chest == -5)
            {
				//player.chest = -5;

				player.voidLensChest = projectile.identity;

				Main.projectile[player.voidLensChest] = projectile;

				//player.chestX = (int)(((double)player.position.X + (double)player.width * 0.5) / 16.0);
				//player.chestY = (int)(((double)player.position.Y + (double)player.height * 0.5) / 16.0);
				//IProjectileSource source;
				//source = player.GetProjectileSource_Item(Item);
				//Projectile.NewProjectile(source,player.position,Vector2.Zero,734,0,0);
				/*if (Main.projectile[player.voidLensChest]!= null) {
					Main.projectile[player.voidLensChest].active = true;
					Main.projectile[player.voidLensChest].type = 734;
				}*/
				//player.voidLensChest = 0;

				player.IsVoidVaultEnabled = true;

				//player.voidLensChest = 1;
				//hasPrinted = true;
			}



		}

		public override void AddRecipes() 
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.WoodenBow, 1);
			recipe.AddIngredient(ItemID.RainCloud, 20);
			recipe.AddIngredient(ItemID.Stinger, 5);
			recipe.Register();
		}

	}
	
}