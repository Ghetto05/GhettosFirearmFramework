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
    class ItemM1911 : MonoBehaviour
    {
        Item item;
        ItemModuleM1911 module;

        //AMMO
        ItemModuleMagazine currentMagazine;

        //STATUS
        bool hammerBack = false;
        bool slideLockedBack = false;
        bool slideLockedFront = true;
        bool triggerPulled = false;
        bool safety = true;

        //AUDIO

        public void Awake()
        {
            item = base.GetComponent<Item>();
            module = item.data.GetModule<ItemModuleM1911>();

            item.OnHeldActionEvent += Item_OnHeldActionEvent;
        }

        private void OnCollisionEnter(Collision hit)
        {
        }

        private void Item_OnHeldActionEvent(RagdollHand ragdollHand, Handle handle, Interactable.Action action)
        {
            if (handle == item.transform.Find("MainHandle").GetComponent<Handle>())
            {
                DoTriggerPull(action);
                DoSafetySwitch(action);
            }
            if (handle == item.transform.Find("SlideHandle").GetComponent<Handle>())
            {
                DoHammerPull(action);
            }
        }

        void Update()
        {
            if (slideLockedFront && hammerBack && triggerPulled && !safety)
            {
                StartCoroutine(Fire());
            }
        }

        public IEnumerator Fire()
        {
            item.transform.Find("Components").Find("Hammer").GetComponent<Animation>().Play("HammerHit");
            hammerBack = false;
            yield return new WaitForSeconds(0.05f);
            item.transform.Find("SFX").Find("HammerHitSFX").GetComponent<AudioSource>().Play();
        }

        //animations and shit
        private void DoTriggerPull(Interactable.Action ac)
        {
            if (ac == Interactable.Action.UseStart)
            { 
                triggerPulled = true;
            }
            else if (ac == Interactable.Action.UseStop)
            { 
                triggerPulled = false;
            }
            if (ac == Interactable.Action.UseStart || ac == Interactable.Action.UseStop)
            {
                if (triggerPulled && !safety)
                {
                    item.transform.Find("Components").Find("Trigger").GetComponent<Animation>().Play("TriggerPull");
                    item.transform.Find("SFX").Find("TriggerPullSFX").GetComponent<AudioSource>().Play();
                }
                else if (!safety)
                {
                    item.transform.Find("Components").Find("Trigger").GetComponent<Animation>().Play("TriggerReset");
                    item.transform.Find("SFX").Find("TriggerResetSFX").GetComponent<AudioSource>().Play();
                }
            }
        }

        private void DoSafetySwitch(Interactable.Action ac)
        {
            if (ac == Interactable.Action.AlternateUseStart)
            {
                if (!safety) safety = true;
                else safety = false;

                if (safety)
                {
                    item.transform.Find("Components").Find("Safety").GetComponent<Animation>().Play("SafetySafe");
                    item.transform.Find("SFX").Find("SafetySFX").GetComponent<AudioSource>().Play();
                }
                else
                {
                    item.transform.Find("Components").Find("Safety").GetComponent<Animation>().Play("SafetySemi");
                    item.transform.Find("SFX").Find("SafetySFX").GetComponent<AudioSource>().Play();
                }
            }
        }

        private void DoHammerPull(Interactable.Action ac)
        {
            if (ac == Interactable.Action.UseStart)
            {
                hammerBack = true;
                item.transform.Find("Components").Find("Hammer").GetComponent<Animation>().Play("HammerGoBack");
            }
        }

        private void UpdateTrigger()
        {
        }
        private void UpdateSafety()
        {
        }
        private void UpdateMagrelease()
        { }
    }
}
