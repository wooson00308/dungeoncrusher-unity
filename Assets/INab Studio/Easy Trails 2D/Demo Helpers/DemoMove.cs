using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trails2DDemo
{
    public class DemoMove : MonoBehaviour
    {
        public float Speed = 10f;
        public Vector3 Direction;
        public bool useDamp = true;

        public GameObject targetPoints;
        private List<Transform> points = new List<Transform>();
        private int currentIndex = 0;


        private void Start()
        {
            if (useDamp)
                points.AddRange(targetPoints.GetComponentsInChildren<Transform>());
        }

        void Update()
        {
            /*if(Input.GetKeyDown(KeyCode.S))
            {
                GetComponent<Trails2D>().StopTriggeringTrail();
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                GetComponent<Trails2D>().StartTriggeringTrail();
            }*/

            if (useDamp)
            {
                if (points.Count > currentIndex)
                {
                    // Define a target position above and behind the target transform
                    Vector3 targetPosition = points[currentIndex].position;
                    if (transform.position == targetPosition)
                    {
                        currentIndex++;
                    }

                    // Smoothly move the camera towards that target position
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, Speed * Time.deltaTime);
                }
                else
                {
                    currentIndex = 0;
                }

            }
            else
            {
                transform.position += Direction * Speed * Time.deltaTime;
            }
        }
    }
}