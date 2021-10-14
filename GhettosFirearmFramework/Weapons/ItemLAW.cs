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
    class ItemLAW : MonoBehaviour
    {
        Item item;
        ItemModuleLAW module;

        public void Awake()
        {
            item = base.GetComponent<Item>();
            module = item.data.GetModule<ItemModuleLAW>();

            item.OnHeldActionEvent += Item_OnHeldActionEvent;
        }

        private void Item_OnHeldActionEvent(RagdollHand ragdollHand, Handle handle, Interactable.Action action)
        {
            if (handle == item.transform.Find("MainHandle").GetComponent<Handle>() && action == Interactable.Action.UseStart)
            {
                Fire();
            }
        }

        private void Fire()
        {
            if(item.transform.Find("FireSFX").GetComponent<AudioSource>() != null) item.transform.Find("FireSFX").GetComponent<AudioSource>().Play();
            try { if (item.transform.Find("Animations").GetComponent<Animation>() != null) item.transform.Find("Animations").GetComponent<Animation>().Play("FireAnim"); } catch { }
            try { Catalog.GetData<ItemData>(module.ProjectileID)} catch { }
        }

        private void OnCollisionEnter(Collision hit)
        {
            if (module.AmmoIDs.Contains(hit.collider.transform.GetComponentInParent<Item>().itemId))
            {
                Reload();
            }
        }

        private void Reload()
        { 
        
        }
}
