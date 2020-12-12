using UnityEngine;

namespace com.braineeeedevs.mouseflight.game
{
    public class FollowCam : MonoBehaviour
    {
        public Controller controls;
        public float default_z, height;
        public Camera cam;
        public void Awake()
        {
            QualitySettings.vSyncCount = 1;
            controls = FindObjectOfType<Controller>();
            cam = GetComponent<Camera>();
            transform.localPosition = new Vector3(0f, height, default_z);
        }


        public void FixedUpdate()
        {
            if (controls.slave != null && controls.slave.isActiveAndEnabled)
            {
                Vector3 posTarget = controls.slave.transform.position + controls.slave.transform.rotation * controls.camStartPos;
                transform.position = Vector3.MoveTowards(transform.position, posTarget, Mathf.Max(controls.slave.physics.velocity.magnitude * 0.35f, 1f));
            }
        }

        ///<summary>
        /// Camera controls.
        ///</summary>      

        public void LateUpdate()
        {
            if (controls.slave != null && controls.slave.isActiveAndEnabled)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, controls.slave.transform.rotation, 3f);
            }
        }
    }
}