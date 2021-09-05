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
        Handle mainHandle;

        AudioSource flightLoop;

        bool hasSound = false;
        bool boosterActive = false;
        float thrust;

        public void Awake()
        {
            item = base.GetComponent<Item>();
            module = item.data.GetModule<ItemModuleRocket>();
            hasSound = module.hasSound;
            if (hasSound) { flightLoop = item.GetCustomReference("flightSound").GetComponent<AudioSource>(); }
            thrust = module.thrust;

            item.OnHeldActionEvent += Item_OnHeldActionEvent;
        }

        private void Item_OnHeldActionEvent(RagdollHand ragdollHand, Handle interactionHandle, Interactable.Action action)
        {
            if (action == Interactable.Action.UseStart)
            {
                if (hasSound)
                {
                    flightLoop.Play();
                }
                Fire();
            }
        }

        public void Fire()
        {
            boosterActive = true;
        }

        public void Update()
        {
            if (boosterActive)
            {
                item.rb.AddRelativeForce(Vector3.up * thrust);
            }
        }
    }
}
