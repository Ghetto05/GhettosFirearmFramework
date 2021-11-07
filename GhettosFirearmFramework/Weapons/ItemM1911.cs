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

        //COMPONENTS
        Rigidbody slideRB;
        Transform slide;

        public void Awake()
        {
            item = base.GetComponent<Item>();
            module = item.data.GetModule<ItemModuleM1911>();

            slide = item.transform.Find("Components").Find("SlideRoot").Find("Slide");
            Debug.Log(slide.name);

            if (module.GenerateSlideJoint)
            {
                InitializeConfigurableJoint();
            }
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
            slideRB.AddForce(Vector3.back * module.BlowBackForce);
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

        private void InitializeConfigurableJoint()
        {
            slideRB = slide.GetComponent<Rigidbody>();
            if (slideRB == null)
            {
            }
            else { Debug.Log("[Fisher-ModularFirearms][Config-Joint-Init] ACCESSED Rigidbody on Slide Object..."); }

            slideRB.mass = 1.0f;
            slideRB.drag = 0.0f;
            slideRB.angularDrag = 0.0f;
            slideRB.useGravity = true;
            slideRB.isKinematic = false;
            slideRB.interpolation = RigidbodyInterpolation.None;
            slideRB.collisionDetectionMode = CollisionDetectionMode.Discrete;

            Debug.Log("[Fisher-ModularFirearms][Config-Joint-Init] Creating Config Joint and Setting Joint Values...");
            ConfigurableJoint connectedJoint = item.gameObject.AddComponent<ConfigurableJoint>();
            connectedJoint.connectedBody = slideRB;
            connectedJoint.anchor = new Vector3(0, 0, 0.5f * module.SlideTravelDistance);
            connectedJoint.axis = Vector3.right;
            connectedJoint.autoConfigureConnectedAnchor = false;
            connectedJoint.connectedAnchor = Vector3.zero; //new Vector3(0.04f, -0.1f, -0.22f);
            connectedJoint.secondaryAxis = Vector3.up;
            connectedJoint.xMotion = ConfigurableJointMotion.Locked;
            connectedJoint.yMotion = ConfigurableJointMotion.Locked;
            connectedJoint.zMotion = ConfigurableJointMotion.Limited;
            connectedJoint.angularXMotion = ConfigurableJointMotion.Locked;
            connectedJoint.angularYMotion = ConfigurableJointMotion.Locked;
            connectedJoint.angularZMotion = ConfigurableJointMotion.Locked;
            connectedJoint.linearLimit = new SoftJointLimit { limit = 0.5f * module.SlideTravelDistance, bounciness = 0.0f, contactDistance = 0.0f };
            connectedJoint.massScale = 1.0f;
            connectedJoint.connectedMassScale = 1.0f;  //module.slideMassOffset;
            Debug.Log("[Fisher-ModularFirearms][Config-Joint-Init] Created Configurable Joint !");
            //DumpRigidbodyToLog(slideRB);
        }
    }
}
