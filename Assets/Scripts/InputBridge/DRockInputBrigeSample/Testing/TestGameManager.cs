using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DRockInputBridge.Sample
{
    public class TestGameManager : MonoBehaviour
    {
        [SerializeField] private TestPlayer player;
        [SerializeField] private TestAnotherPlayer anotherPlayer;

        public TestPlayer mainPlayer => player;
        public TestAnotherPlayer subPlayer => anotherPlayer;

        public DRockManagerSample pdFSM;

        private void Start()
        {
            pdFSM = new DRockManagerSample();
            player.Init(this, pdFSM);
            anotherPlayer.Init(this, pdFSM);
            pdFSM.Push(player);
        }

        private void Update()
        {
            pdFSM.OnUpdate(Time.deltaTime);
        }
    }
}
