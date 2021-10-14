using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ThunderRoad;

namespace GhettosFirearmFramework
{
    class ItemModuleLAW : ItemModule
    {
        public string ProjectileID;
        public bool DebugMode;
        public String[] AmmoIDs;
        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            item.gameObject.AddComponent<ItemLAW>();
        }
    }
}
