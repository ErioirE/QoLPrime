using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace QoLPrime.Content.Projectiles
{
    // This Example show how to implement simple homing projectile
    // Can be tested with ExampleCustomAmmoGun
    public class QuillStormProjectile : ModProjectile
    {
        float speedMod = 1f;

        bool boosted = false;
        bool hasAccelerated = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Quill Storm Projectile"); // Name of the projectile. It can be appear in chat
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Tell the game that it is a homing projectile

        }
        public override string Texture => "QoLPrime/Assets/Textures/Items/Quill";
        // Setting the default parameters of the projectile
        // You can check most of Fields and Properties here https://github.com/tModLoader/tModLoader/wiki/Projectile-Class-Documentation
        public override void SetDefaults()
        {
            Projectile.width = 3; // The width of projectile hitbox
            Projectile.height = 10; // The height of projectile hitbox
            Projectile.aiStyle = ProjAIStyleID.Arrow; // The ai style of the projectile (0 means custom AI). For more please reference the source code of Terraria
            Projectile.DamageType = DamageClass.Ranged; // What type of damage does this projectile affect?
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = false; // Is the projectile's speed be influenced by water?
            Projectile.tileCollide = true; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 1;

        }

        // Custom AI
        public override void AI()
        {
            Random rand = new Random();
            NPC closest = FindClosestNPC(75);
            float projSpeed = 10f;




            if (!hasAccelerated)
            {
                Projectile.damage = (int)Math.Round(Projectile.damage * 0.5);
                Projectile.knockBack = (int)Math.Round(Projectile.knockBack * 0.5);
                int invert = rand.Next(0, 1);
                if (invert > 0)
                {
                    speedMod = (5f + (float)rand.NextDouble()) / 5;

                }
                else
                {
                    speedMod = (5f - (float)rand.NextDouble()) / 5;
                }
                Projectile.velocity.X *= speedMod;
                invert = rand.Next(0, 1);
                if (invert > 0)
                {
                    speedMod = (5f + (float)rand.NextDouble()) / 5;

                }
                else
                {
                    speedMod = (5f - (float)rand.NextDouble()) / 5;
                }
                Projectile.velocity.Y *= speedMod;
                Projectile.direction = (int)((float)Projectile.direction * speedMod);
                hasAccelerated = true;
            }



            if (!boosted)
            {
                
                boosted = true;
            }


            // If found, change the velocity of the projectile and turn it in the direction of the target
            // Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero 
            try
            {
                if (closest != null) {
                    Vector2 target = (closest.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                    Projectile.velocity = target * projSpeed;
                    float scaleFactor = Projectile.velocity.Length();
                    float targetAngle = Projectile.Center.AngleTo(target);
                    Vector2 thing1 = (Projectile.velocity.ToRotation().AngleTowards(targetAngle, (float)Math.PI / 180f).ToRotationVector2() * scaleFactor);

                    Projectile.rotation = thing1.ToRotation() + 1.5708f;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            // The speed at which the projectile moves towards the target
            // Trying to find NPC closest to the projectile


            //return;
            //Main.NewText($"Last Hit: {lastHit}");


        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Ichor, 60);
            target.AddBuff(BuffID.Poisoned, 250);
        }
        public NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs(max always 200)
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC target = Main.npc[k];
                // Check if NPC able to be targeted. It means that NPC is
                // 1. active (alive)
                // 2. chaseable (e.g. not a cultist archer)
                // 3. max life bigger than 5 (e.g. not a critter)
                // 4. can take damage (e.g. moonlord core after all it's parts are downed)
                // 5. hostile (!friendly)
                // 6. not immortal (e.g. not a target dummy)
                if (target.CanBeChasedBy())
                {
                    // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    // Check if it is within the radius
                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }

        // Finding the closest NPC to attack within maxDetectDistance range
        // If not found then returns null

    }
}
