using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trails2DDemo
{

    public class SceneManager : MonoBehaviour
    {
        public bool useToTestActivation = false;
        public GameObject testTarget;

        public List<GameObject> objects;
        private int id = 0;

        void Start()
        {

            // Here you can find little showcase on how to use Trails2D with code.

            if (useToTestActivation)
            {
                testTarget.GetComponent<Trails2D>().TriggerTrail(3);

                //testTarget.GetComponent<Trails2D>().StartTriggeringTrail();
                //
                //IEnumerator stop()
                //{
                //    yield return new WaitForSeconds(3);
                //    testTarget.GetComponent<Trails2D>().StopTriggeringTrail();
                //    Debug.Log("stop");
                //}
                //
                //StartCoroutine(stop());
            }
        }

        void Update()
        {

            if (useToTestActivation)
            {

            }
            else
            {
                /*if (Input.GetKeyDown(KeyCode.A))
                {
                    foreach (var item in objects)
                    {
                        item.GetComponent<Trails2D>().enabled = false;
                        item.GetComponent<SpriteRenderer>().enabled = false;
                    }
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    foreach (var item in objects)
                    {
                        item.GetComponent<Trails2D>().enabled = false;
                        item.GetComponent<SpriteRenderer>().enabled = false;
                    }

                    objects[id].GetComponent<Trails2D>().enabled = true;
                    objects[id].GetComponent<SpriteRenderer>().enabled = true;
                    id++;
                    id %= (objects.Count);
                }*/
            }
        }
    }
}