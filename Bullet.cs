using System.Collections;
using UnityEngine;

namespace com.braineeeedevs.mouseflight.game
{
    [RequireComponent(typeof(Rigidbody))]

    ///<summary>
    /// A single physical bullet. Dies after 'stats.lifespan' seconds.
    ///</summary>      
    public class Bullet : MonoBehaviour, ITranslate, IDamage, ISpawn
    {
        protected float hp;
        public Stats stats;
        protected Rigidbody physics;
        protected ObjectPooler origin;
        protected Transform target;
        protected StarShip shooter;
        public Vector3 acceleration, prevVelocity;
        public AnimationCurve damageForceGraph;

        protected void OnEnable()
        {
            physics = GetComponent<Rigidbody>();
            hp = stats.initialHealth;
            physics.mass = stats.mass;
            physics.angularDrag = physics.drag = stats.drag;
            physics.useGravity = false;
            physics.collisionDetectionMode = CollisionDetectionMode.Discrete;
            physics.interpolation = RigidbodyInterpolation.None;
            gameObject.tag = "Bullet";
            gameObject.layer = LayerMask.NameToLayer("Bullet");
            damageForceGraph.AddKey(0f, 2f);
            damageForceGraph.AddKey(Mathf.Sqrt((stats.moveForce) / physics.drag) / Time.fixedDeltaTime, 100f);
        }

        protected void OnDisable()
        {
            physics.velocity = physics.angularVelocity = Vector3.zero;
        }
        ///<summary>
        /// Handles physics. If it has a target transform it will seek that.
        ///</summary>      
        protected void FixedUpdate()
        {
            if (target != null) //Issa missile.
            {
                ((ITranslate)this).PhysicsMove(Vector3.zero);
                ((ITranslate)this).Steer(Vector3.zero);
            }
            acceleration = physics.velocity - prevVelocity;
            prevVelocity = physics.velocity;
        }

        ///<summary>
        /// Sets up the bullet for flight.
        ///</summary>      
        void ISpawn.Init(Transform start, ObjectPooler src, StarShip backRef)
        {
            origin = src;
            shooter = backRef;
            physics.position = transform.position = start.position;
            physics.transform.forward = transform.forward = start.forward;
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(((IDamage)this).Die(true));
            }
            if (target == null) //Just a bullet.
            {
                ((ITranslate)this).PhysicsMove(shooter.physics.velocity);
            }
        }
        ///<summary>
        /// Causes damage to the bullet.
        ///</summary>      
        void IDamage.DoDamage(float othersRelativeSpeed)
        {
            StartCoroutine(((IDamage)this).Die(false));
        }
        ///<summary>
        /// Computes and delivers realized damage to the other object before deleting this bullet.
        ///</summary>      
        public void OnCollisionEnter(Collision other)
        {
            IDamage otherObj;
            if (other != null && other.gameObject.tag == "Ship" && other.gameObject != shooter.gameObject && (otherObj = other.gameObject.GetComponent<IDamage>()) != null)
            {
                float computedDamage = damageForceGraph.Evaluate((acceleration.magnitude * stats.mass + acceleration.magnitude * stats.moveForce) * 0.5f);
                otherObj.DoDamage(computedDamage);
            }
            this.gameObject.SetActive(false);

        }
        ///<summary>
        /// Kills the bullet. Optional 'wait' paramter ensures the bullet waits 'stats.lifespan' seconds before dying.
        ///</summary>   
        IEnumerator IDamage.Die(bool wait)
        {
            Debug.Assert(origin != null, "Origin is null.");
            if (wait)
            {
                yield return new WaitForSeconds(stats.lifespan);
            }
            gameObject.SetActive(false);
            origin.PutBack(gameObject);
        }
        ///<summary>
        ///  Moves the bullet.
        ///</summary>  
        void ITranslate.PhysicsMove(Vector3 moveDelta)
        {
            Debug.Assert(physics != null, "Physics is null.");
            physics.AddForce(moveDelta + transform.rotation * Vector3.forward * stats.moveForce, ForceMode.VelocityChange);
        }

        ///<summary>
        ///  Steers the bullet.
        ///</summary>  
        void ITranslate.Steer(Vector3 steering)
        {
            //Src is target transform here.
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(this.target.position - transform.position, transform.up), stats.turnForce * Time.fixedDeltaTime);
        }
    }
}