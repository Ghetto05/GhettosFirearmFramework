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
        Item item;
        ItemModuleExplosionCollision module;

        AudioSource explosionSFX;
        ParticleSystem explosionVFX;

        private void OnCollisionEnter(Collision hit)
        {
            Detonate();
        }

        public void Awake()
        {
            item = base.GetComponent<Item>();
            module = item.data.GetModule<ItemModuleExplosionCollision>();
            explosionVFX = item.transform.Find("ExplosionVFX").GetComponent<ParticleSystem>();
            explosionSFX = item.transform.Find("ExplosionSFX").GetComponent<AudioSource>();
        }

        private void Detonate()
        {
            explosionVFX.transform.parent = null;
            explosionSFX.transform.parent = null;
            explosionVFX.Play();
            explosionSFX.Play();

            Vector3 explosionPos = item.transform.position;
            Collider[] colliders = Physics.OverlapSphere(explosionPos, module.BlastRadius);
            foreach (Collider hit in colliders)
            {
                if (hit.GetComponent<Rigidbody>() != null)
                {
                    Creature cr;
                    Rigidbody rb = hit.GetComponent<Rigidbody>();
                    if (rb.gameObject.GetComponentInParent<Creature>() != null && rb.gameObject.GetComponentInParent<Creature>() != Player.local.creature)
                    {
                        cr = rb.gameObject.GetComponentInParent<Creature>();
                        cr.Kill();
                        cr.locomotion.rb.AddExplosionForce(module.ExplosionForce, explosionPos, module.BlastRadius, 3.0F, ForceMode.Impulse);
                        foreach (RagdollPart part in cr.ragdoll.parts)
                        {
                            part.rb.AddExplosionForce(module.ExplosionForce, explosionPos, module.BlastRadius, 3.0F, ForceMode.Impulse);
                        }
                    }
                    else if (rb.gameObject.GetComponentInParent<Creature>() == Player.local.creature || rb.gameObject.GetComponentInParent<Creature>() == null)
                    {
                        rb.AddExplosionForce(module.ExplosionForce * module.PlayerForceMultiplyer, explosionPos, module.BlastRadius, 3.0F, ForceMode.Impulse);
                    }
                }
            }
            item.Despawn(0);
        }
    }
}
