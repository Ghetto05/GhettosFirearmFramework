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
    class ItemMagazine : MonoBehaviour
    {
        Item item;
        ItemModuleMagazine module;
        string[] rounds;
        float roundCount;

        public void Awake()
        {
            item = base.GetComponent<Item>();
            module = item.data.GetModule<ItemModuleMagazine>();
        }

        public void LoadOne()
        {
            int o = 0;
            int l = rounds.Length;
            for (int i = 0; i < l; i++)
            {
                rounds[i + 1] = rounds[i];
                o = i;
            }
            rounds[0] = null; //actually round but don't have round detection yet
        }

        public void UnloadOne()
        {
            for (int i = 0; i < rounds.Length; i++)
                if (rounds[i + 1] != null)
                {
                    rounds[i] = rounds[i + 1];
                }
                else
                {
                    rounds[i] = null;
                }
        }
    }
}
