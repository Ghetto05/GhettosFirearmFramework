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

        //Transforms
        Transform muzzle;

        //SFX
        AudioSource fireSFX;
        bool hasFireSFX;
        AudioSource retractSFX;
        bool hasRetractSFX;
        AudioSource deploySFX;
        bool hasDeploySFX;

        //VFX
        ParticleSystem muzzleFlash;
        bool hasMuzzleFlash;

        //Animations
        Animation deployAnim;
        bool hasDeployAnim;
        Animation retractAnim;
        bool hasRetractAnim;
        Animation fireAnim;
        bool hasFireAnim;

        string deployAnimName;
        string retractAnimName;
        string fireAnimName;

        //Data
        string projectileItem;
        private bool usesRocketScript = false;

        //func vars
        private int ammoCap = 1;
        private bool deployed = false;
        private bool animating = false;

        public void GetReferences()
        {
            //Transforms
            if (item.GetCustomReference("mainHandle").GetComponent<Handle>() == null)
            {
                if (module.debugMode)
                {
                    Debug.LogError(item.data.id + ": failed to get reference: mainHandle");
                }
            }
            else
            {
                mainHandle = item.GetCustomReference("mainHandle").GetComponent<Handle>();
            }

            if (item.GetCustomReference("muzzle") == null)
            {
                if (module.debugMode)
                {
                    Debug.LogError(item.data.id + ": failed to get reference: muzzle");
                }
            }
            else
            {
                muzzle = item.GetCustomReference("muzzle");
            }

            //SFX
            if (item.GetCustomReference("fireSFX").GetComponent<AudioSource>() == null)
            {
                if (module.debugMode)
                {
                    Debug.LogError(item.data.id + ": failed to get reference: fireSFX");
                }
                hasFireSFX = false;
            }
            else
            {
                fireSFX = item.GetCustomReference("fireSFX").GetComponent<AudioSource>();
                hasFireSFX = true;
            }

            if (item.GetCustomReference("retractSFX").GetComponent<AudioSource>() == null)
            {
                if (module.debugMode)
                {
                    Debug.LogError(item.data.id + ": failed to get reference: retractSFX");
                }
                hasRetractSFX = false;
            }
            else
            {
                fireSFX = item.GetCustomReference("retractSFX").GetComponent<AudioSource>();
                hasRetractSFX = true;
            }

            if (item.GetCustomReference("deploySFX").GetComponent<AudioSource>() == null)
            {
                if (module.debugMode)
                {
                    Debug.LogError(item.data.id + ": failed to get reference: deploySFX");
                }
                hasDeploySFX = false;
            }
            else
            {
                deploySFX = item.GetCustomReference("fireSFX").GetComponent<AudioSource>();
                hasDeploySFX = true;
            }

            //VFX
            if (item.GetCustomReference("muzzleFlash").GetComponent<ParticleSystem>() == null)
            {
                if (module.debugMode)
                {
                    Debug.LogError(item.data.id + ": failed to get reference: muzzleFlash");
                }
                hasMuzzleFlash = false;
            }
            else
            {
                muzzleFlash = item.GetCustomReference("muzzleFlash").GetComponent<ParticleSystem>();
                hasMuzzleFlash = true;
            }

            //Animations
            if (item.GetCustomReference("deployAnim").GetComponent<Animation>() == null)
            {
                if (module.debugMode)
                {
                    Debug.LogError(item.data.id + ": failed to get reference: deployAnim");
                }
                hasDeployAnim = false;
            }
            else
            {
                deployAnim = item.GetCustomReference("deployAnim").GetComponent<Animation>();
                hasDeployAnim = true;
            }

            if (item.GetCustomReference("retractAnim").GetComponent<Animation>() == null)
            {
                if (module.debugMode)
                {
                    Debug.LogError(item.data.id + ": failed to get reference: retractAnim");
                }
                hasRetractAnim = false;
            }
            else
            {
                retractAnim = item.GetCustomReference("retractAnim").GetComponent<Animation>();
                hasRetractAnim = true;
            }

            if (item.GetCustomReference("fireAnim").GetComponent<Animation>() == null)
            {
                if (module.debugMode)
                {
                    Debug.LogError(item.data.id + ": failed to get reference: fireAnim");
                }
                hasFireAnim = false;
            }
            else
            {
                fireAnim = item.GetCustomReference("fireAnim").GetComponent<Animation>();
                hasFireAnim = true;
            }

            //Animation names
            if (module.fireAnimName == null)
            {
                if (module.debugMode)
                {
                    Debug.LogError(item.data.id + ": failed to get reference: fireAnimName");
                }
                hasFireAnim = false;
            }
            else if (hasFireAnim)
            {
                fireAnimName = module.fireAnimName;
            }

            if (module.retractAnimName == null)
            {
                if (module.debugMode)
                {
                    Debug.LogError(item.data.id + ": failed to get reference: retractAnimName");
                }
                hasRetractAnim = false;
            }
            else if (hasRetractAnim)
            {
                retractAnimName = module.retractAnimName;
            }

            if (module.deployAnimName == null)
            {
                if (module.debugMode)
                {
                    Debug.LogError(item.data.id + ": failed to get reference: deployAnimName");
                }
                hasFireAnim = false;
            }
            else if (hasDeployAnim)
            {
                deployAnimName = module.deployAnimName;
            }

            //Data
            if (module.ProjectileID == null)
            {
                if (module.debugMode)
                {
                    Debug.LogError(item.data.id + ": failed to get reference: projectileItem");
                }
                hasFireAnim = false;
            }
            else
            {
                projectileItem = module.ProjectileID;
            }
        }

        public void Awake()
        {
            GetReferences();

            item.OnHeldActionEvent += Item_OnHeldActionEvent;
        }

        private void Item_OnHeldActionEvent(RagdollHand ragdollHand, Handle interactionHandle, Interactable.Action action)
        {
            if (interactionHandle == mainHandle && action == Interactable.Action.UseStart && deployed && !animating && ammoCap > 0)
            {
                Fire();
            }
            if (interactionHandle == mainHandle && action == Interactable.Action.AlternateUseStart && !animating)
            {
                if (!deployed)
                { StartCoroutine(Deploy()); }
                else
                { StartCoroutine(Retract()); }
            }
        }

        private IEnumerator Deploy()
        {
            if (hasDeploySFX)
            {
                deploySFX.Play();
            }
            if (hasDeployAnim)
            {
                animating = true;
                deployAnim.Play(deployAnimName);
                yield return new WaitForSeconds(deployAnim.clip.length);
                animating = false;
            }
            deployed = true;
        }

        private IEnumerator Retract()
        {
            if (hasRetractSFX)
            {
                retractSFX.Play();
            }
            if (hasRetractAnim)
            {
                animating = true;
                retractAnim.Play(retractAnimName);
                yield return new WaitForSeconds(retractAnim.clip.length);
                animating = false;
            }
            deployed = false;
        }

        private void Fire()
        {
            Catalog.GetData<ItemData>(projectileItem).SpawnAsync(i =>
            {
                i.transform.rotation = muzzle.rotation;
                i.transform.position = muzzle.position;
                if (usesRocketScript)
                {
                    i.GetComponent<ItemRocket>().Fire();
                }
            });
            ammoCap--;

            if (hasFireSFX)
            {
                fireSFX.Play();
            }
            if (hasFireAnim)
            {
                fireAnim.Play(fireAnimName);
            }
            if (hasMuzzleFlash)
            {
                muzzleFlash.Play();
            }
        }
    }
}
