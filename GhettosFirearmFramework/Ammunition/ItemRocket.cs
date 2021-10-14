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
    class ItemRocket : MonoBehaviour
    {
        Item item;
        ItemModuleRocket module;

        float thrust;
        bool boosterActive = false;

        public void Awake()
        {
            item = base.GetComponent<Item>();
            module = item.data.GetModule<ItemModuleRocket>();
            thrust = module.ThrusterForce;

            item.OnHeldActionEvent += Item_OnHeldActionEvent;
        }

        private void Item_OnHeldActionEvent(RagdollHand ragdollHand, Handle interactionHandle, Interactable.Action action)
        {
            if (action == Interactable.Action.UseStart)
            {
                Fire();
            }
        }

        public void Fire()
        {
            boosterActive = true;
            item.transform.Find("FireSound");
        }

        public void Update()
        {
            if (boosterActive)
            {
                item.rb.angularDrag = 0;
                item.rb.AddRelativeForce(Vector3.up * thrust);
            }
        }
    }
}
