using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTreesSystem
{
    public class BehaviourTreeRunner : MonoBehaviour
    {
        public BehaviourTree tree;
        public EnemyController enemyController;

        private void Start()
        {
            tree = tree.Clone();
            tree.Bind(GetComponent<EnemyController>());
        }

        private void Update()
        {
            tree.Update();
        }
    }
}

