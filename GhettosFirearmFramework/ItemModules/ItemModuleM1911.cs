using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ThunderRoad;

namespace GhettosFirearmFramework
{
    class ItemModuleM1911 : ItemModule
    {
        //AMMO
        public string Caliber;
        public string MagazineType;
        public float SlideTravelDistance;
        public float BlowBackForce;
        public bool GenerateSlideJoint;

        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            item.gameObject.AddComponent<ItemM1911>();
        }
    }
}
