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
    class ItemSimpleFirearm : MonoBehaviour
    {
        private Item item;
        private Transform IT;
        private ItemModuleSimpleFirearm module;
        private Handle mainHandle;
        private Transform muzzle;

        private float despawnTime;
        private string id;
        private bool limitedAmmo = true;
        private int maxAmmo;
        private int currentAmmo;
        private float rpm;
        private float rps;
        private float lastShotTime = 0;
        private bool cocked = true;

        private string currentFiremode;
        private bool triggerPressed;
        private bool isCountingForlongPress = false;
        private float lastSpellPressTime;
        private float longPressDuration = 0.4f;
        private string[] allowedFiremodes;
        private int fireModeIndex = 0;
        private float force;
        private float recoil;
        private bool waitingForTriggerReset = false;

        private AudioSource fireSFX;
        private AudioSource releaseSFX;
        private AudioSource reloadSFX;
        private AudioSource emptySFX;
        private AudioSource safetySFX;

        private ParticleSystem muzzleFlash;
        private ParticleSystem shellEjection;

        private Animation anim;

        private Transform safetyMesh;
        private Transform safetySafe;
        private Transform safetySemi;
        private Transform safetyAuto;

        public void Awake()
        {
            item = base.GetComponent<Item>();
            IT = item.transform;
            module = item.data.GetModule<ItemModuleSimpleFirearm>();
            maxAmmo = module.AmmoCount;
            currentAmmo = maxAmmo;
            if (maxAmmo == 0) limitedAmmo = false;
            rpm = module.FireRate;
            rps = 60 / rpm;
            id = module.ProjectileID;
            recoil = module.RecoilForce;
            force = module.ProjectileForce;
            despawnTime = module.BulletDespawnTime;
            allowedFiremodes = module.Firemodes;

            mainHandle = IT.Find("MainHandle").GetComponent<Handle>();
            muzzle = IT.Find("Muzzle");
            fireSFX = IT.Find("FireSFX").GetComponent<AudioSource>();
            reloadSFX = IT.Find("ReloadSFX").GetComponent<AudioSource>();
            releaseSFX = IT.Find("ReleaseSFX").GetComponent<AudioSource>();
            safetySFX = IT.Find("SafetySFX").GetComponent<AudioSource>();
            emptySFX = IT.Find("EmptySFX").GetComponent<AudioSource>();
            muzzleFlash = IT.Find("MuzzleFlash").GetComponent<ParticleSystem>();
            shellEjection = IT.Find("ShellEjection").GetComponent<ParticleSystem>();
            anim = IT.Find("Animations").GetComponent<Animation>();

            safetyMesh = IT.Find("GF_Safety");
            safetySafe = IT.Find("GF_Safety_Safe");
            safetySemi = IT.Find("GF_Safety_Semi");
            safetyAuto = IT.Find("GF_Safety_Auto");

            //safety
            currentFiremode = allowedFiremodes[0]; if (safetyMesh != null)
            {
                if (currentFiremode == "Safe") if (safetySafe != null) safetyMesh.rotation = safetySafe.rotation; safetyMesh.position = safetySafe.position;
                if (currentFiremode == "Semi") if (safetySemi != null) safetyMesh.rotation = safetySemi.rotation; safetyMesh.position = safetySemi.position;
                if (currentFiremode == "Auto") if (safetyAuto != null) safetyMesh.rotation = safetyAuto.rotation; safetyMesh.position = safetyAuto.position;
            }

            item.OnHeldActionEvent += Item_OnHeldActionEvent;
        }

        private void Item_OnHeldActionEvent(RagdollHand ragdollHand, Handle handle, Interactable.Action action)
        {
            if (handle == mainHandle && action == Interactable.Action.UseStart)
            {
                triggerPressed = true;
            }
            else if (handle == mainHandle && action == Interactable.Action.UseStop || handle == mainHandle && action == Interactable.Action.Ungrab)
            {
                waitingForTriggerReset = false;
                triggerPressed = false;
            }

            //safety start
            if (handle == mainHandle && action == Interactable.Action.AlternateUseStart)
            {
                isCountingForlongPress = true;
                lastSpellPressTime = Time.time;
            }
            if (handle == mainHandle && action == Interactable.Action.AlternateUseStop)
            {
                if (isCountingForlongPress && Time.time - lastSpellPressTime <= longPressDuration)
                {
                    ShortPress();
                }
                else if (isCountingForlongPress)
                {
                    LongPress();
                }
                isCountingForlongPress = false;
            }
            if (handle == mainHandle && action == Interactable.Action.Ungrab)
            {
                isCountingForlongPress = false;
            }
            //safety end
        }

        private void ShortPress()
        {
            Reload();
        }

        private void LongPress()
        {
            SwitchFiremode();
        }

        public void Update()
        {
            //Debug.Log(Math.Round(Time.time, 3) % rps == 0);

            if (currentFiremode == "Auto" && triggerPressed && IsTimeReadyToFire())
            {
                if (currentAmmo > 0)
                {
                    waitingForTriggerReset = true;
                    lastShotTime = Time.time;
                    Fire();
                }
                //empty
                else if (cocked && !IsTimeReadyToFire())
                {
                    waitingForTriggerReset = true;
                    emptySFX?.Play();
                    cocked = false;
                }
            }
            if (triggerPressed && currentFiremode == "Semi" && IsTimeReadyToFire() && currentAmmo > 0 && !waitingForTriggerReset)
            {
                if (currentAmmo > 0)
                {
                    waitingForTriggerReset = true;
                    Fire();
                }
                //empty
                else if (cocked && !IsTimeReadyToFire())
                {
                    waitingForTriggerReset = true;
                    emptySFX?.Play();
                    cocked = false;
                }
            }
        }

        private void Fire()
        {
            fireSFX?.Play();
            muzzleFlash?.Play();
            shellEjection?.Play();
            currentAmmo--;
            if (currentAmmo == 0 && module.HasBoltCatch)
                    anim?.Play("Empty");
            else
                    anim?.Play("Fire");

            Catalog.GetData<ItemData>(id, true).SpawnAsync(thisSpawnedItem =>
            {
                thisSpawnedItem.transform.rotation = muzzle.rotation;
                thisSpawnedItem.transform.position = muzzle.position;
                thisSpawnedItem.rb.velocity = item.rb.velocity;
                thisSpawnedItem.rb.AddForce(thisSpawnedItem.rb.transform.forward * 1000.0f * force);
                thisSpawnedItem.Throw();
                thisSpawnedItem.Despawn(despawnTime);
            });
            item.rb.AddForce(item.rb.transform.forward * -recoil, ForceMode.Impulse);
        }

        private void Reload()
        {
            if (currentAmmo == 0 && module.HasBoltCatch)
            {
                anim?.Play("Release");
                releaseSFX?.Play();
            }
            else
            {
                anim?.Play("Reload");
                reloadSFX?.Play();
            }
            currentAmmo = maxAmmo;
            cocked = true;
        }

        private bool IsTimeReadyToFire()
        {
            if (Time.time - lastShotTime > rps) return true;
            //if (Math.Round(Time.time, 3) % rps == 0) return true;
            else return false;
        }

        private void SwitchFiremode()
        {
            if (allowedFiremodes.Length > 0)
            {
                safetySFX.Play();
                if (fireModeIndex + 1 < allowedFiremodes.Length)
                {
                    fireModeIndex++;
                    currentFiremode = allowedFiremodes[fireModeIndex];
                }
                else
                {
                    fireModeIndex = 0;
                    currentFiremode = allowedFiremodes[fireModeIndex];
                }
                if (safetyMesh != null)
                {
                    if (currentFiremode == "Safe") if (safetySafe != null) safetyMesh.rotation = safetySafe.rotation; safetyMesh.position = safetySafe.position;
                    if (currentFiremode == "Semi") if (safetySemi != null) safetyMesh.rotation = safetySemi.rotation; safetyMesh.position = safetySemi.position;
                    if (currentFiremode == "Auto") if (safetyAuto != null) safetyMesh.rotation = safetyAuto.rotation; safetyMesh.position = safetyAuto.position;
                }
            }
        }
    }
}
