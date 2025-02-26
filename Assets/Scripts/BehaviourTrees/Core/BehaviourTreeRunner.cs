using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTreesSystem
{
    public class BehaviourTreeRunner : MonoBehaviour
    {
        public BehaviourTree tree;
        public Agent agent;

        private void Start()
        {
            tree = tree.Clone();
            tree.Bind(GetComponent<Agent>());
        }

        private void Update()
        {
            tree.Update();
        }
    }
}

