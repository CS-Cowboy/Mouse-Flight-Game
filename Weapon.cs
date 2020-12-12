using System.Collections;
using UnityEngine;

namespace com.braineeeedevs.mouseflight.game
{
    [RequireComponent(typeof(ObjectPooler))]
    ///<summary>
    /// Basic gun class. Fires object pooled bullets. 
    /// Add a reference to 'WepStats' variable for a 'WeaponStats' object to provide initial values. Automatically fills fields and operates from that WeaponStats object.
    /// Automatically includes an object pooler component to the gameobject.
    ///</summary>
    public class Weapon : MonoBehaviour, IFire 
    {
        protected int bullets, mags;
        protected Coroutine shooterRoutine = null, reloadRoutine = null;
        protected ObjectPooler bulletPool;
        protected StarShip shooter;
        protected bool ready = true;
        public WeaponStats wepStats;
        ///<summary>
        ///Aims this particular gun
        ///</summary>
        public void Aim(Vector3 aimPoint)
        {
            transform.LookAt(aimPoint - transform.position, transform.parent.up);
        }
        ///<summary>
        /// Initiates a reload by calling coroutine: LoadGun()
        ///</summary>
        void IFire.Reload()
        {
            if (reloadRoutine == null)
            {
                reloadRoutine = StartCoroutine(LoadGun());
            }
        }
        ///<summary>
        /// Fires this particular gun once by calling Discharge()
        ///</summary>        
        void IFire.Fire()
        {
            print(bullets);
            if (bullets > 0)
            {
                if (shooterRoutine == null) //If not fired
                    shooterRoutine = StartCoroutine(((IFire)this).Discharge()); //Begin full auto firing and save ref to 'fireCoroutine'
            }
            else
            {
                if (reloadRoutine == null)
                    reloadRoutine = StartCoroutine(LoadGun());
            }
        }
        ///<summary>
        /// Unity method. Initializes the object every time it is created or set active. 
        ///</summary>
        public void OnEnable()
        {
            gameObject.tag = "Weapon";
            gameObject.layer = LayerMask.NameToLayer("Physical");
            if (bulletPool == null)
            {
                bulletPool = gameObject.GetComponent<ObjectPooler>();
            }
            if((shooter = gameObject.GetComponentInParent<StarShip>()) == null)
            {
                Debug.Log(gameObject.name + " is not attached to a ship. Is this your goal?");
            }
            Debug.Assert(wepStats != null, "WepStats variable is null. Did you forget to add a WeaponStats reference to the script?");
            mags = wepStats.magazineCount;
            bullets = wepStats.ammoCapacity;
        }

        ///<summary>
        /// Does the timing for reloading the gun.
        ///</summary>        
        private IEnumerator LoadGun()
        {
            ready = false;
            yield return new WaitForSeconds(wepStats.reloadDelay);

            mags = Mathf.Clamp(mags - 1, 0, wepStats.magazineCount);
            if (mags == 0)
            {
                ready = false;
            }
            else
            {
                bullets = wepStats.ammoCapacity;
                reloadRoutine = null;
                ready = true;
            }
        }
        ///<summary>
        /// Does the timing and firing of a bullet.
        ///</summary>        
        IEnumerator IFire.Discharge()
        {
            if (ready)
            {
                bulletPool.GetObject( shooter, transform);
                bullets = Mathf.Clamp(bullets - 1, 0, wepStats.ammoCapacity);
                yield return new WaitForSeconds(wepStats.fireDelay);
                shooterRoutine = null;
            }
        }

}
}