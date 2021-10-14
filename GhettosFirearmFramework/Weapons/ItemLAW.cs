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
        bool deployed = false;
        bool animating = false;
        bool loaded = true;

        public void Awake()
        {
            item = base.GetComponent<Item>();
            module = item.data.GetModule<ItemModuleLAW>();

            item.OnHeldActionEvent += Item_OnHeldActionEvent;
        }

        private void Item_OnHeldActionEvent(RagdollHand ragdollHand, Handle handle, Interactable.Action action)
        {
            if (handle == item.transform.Find("MainHandle").GetComponent<Handle>() && action == Interactable.Action.UseStart && deployed && !animating && loaded)
            {
                Fire();
            }
            if (action == Interactable.Action.AlternateUseStart && !animating)
            {
                StartCoroutine(DeployOrRetract());
            }
        }

        private void Fire()
        {
            loaded = false;
            item.transform.Find("RoundInWeaponMesh").gameObject.SetActive(false);
            if (item.transform.Find("FireSFX").GetComponent<AudioSource>() != null) item.transform.Find("FireSFX").GetComponent<AudioSource>().Play();
            try { if (item.transform.Find("Animations").GetComponent<Animation>() != null) item.transform.Find("Animations").GetComponent<Animation>().Play("FireAnim"); } catch { }
            try
            {
                Catalog.GetData<ItemData>(module.ProjectileID).SpawnAsync(Item =>
          {
                //item.GetComponent<ItemBatonFragment>().SetItem1(item.transform); 
            }, item.transform.Find("").position, item.transform.Find("").rotation, null);
            }
            catch { }
        }

        public IEnumerator DeployOrRetract()
        {
            if (deployed)
            {
                animating = true;
                try
                {
                    if (item.transform.Find("Animations").GetComponent<Animation>() != null)
                    {
                        item.transform.Find("Animations").GetComponent<Animation>().Play("RetractAnim");
                    }
                }
                catch { }
                yield return new WaitForSeconds(1.0f);
                animating = false;
                deployed = false;
            }
            else
            {
                animating = true;
                try
                {
                    if (item.transform.Find("Animations").GetComponent<Animation>() != null)
                    {
                        item.transform.Find("Animations").GetComponent<Animation>().Play("DeployAnim");
                    }
                }
                catch { }
                yield return new WaitForSeconds(1.0f);
                animating = false;
                deployed = true;
            }
        }

        private void OnCollisionEnter(Collision hit)
        {
            if (hit.collider.transform.name == "ReloadingCollider" && module.AmmoIDs.Contains(hit.collider.transform.GetComponentInParent<Item>().itemId) && !animating)
            {
                Reload(hit);
            }
        }

        private void Reload(Collision hitpoint)
        {
            hitpoint.collider.transform.GetComponentInParent<Item>().Despawn();
            item.transform.Find("RoundInWeaponMesh").gameObject.SetActive(true);
            loaded = true;
        }
    }
}
