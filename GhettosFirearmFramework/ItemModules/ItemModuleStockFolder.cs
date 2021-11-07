using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ThunderRoad;

namespace GhettosFirearmFramework
{
    class ItemModuleStockFolder : ItemModule
    {
        public string AnimatorComponentReference;
        public string HandleName;
        public bool UsesTrigger;
        public bool UsesAlternateUse;

        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            item.gameObject.AddComponent<ItemStockFolder>();
        }
    }
}
