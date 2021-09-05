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
        public float explosionForce;
        public float blastRadius;
        public float liftMultiplier;

        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            item.gameObject.AddComponent<ItemExplosionCollision>();
        }
    }
}