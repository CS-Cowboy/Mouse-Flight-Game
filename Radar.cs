using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.braineeeedevs.mouseflight.game
{
    public class Radar : MonoBehaviour
    {
        public List<StarShip> bubbleTargets = new List<StarShip>();
        public Dictionary<string, Image> targetBoxes = new Dictionary<string, Image>();
        public Image boxPrefab;
        public Vector2 playerFov;
        public Camera playerCam;
        public Controller controlRef;
        public Canvas playerCanvas;
        void Start()
        {
            playerFov *= Mathf.Deg2Rad;
            playerFov = new Vector2(Mathf.Sin(playerFov.x), Mathf.Cos(playerFov.y));
            controlRef = FindObjectOfType<Controller>();
        }

        ///<summary>
        /// Updates targets on screen.
        ///</summary>      
        void Update()
        {
            foreach (StarShip target in bubbleTargets)
            {
                if (Vector3.Dot(transform.forward, target.gameObject.transform.position - transform.position) > 0f)
                {
                    //Draw target box.
                    Debug.Assert(playerCam != null, "PlayerCam variable is null.");
                    Vector2 targetScreenPos = playerCam.WorldToScreenPoint(target.transform.position);
                    if (targetBoxes.ContainsKey(target.name))
                    {
                        targetBoxes[target.name].gameObject.SetActive(true);
                        targetBoxes[target.name].rectTransform.localScale = Vector3.one;
                        targetBoxes[target.name].rectTransform.localPosition = new Vector3(targetScreenPos.x, targetScreenPos.y, 10f) - controlRef.screenCenter;
                    }

                }
                else
                {
                    //Draw off-screen pointer.
                    targetBoxes[target.name].gameObject.SetActive(false);
                }
            }
        }

        ///<summary>
        /// Adds new objects to radar.
        ///</summary>      
        public void OnTriggerEnter(Collider other)
        {
            StarShip temporaneous = other.gameObject.GetComponent<StarShip>();
            if (temporaneous != null)
            {
                Debug.Assert(boxPrefab != null, "BoxPrefab variable is null.");
                if (!bubbleTargets.Contains(temporaneous) && temporaneous.gameObject.name != "Player")
                {
                    bubbleTargets.Add(temporaneous);
                    Image newTB = Instantiate<Image>(boxPrefab);
                    newTB.rectTransform.SetParent(playerCanvas.transform);
                    newTB.transform.localScale = Vector3.one;
                    newTB.rectTransform.localPosition = Vector3.forward * 10f;
                    newTB.rectTransform.rotation = playerCanvas.transform.rotation;
                    targetBoxes.Add(other.gameObject.name, newTB);
                }
            }
        }

        ///<summary>
        /// Removes an object from the radar if it hasn't been already.
        ///</summary>      
        public void OnTriggerExit(Collider other)
        {
            StarShip temporaneous = other.gameObject.GetComponent<StarShip>();
            if (temporaneous != null)
            {
                if (bubbleTargets.Contains(temporaneous) && targetBoxes.ContainsKey(other.gameObject.name))
                {
                    bubbleTargets.Remove(temporaneous);
                    targetBoxes.Remove(other.gameObject.name);
                }
            }
        }
    }
}