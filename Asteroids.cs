using UnityEngine;

namespace com.braineeeedevs.mouseflight.game
{
public class Asteroids : MonoBehaviour ///<summary> Spawns a quantity of asteroid prefabs at random positions and limited scale within a spherical radius.</summary>
{
    public Transform parent;
    public GameObject[] prefabs = new GameObject[2];
    public float radius, scale; /// <summary>
                                /// Radius where within the asteroids will be placed, and scale will be between Vector3.one and Vector3.one * scale * randomValue.
                                /// </summary>
    public int quantity;
    public float chance = 0.90f;

    void Start()
    {
        Transform t;
        int shipCount = 0;
        for (int i = 0; i < quantity; i++)
        {
            float prob = Random.Range(0f, 100f) * 0.01f;
            if (prob > chance)
            {
                t = Instantiate(prefabs[i%prefabs.Length]).transform;
                t.name += i.ToString();
                t.position = Random.insideUnitSphere * radius;
                t.rotation = Quaternion.LookRotation(Random.insideUnitSphere);
                if (t.tag != "Ship")
                {
                    t.localScale = Vector3.one * Random.Range(1f, scale);
                    t.SetParent(parent);
                } else{
                    t.name += shipCount++;
                }
            }
        }
    }
}}
