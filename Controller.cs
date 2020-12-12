using System.Collections;
using UnityEditorInternal.VersionControl;
using UnityEngine;

namespace com.braineeeedevs.mouseflight.game
{
    public class Controller : MonoBehaviour
    {
        public StarShip slave;
        private Camera cam;
        public float gunLookDT = 1.5f, radialAffect = 0.6f, sightRange = 950f, boostMultiplier = 2f;
        public bool flightEnabled = false;
        protected float affectLimit, mouseAngle;
        protected Vector3 inputPosition, worldTarget, movementVec;
        public Vector3 screenCenter, mouseProjection, relativeMousePos, camStartPos, prevTarget, steeringTorque;
        protected Coroutine cameraResetDelay;
        protected PidController.PidController vertController, horiController;

        public void Start()
        {
            screenCenter = new Vector2(Screen.width, Screen.height) * 0.5f;
            affectLimit = screenCenter.magnitude * radialAffect;
            cam = FindObjectOfType<Camera>();
            slave = FindObjectOfType<StarShip>();
            camStartPos = new Vector3(0f, 6, -16);
            vertController = new PidController.PidController(.7f, .25f, 0.75f, slave.stats.turnForce, 0f);
            horiController = new PidController.PidController(.7f, .25f, 0.75f, slave.stats.turnForce, 0f);
        }

        ///<summary>
        /// Gets/updates inputs.
        ///</summary>         
        public void Update()
        {
            if (slave != null && slave.gameObject.activeInHierarchy)
            {
                GetMouseInput();
                if (Input.GetAxisRaw("MouseFlight") > 0f)
                    ToggleFlight();

                float rollInput = Input.GetAxisRaw("Roll");
                if (flightEnabled)
                {
                    relativeMousePos = mouseProjection.magnitude < affectLimit ? mouseProjection : mouseProjection.normalized * affectLimit;
                    relativeMousePos = relativeMousePos.magnitude * new Vector3( -Mathf.Sin(mouseAngle), Mathf.Cos(mouseAngle), rollInput);
                }
                else
                {
                    relativeMousePos = Vector3.forward * rollInput;
                }

                if (Input.GetAxisRaw("Reload") > 0f)
                    (slave as IFire).Reload();
                if (Input.GetAxisRaw("SwapWeapons") > 0f)
                    slave.Swap();
                    
                if(flightEnabled)
                {
                    ((ITranslate)slave).Steer(relativeMousePos * 1f / screenCenter.magnitude);
                }
                ((IFire)slave).Aim(Vector3.MoveTowards(prevTarget, worldTarget, gunLookDT * Time.smoothDeltaTime));
                prevTarget = worldTarget;
            }
            else
            {
                ResetCamera(GameObject.Find("CameraWaitingArea").transform);
            }
        }

        ///<summary>
        /// Returns camera to respawn position and orientation.
        ///</summary>     
        public void ResetCamera(Transform tar)
        {
            if (cameraResetDelay == null)
            {
                cameraResetDelay = StartCoroutine(Reset(tar));
            }
        }

        ///<summary>
        /// Handles timing and control of camera while player is not alive.
        ///</summary>     
        protected IEnumerator Reset(Transform target)
        {
            float cycleCounter = 0f;
            while (Time.smoothDeltaTime * 2.5f >= cycleCounter)
            {
                yield return new WaitForSeconds(Time.smoothDeltaTime);
                cycleCounter += Time.smoothDeltaTime;
                transform.position = Vector3.MoveTowards(transform.position, target.position + target.rotation * camStartPos, 10f);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, target.rotation, 2f);
            }

            cameraResetDelay = null;
        }

        ///<summary>
        /// For things that require physics updates.
        ///</summary>     
        public void FixedUpdate()
        {
            movementVec = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f);

            if (Input.GetAxisRaw("Boost") > 0f)
            {
                movementVec.y = boostMultiplier;
            }
            if (slave != null)
            {
                (slave as ITranslate).PhysicsMove(movementVec);
            }
        }
        ///<summar>
        /// Toggles mouse flight controls.protected
        ///</summary>     
        public void ToggleFlight()
        {
            flightEnabled = !flightEnabled;
        }

        ///<summar>
        /// Gets the relevant mouse control inputs.
        ///</summary>     
        protected void GetMouseInput()
        {
            inputPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            mouseProjection = inputPosition - screenCenter;
            mouseAngle = Vector2.SignedAngle(mouseProjection, Vector2.right) * Mathf.Deg2Rad;

            Ray ray = cam.ScreenPointToRay(inputPosition);
            RaycastHit hit;
            // print(ray.direction);
            worldTarget = Physics.Raycast(transform.position, ray.direction, out hit, sightRange, (LayerMask.NameToLayer("Default"))) ? hit.point : ray.GetPoint(sightRange);
        }

    }
}