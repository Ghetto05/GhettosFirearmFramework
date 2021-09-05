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
        Handle mainHandle;
        
        Animation deployAnim;
        Animation retractAnim;
        Animation fireAnim;
        Transform muzzle;
        ParticleSystem muzzleFlash;

        AudioSource fireSFX;
        AudioSource retractSFX;
        AudioSource deploySFX;

        string projectileItem;
        string deployAnimName;
        string retractAnimName;
        string fireAnimName;

        private bool deployed = false;
        private bool animating = false;
        private bool usesRocketScript = false;

        private int ammoCap = 1;

        public void GetReferences()
        {
            if (!module.debugMode)
            {
                deployAnim = item.GetCustomReference("deployAnim").GetComponent<Animation>();
                retractAnim = item.GetCustomReference("retractAnim").GetComponent<Animation>();
                fireAnim = item.GetCustomReference("fireAnim").GetComponent<Animation>();
                fireSFX = item.GetCustomReference("fireSFX").GetComponent<AudioSource>();
                deploySFX = item.GetCustomReference("deploySFX").GetComponent<AudioSource>();
                retractSFX = item.GetCustomReference("retractSFX").GetComponent<AudioSource>();
                muzzle = item.GetCustomReference("muzzle");
                muzzleFlash = item.GetCustomReference("muzzleFlash").GetComponent<ParticleSystem>();
                projectileItem = module.ProjectileID;
                deployAnimName = module.deployAnimName;
                retractAnimName = module.retractAnimName;
                fireAnimName = module.fireAnimName;
                usesRocketScript = module.usesRocketScript;
                //fireSFX.enabled = true;
                //deploySFX.enabled = true;
                //retractSFX.enabled = true;
            }
            else
            {
                try
                {
                    deployAnim = item.GetCustomReference("deployAnim").GetComponent<Animation>();
                }
                catch
                { Debug.LogError("LAW: Failed to get reference: deployAnim"); }
                try
                {
                    fireAnim = item.GetCustomReference("fireAnim").GetComponent<Animation>();
                }
                catch
                { Debug.LogError("LAW: Failed to get reference: fireAnim"); }
                try
                {
                    deploySFX = item.GetCustomReference("deploySFX").GetComponent<AudioSource>();
                }
                catch
                { Debug.LogError("LAW: Failed to get reference: deploySFX"); }
                try
                {
                    muzzle = item.GetCustomReference("muzzle");
                }
                catch
                { Debug.LogError("LAW: Failed to get reference: muzzle"); }
                try
                {
                    muzzleFlash = item.GetCustomReference("muzzleFlash").GetComponent<ParticleSystem>();
                }
                catch
                { Debug.LogError("LAW: Failed to get reference: muzzleFlash"); }
                try
                {
                    retractSFX = item.GetCustomReference("retractSFX").GetComponent<AudioSource>();
                }
                catch
                { Debug.LogError("LAW: Failed to get reference: retractSFX"); }
                try
                {
                    fireSFX = item.GetCustomReference("fireSFX").GetComponent<AudioSource>();
                }
                catch
                { Debug.LogError("LAW: Failed to get reference: fireSFX"); }
                try
                {
                    retractAnim = item.GetCustomReference("retractAnim").GetComponent<Animation>();
                }
                catch
                { Debug.LogError("LAW: Failed to get reference: retractAnim"); }
            }
        }

        public void Awake()
        {
            item = base.GetComponent<Item>();
            module = item.data.GetModule<ItemModuleLAW>();
            mainHandle = item.GetCustomReference("MainHandle").GetComponent<Handle>();
            GetReferences();

            item.OnHeldActionEvent += Item_OnHeldActionEvent;
        }

        private void Item_OnHeldActionEvent(RagdollHand ragdollHand, Handle interactionHandle, Interactable.Action action)
        {
            if (interactionHandle == mainHandle && action == Interactable.Action.UseStart && deployed && !animating && ammoCap > 0)
            {
                Fire();
            }
            if (interactionHandle == mainHandle && action == Interactable.Action.AlternateUseStart && !deployed && !animating)
            {
                StartCoroutine(Deploy());
            }
            if (interactionHandle == mainHandle && action == Interactable.Action.AlternateUseStart && deployed && !animating)
            {
                StartCoroutine(Retract());
            }
        }

        private IEnumerator Deploy()
        {
            animating = true;
            deployAnim.Play(deployAnimName);
            //deploySFX.Play();
            yield return new WaitForSeconds(deployAnim.clip.length);
            deployed = true;
            animating = false;
        }

        private IEnumerator Retract()
        {
            animating = true;
            retractAnim.Play(retractAnimName);
            //retractSFX.Play();
            yield return new WaitForSeconds(retractAnim.clip.length);
            deployed = false;
            animating = false;
        }

        private void Fire()
        {
            ammoCap--;
            fireAnim.Play(fireAnimName);
            muzzleFlash.Play();
            //fireSFX.Play();
            Catalog.GetData<ItemData>(projectileItem).SpawnAsync(i => 
            { 
                i.transform.rotation = muzzle.rotation; 
                i.transform.position = muzzle.position; 
                if (usesRocketScript) 
                { 
                    i.GetComponent<ItemRocket>().Fire(); 
                } 
            });
        }
    }
}
