using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ThunderRoad;

namespace GhettosFirearmFramework
{
    class ItemExplosionCollision : MonoBehaviour
    {
        float force;
        float blastRadius;
        float liftMult;

        Item item;
        ItemModuleExplosionCollision module;
        ParticleSystem vfx;
        AudioSource sfx;

        private void OnCollisionEnter(Collision hit)
        {
            Detonate();
        }

        public void Awake()
        {
            item = base.GetComponent<Item>();
            module = item.data.GetModule<ItemModuleExplosionCollision>();
            force = module.explosionForce;
            blastRadius = module.blastRadius;
            liftMult = module.liftMultiplier;
        }

        private void Detonate()
        {
            vfx.transform.parent = null;
            sfx.transform.parent = null;
            //vfx.Play();
            //sfx.Play();
            HitscanExplosion(this.transform.position, force, blastRadius, liftMult);
            item.Despawn();
        }

        public static void HitscanExplosion(Vector3 origin, float force, float blastRadius, float liftMult, ForceMode forceMode = ForceMode.Impulse)
        {
            try
            {
                foreach (Item item in Item.list)
                {
                    if (Math.Abs(Vector3.Distance(item.transform.position, origin)) <= blastRadius)
                    {
                        Debug.Log("LAW: Hit Item: " + item.name);
                        item.rb.AddExplosionForce(force * item.rb.mass, origin, blastRadius, liftMult, forceMode);
                        item.rb.AddForce(Vector3.up * liftMult * item.rb.mass, forceMode);
                    }
                }

                foreach (Creature creature in Creature.list)
                {
                    if (creature == Player.currentCreature) continue;
                    if (Math.Abs(Vector3.Distance(creature.transform.position, origin)) <= blastRadius)
                    {
                        // Kill Creatures in Range
                        Debug.Log("LAW: Hit Creature: " + creature.name);
                        if (!creature.isKilled)
                        {
                            Debug.Log("LAW: Damaging Creature: " + creature.name);
                            creature.Damage(new CollisionInstance(new DamageStruct(DamageType.Energy, 9999f), (MaterialData)null, (MaterialData)null));
                        }
                        // Apply Forces to Creature Main Body
                        creature.locomotion.rb.AddExplosionForce(force * creature.locomotion.rb.mass, origin, blastRadius, liftMult, forceMode);
                        creature.locomotion.rb.AddForce(Vector3.up * liftMult * creature.locomotion.rb.mass, forceMode);

                        //// Dismember Creature Parts
                        creature.ragdoll.headPart.Slice();
                        creature.ragdoll.GetPart(RagdollPart.Type.LeftLeg).Slice();
                        creature.ragdoll.GetPart(RagdollPart.Type.RightLeg).Slice();
                        creature.ragdoll.GetPart(RagdollPart.Type.RightArm).Slice();
                        creature.ragdoll.GetPart(RagdollPart.Type.LeftArm).Slice();

                        // Apply Forces to Creature Parts
                        foreach (RagdollPart part in creature.ragdoll.parts)
                        {
                            Debug.Log("LAW: Appyling Force to RD-part " + part.name);
                            part.rb.AddExplosionForce(force * part.rb.mass, origin, blastRadius, liftMult, forceMode);
                            part.rb.AddForce(Vector3.up * liftMult * part.rb.mass, forceMode);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("LAW EXCEPTION: " + e.Message + " \n " + e.StackTrace);
            }
        }
    }
}
