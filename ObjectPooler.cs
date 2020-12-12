using UnityEngine;
using System.Collections.Generic;

namespace com.braineeeedevs.mouseflight.game
{
    public class ObjectPooler : MonoBehaviour
    {
        /*Simple O(1) time complexity object pooler. */

        public GameObject prefab;
        public int max = 100;
        private Stack<GameObject> objects = new Stack<GameObject>();

        ///<summary>
        ///  Gets an object at src location and orientation. 'ShooterRef' is the return pool.
        ///</summary>      
        public GameObject GetObject(StarShip shooterRef, Transform src)
        {
            GameObject retObj;
            if (objects.Count > 0)
            {
                retObj = objects.Pop();
                retObj.GetComponent<ISpawn>().Init(src, this, shooterRef);
                retObj.SetActive(true);
            }
            else
            {
                retObj = Instantiate(prefab, src.position + src.rotation * src.forward * 1.50f, transform.rotation) as GameObject;
                retObj.GetComponent<ISpawn>().Init(src, this, shooterRef);
            }
            return retObj;
        }
        
        ///<summary>
        /// Puts an object back or destroys it if over the limit.
        ///</summary>      
        public void PutBack(GameObject obj)
        {
            if (objects.Count < max)
            {
                objects.Push(obj);
            }
            else
            {
                Destroy(obj);
            }
        }

    }
}