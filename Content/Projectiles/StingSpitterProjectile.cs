using Microsoft.Xna.Framework;
using QoLPrime.Content.Buffs;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace QoLPrime.Content.Projectiles
{
    // This Example show how to implement simple homing projectile
    // Can be tested with ExampleCustomAmmoGun
    public class StingSpitterProjectile : ModProjectile
    {

        bool hasAccelerated = false;

        float speedMod = 1f;
        int lastNumHits = 0;
        float brightness = 0.3f;

        bool boosted = false;


        Vector2 originalVelocity;
        static float defaultDetectRadius = 95f;
        float maxDetectRadius = defaultDetectRadius; // The maximum radius at which a projectile can detect a target
        int lastHit = 0;
        Random rand = new Random();
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sting Spitter Projectile"); // Name of the projectile. It can be appear in chat
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Tell the game that it is a homing projectile

        }
        public override string Texture => "QoLPrime/Assets/Textures/Items/sting";
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
            Projectile.light = .01f; // How much light emit around the projectile
            Projectile.tileCollide = true; // Can the projectile collide with tiles?
            Projectile.timeLeft = 120; //The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.extraUpdates = 0;
            int maxChains = rand.Next(3, 8);
            Projectile.penetrate = maxChains;
            Projectile.maxPenetrate = maxChains;
            Projectile.knockBack = .005f;
            
        }

        // Custom AI
        public override void AI()
        {
            AddLightToScale(Projectile.position, Color.Green.ToVector3(), brightness);
            int percentTimeLeft = (int)(1 * ((130 * this.Projectile.timeLeft) / 100));
            brightness = (float)percentTimeLeft;

            if (!hasAccelerated)
            {
                originalVelocity = Projectile.velocity;
                hasAccelerated = true;
                //Main.NewText(Projectile.maxPenetrate);
            }
            // The speed at which the projectile moves towards the target
            float projSpeed = 5f;// Trying to find NPC closest to the projectile
            NPC closestNPC = FindClosestNPC(maxDetectRadius, lastHit);


            speedMod = 1.3f;
            Random rand = new Random();
            //Main.NewText($"Last Hit: {lastHit}");
            if (Projectile.numHits > lastNumHits)
            {
                Projectile.damage = (int)Math.Round(0.95 * Projectile.damage);
                if (closestNPC != null)
                {
                    lastHit = closestNPC.life;
                }

                Projectile.velocity.X *= (float)rand.Next(2, 4);
                Projectile.velocity.Y *= (float)rand.Next(2, 4);
                lastNumHits = Projectile.numHits;
            }
            else
            {
                Projectile.velocity.X = originalVelocity.X * speedMod;
                Projectile.velocity.Y = originalVelocity.Y * speedMod;
                projSpeed *= speedMod;
            }
            if (closestNPC == null)
            {


                if (Projectile.numHits > 0)
                {
                    maxDetectRadius = (defaultDetectRadius + (Projectile.numHits * 150));
                    if (maxDetectRadius > 500)
                    {
                        maxDetectRadius = 500;
                    }
                }
                Tile? tileAtLocation = null;
                try
                {
                    var pos = Projectile.position;
                    //Main.NewText($"Pos: {pos.X/16f} * {pos.Y/16f}");
                    tileAtLocation = Main.tile[(int)Math.Round(pos.X / 16f), ((int)Math.Round(pos.Y / 16f))];
                }
                catch (IndexOutOfRangeException ex)
                {
                    Console.WriteLine(ex);
                }
                
                return;
            }
            else
            {
                //Projectile.tileCollide = false;
            }
            // If found, change the velocity of the projectile and turn it in the direction of the target
            // Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero 
            Vector2 target = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
            Projectile.velocity = target * projSpeed;
            float scaleFactor = Projectile.velocity.Length();
            float targetAngle = Projectile.Center.AngleTo(target);
            Vector2 thing1 = (Projectile.velocity.ToRotation().AngleTowards(targetAngle, (float)Math.PI / 180f).ToRotationVector2() * scaleFactor);

            Projectile.rotation = thing1.ToRotation() + 1.5708f;

        }
        public float reduceSpeed(float current, float multiplier, float minimum = 0.03f)
        {
            float temp = (current *= multiplier);
            if (temp >= minimum)
            {
                return temp;
            }
            return minimum;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            this.Projectile.timeLeft += damage / 2;
            target.AddBuff(ModContent.BuffType<RavenousBuff>(),20,true);
            target.AddBuff(BuffID.Venom, 250);
        }
        public void AddLightToScale(Vector2 position, Vector3 rgb, float multiplier)
        {
            float trueMult = multiplier / 100;
            Lighting.AddLight(Projectile.position, new Vector3(rgb.X * trueMult, rgb.Y * trueMult, rgb.Z * trueMult));
        }
        // Finding the closest NPC to attack within maxDetectDistance range
        // If not found then returns null
        public NPC FindClosestNPC(float maxDetectDistance, int lastHit)
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
                    if (!target.HasBuff(ModContent.BuffType<RavenousBuff>()) && sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;

                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }
    }
}
