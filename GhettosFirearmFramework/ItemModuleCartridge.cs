using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ThunderRoad;

namespace GhettosFirearmFramework
{
    class ItemModuleCartridge : ItemModule
    {
        public String MagazineType;
        public String Caliber;

        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            item.gameObject.AddComponent<ItemCartridge>();
        }
    }
}
