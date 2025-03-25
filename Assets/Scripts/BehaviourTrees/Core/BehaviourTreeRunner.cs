using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTreeSystem
{
    public class BehaviourTreeRunner : MonoBehaviour
    {
        public BehaviourTree tree;
        [SerializeField] protected bool isLog = false;

        protected virtual void Start()
        {
            tree.isLog = isLog;
            tree = tree.Clone();
        }

        protected virtual void Update()
        {
            tree.Update();
        }
    }
}

