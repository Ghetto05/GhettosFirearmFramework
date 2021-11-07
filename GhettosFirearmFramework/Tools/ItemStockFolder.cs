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
    class ItemStockFolder : MonoBehaviour
    {
        Item item;
        ItemModuleStockFolder module;
        Handle animHandle;
        Animation anim;
        bool extended = true;
        AudioSource foldSFX;
        AudioSource unfoldSFX;

        public void Awake()
        {
            item = base.GetComponent<Item>();
            module = item.data.GetModule<ItemModuleStockFolder>();
            anim = item.transform.Find(module.AnimatorComponentReference).GetComponent<Animation>();
            animHandle = item.transform.Find(module.HandleName).GetComponent<Handle>();
            unfoldSFX = item.transform.Find("UnfoldSFX").GetComponent<AudioSource>();
            foldSFX = item.transform.Find("FoldSFX").GetComponent<AudioSource>();

            item.OnHeldActionEvent += Item_OnHeldActionEvent;
        }

        private void Item_OnHeldActionEvent(RagdollHand ragdollHand, Handle handle, Interactable.Action action)
        {
            if (module.UsesTrigger && handle == animHandle && action == Interactable.Action.UseStart) Toggle();
            if (module.UsesAlternateUse && handle == animHandle && action == Interactable.Action.AlternateUseStart) Toggle();
        }

        private void Toggle()
        {
            if (extended)
            {
                extended = false;
                anim.Play("Fold");
                foldSFX.Play();
            }
            else
            {
                unfoldSFX.Play();
                extended = true;
                anim.Play("Unfold");
            }
        }
    }
}
