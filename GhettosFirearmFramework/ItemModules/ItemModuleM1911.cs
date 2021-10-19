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
        string Caliber;
        string MagazineType;

        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            item.gameObject.AddComponent<ItemM1911>();
        }
    }
}
