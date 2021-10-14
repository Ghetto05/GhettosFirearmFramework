using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ThunderRoad;

namespace GhettosFirearmFramework
{
    class ItemModuleExplosionCollision : ItemModule
    {
        public float ExplosionForce;
        public float BlastRadius;
        public float PlayerForceMultiplyer;

        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            item.gameObject.AddComponent<ItemExplosionCollision>();
        }
    }
}