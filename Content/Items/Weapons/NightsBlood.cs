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
using QoLPrime.Content.Players;
using QoLPrime.Content.Buffs;

namespace QoLPrime.Items
{
	public class NightsBlood : ModItem
	{
		
		public override void SetStaticDefaults() 
		{
			Tooltip.SetDefault($"Use time: {Item.useTime}{Environment.NewLine}Consumes mana when fired. If mana is insufficient, consumes life. Gains damage proportional to cost.{Environment.NewLine}Gains 1 bonus damage per 50 enemy maximum life, up to a cap of 10.{Environment.NewLine}Applies a short ichor debuff{Environment.NewLine}You gain mana on kill with this weapon equal to the max life of the enemy slain.{Environment.NewLine}It shivers with latent power, biding its time.");
			DisplayName.SetDefault("Night's Blood, the Ravenous");
		}
		public int cost = 0;
		public override string Texture => "QoLPrime/Assets/Textures/Items/NightsBlood";
		public int ammoUsed;
		public bool hasPrinted = false;
		private int ravenousBuff = ModContent.BuffType<RavenousBuff>();
		string deathText = "";
		public static bool willHurt = false;
		PlayerDeathReason reason;
		public override void SetDefaults()
		{
			Item.useTime = 10;
			Item.useAnimation = 10;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.UseSound = SoundID.Item17;
			Item.rare = ItemRarityID.Orange;
			Item.autoReuse = true;
			Item.damage = 2;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 26;
			Item.height = 26;
			Item.scale = 1f;
			Item.shootSpeed = 11f;
			Item.crit = 7;
			Item.shoot = AmmoID.Arrow;//ModContent.ProjectileType<Content.Projectiles.RevenantRevengeProjectile>();
			Item.useAmmo = AmmoID.Arrow;
			Item.value = 00200000;
			Item.knockBack = 1f;
			
			

		}
		
		public override bool ConsumeAmmo(Player player)
		{
			return Main.rand.NextFloat() >= .55f;
		}
		public override bool Shoot(Player player, ProjectileSource_Item_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			
			if (deathText.Length < 1) {
				deathText = $"{player.name} couldn't feed Night's Blood.";
				
			}
			PlayerDeathReason reason = PlayerDeathReason.ByCustomReason(deathText);
			player.AddBuff(ravenousBuff, 60 * 5);
			cost = (player.statManaMax / 100)* (1 + (int)(RavenousBuff.counter/2));

                if (player.statMana - cost < 0)
                {
					int diff = (cost - player.statMana);
					player.statMana = 0;
					player.manaRegenDelay = 200;
					if (player.statLife - diff >= 1) {
						player.statLife -= diff;
					}
					else
					{
						player.statLife = 1;
					}
					if (player.statLife <= 1) {
						player.Hurt(reason, 1, 0, quiet: true);
					}
				}
                else
                {
					player.statMana -= cost;
				}

			damage += cost;
				RavenousBuff.counter++;
			
			type = ModContent.ProjectileType<Content.Projectiles.NightsBloodProjectile>(); // or ProjectileID.FireArrow;
			int roll = Main.rand.Next(0,9);
			Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedByRandom(MathHelper.ToRadians(4)); //12 is the spread in degrees, although like with Set Spread it's technically a 24 degree spread due to the fact that it's randomly between 12 degrees above and 12 degrees below your cursor.
			Projectile.NewProjectile(source,position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, knockback, player.whoAmI);

			if (roll >= 8)
            {
				perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedByRandom(MathHelper.ToRadians(4));
				Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, knockback, player.whoAmI);
			}
			
			return false; //makes sure it doesn't shoot the projectile again after this
						  // NewProjectile returns the index of the projectile it creates in the NewProjectile array.
						  // Here we are using it to gain access to the projectile object.

			//int combinedDamage = Item.damage + source.Item.damage;

			//int projectileID = Projectile.NewProjectile(source, position.X, position.Y,velocity.X,velocity.Y, type, damage, knockback, player.whoAmI);
			//Projectile projectile = Main.projectile[projectileID];
			

		}

		public override void UpdateInventory(Player player)
		{
			if (player.statMana - cost < 0)
			{
				willHurt = true;
			}
			else
			{
				willHurt = false;
			}
			//this.ModifyTooltips(new List<TooltipLine> { new TooltipLine(QoLPrime.Instance,"name","Blah")});
			//Item.RebuildTooltip();
		}


	}

	

}