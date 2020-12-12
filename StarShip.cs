﻿using System.Collections;
using UnityEngine;
using System;

namespace com.braineeeedevs.mouseflight.game
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(ObjectPooler))]
    [RequireComponent(typeof(ObjectPooler))]
    ///<summary>
    /// Basic ship class. Provides all the relevant features for a working fighter ship.
    /// Controlled by a 'Controller' component
    ///</summary>        
    public class StarShip : MonoBehaviour, IFire, IDamage, IManeuver, IDefense, IEffects, IHeal, ISpawn, ITranslate
    {
        protected float hp = 100f;
        protected Coroutine deathRoutine, recoverHealthDelayRoutine, noDamageRoutine;
        protected Weapon[] weapons = new Weapon[2];
        public Vector3 acceleration, prevVelocity;
        protected int selectionIndex = 0;
        public ShipStats stats;
        public Rigidbody physics;
        public ObjectPooler origin;
        protected void FixedUpdate()
        {
            acceleration = physics.velocity - prevVelocity;
            prevVelocity = physics.velocity;
        }

        ///<summary>
        /// Fires when impacting a gameobject with a collider component attached.
        /// Applies realistic damage based on force applied.
        ///</summary>        
        public void OnCollisionEnter(Collision other)
        {
            Bullet bull;
            if ((bull = other.gameObject.GetComponent<Bullet>()) != null)
            {
                ((IDamage)this).DoDamage(bull.acceleration.magnitude * bull.stats.moveForce);
                //StartNoDamageAwait();
            }
            Debug.Log("Bump-!");
        }
        public void OnEnable()
        {
            gameObject.layer = LayerMask.NameToLayer("Physical");
            gameObject.tag = "Ship";
            if (origin == null)
            {
                origin = gameObject.GetComponent<ObjectPooler>();
                origin.prefab = this.gameObject;
                origin.max = 3;
            }
            int c = 0;
            foreach (Weapon wep in transform.GetComponentsInChildren<Weapon>())
            {
                weapons[c] = wep;
            }
            if (physics == null)
            {
                physics = gameObject.GetComponent<Rigidbody>();
                physics.mass = stats.mass;
                physics.angularDrag = physics.drag = stats.drag;
                physics.useGravity = false;
                physics.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                physics.interpolation = RigidbodyInterpolation.None;
            }
            Debug.Assert(stats != null, "Stats is null. Did you forget to add a ShipStats reference?");
            hp = stats.initialHealth;
        }
        ///<summary>
        /// Swaps  your primary weapon.
        ///</summary>        
        public void Swap()
        {
            if (weapons.Length > 0)
            {
                int modIndex = selectionIndex + 1 % weapons.Length;
                if (weapons[modIndex] != null)
                {
                    selectionIndex = modIndex;
                }
            }
        }
         ///<summary>
        /// Causes the ship to be killed. Given parameter "true" it waits 'stats.lifespan' seconds to be killed.
        ///</summary>        
        IEnumerator IDamage.Die(bool wait)
        {
            if (wait)
            {
                yield return new WaitForSeconds(stats.lifespan);
            }
            gameObject.SetActive(false);
        }
         ///<summary>
        /// Applies damage to this ship based upon the force given to it.
        ///</summary>        
        void IDamage.DoDamage(float dmgForce)
        {
            hp = Mathf.Clamp(hp - dmgForce, 0f, stats.initialHealth);
            if (hp == 0f)
            {
                StartCoroutine(((IDamage)this).Die(false));
            }
        }
         ///<summary>
        /// Steers the ship given a torque value.
        ///</summary>        
        void ITranslate.Steer(Vector3 steeringTorque)
        {
            physics.AddForceAtPosition(Vector3.ProjectOnPlane(steeringTorque, transform.forward) * stats.turnForce,  transform.forward * 0.05f, ForceMode.Acceleration);
        }
        ///<summary>
        /// Moves the ship with given change in movement parameter 'moveDelta'
        ///</summary>        
        void ITranslate.PhysicsMove(Vector3 moveDelta)
        {
            print("Strafe:" + stats.strafeForce + "moveDelta:" + moveDelta);
            physics.AddRelativeForce(Vector3.right * moveDelta.x * stats.strafeForce + Vector3.forward * moveDelta.y * stats.moveForce, ForceMode.Force);
        }
        ///<summary>
        /// Irrelevant extraneous function. Don't use this, it literally does nothing.
        ///</summary>        
        IEnumerator IFire.Discharge()
        {
            throw new NotImplementedException("Not implemented for a reason. Do not call this with StarShip, it does nothing.");
        }
        ///<summary>
        /// Fires the current weapon.
        ///</summary>                
        void IFire.Fire()
        {
            Debug.Assert(weapons != null, "Weapons variable is null. Did you forget to attach guns?");
            Debug.Assert(weapons[selectionIndex] != null, "Indexed weapon variable is null. Have you removed a gun?");
            ((IFire)this.weapons[selectionIndex]).Fire();
        }
        ///<summary>
        /// Reloads the current gun.
        ///</summary>        
        void IFire.Reload()
        {
            //Do reload.
           
            Debug.Assert(weapons != null, "Weapons variable is null. Did you forget to attach guns?");
            Debug.Assert(weapons[selectionIndex] != null, "Indexed weapon variable is null. Have you removed a gun?");
            ((IFire)this.weapons[selectionIndex]).Reload();
        }


        ///<summary>
        /// Aims the current gun at the parameter 'target' in world space.
        ///</summary>        
        void IFire.Aim(Vector3 target)
        {
            //Do aim stuff.
            Debug.Assert(weapons != null, "Weapons variable is null");
            Debug.Assert(weapons[selectionIndex] != null, "Indexed weapon variable is null");
            weapons[selectionIndex].Aim(target + physics.velocity);
        }

        ///<summary>
        /// Recovers health points over time.
        ///</summary>        
        IEnumerator IHeal.RecoverHP()
        {
            WaitForSeconds awaitDT = new WaitForSeconds(stats.chargeDT);
            while (noDamageRoutine == null && hp != stats.initialHealth)
            {
                hp = Mathf.Clamp(hp + Mathf.Abs(stats.initialHealth * stats.chargeDT), 0f, Mathf.Abs(stats.initialHealth));
                if (hp == stats.initialHealth) //If done.
                {
                    StopCoroutine(recoverHealthDelayRoutine); //Stop.
                }
                yield return awaitDT; //Otherwise continue recovery process.
            }
        }
        
        ///<summary>
        /// Restarts the health recovery process.
        ///</summary>        
        protected void StartNoDamageAwait()
        {
            if (noDamageRoutine == null)
                noDamageRoutine = StartCoroutine(AwaitNoDamage());
        }
        ///<summary>
        /// Starts the actual recovery process if a 'stats.rechargeDelay' seconds have passed with no damage.
        ///</summary>        
        public IEnumerator AwaitNoDamage()
        {
            yield return new WaitForSeconds(stats.rechargeDelay);
            noDamageRoutine = null;
            recoverHealthDelayRoutine = StartCoroutine((this as IHeal).RecoverHP());
        }
        ///<summary>
        /// Last function called before the ship is recycled or destroyed in memory.  Handles cleanup and starts the recycling process.
        ///</summary>      
        protected void OnDisable()
        {
           // SpawnManager karen = GameObject.FindObjectOfType<SpawnManager>();
          //  karen.StartRespawn(this);
            origin.PutBack(gameObject);
        }
        
        ///<summary>
        /// Not implemented.
        ///</summary>      
        public void DoManeuever()
        {
            throw new NotImplementedException("Stub. To be completed later.");
        }

        ///<summary>
        /// Not implemented.
        ///</summary>      
        public void PlayFx(AudioSource[] source)
        {
            throw new NotImplementedException("Stub. To be completed later.");
        }

        ///<summary>
        /// Not implemented.
        ///</summary>      
        void IDefense.UseCountermeasure()
        {
            throw new NotImplementedException("Stub. To be completed later.");
        }
        ///<summary>
        /// Initializes the ship after being recyled.
        ///</summary>      

        void ISpawn.Init(Transform start, ObjectPooler src, StarShip shooter)
        {
            physics.position = transform.position = start.position;
            physics.rotation = transform.rotation = start.rotation;
            origin = src;
        }
    }

}