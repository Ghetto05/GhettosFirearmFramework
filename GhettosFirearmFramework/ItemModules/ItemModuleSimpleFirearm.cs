using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ThunderRoad;

namespace GhettosFirearmFramework
{
    class ItemModuleSimpleFirearm : ItemModule
    {
        public string ProjectileID;
        public int AmmoCount;
        public float ProjectileForce;
        public float FireRate;
        public String[] Firemodes;
        public float BulletDespawnTime = 5.0f;
        public float RecoilForce;
        public bool HasBoltCatch = true;
        //public string Firemode;

        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            item.gameObject.AddComponent<ItemSimpleFirearm>();
        }
    }
}
