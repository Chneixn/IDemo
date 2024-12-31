using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DRockInputBridge.Sample
{
    public class TestAnotherPlayer : DRockUnityInputReceiver
    {
        [SerializeField] private float jumpForce;
        private MeshRenderer rander;
        private Rigidbody rb;
        private TestGameManager game;

        public void Init(TestGameManager game, IDRockUserInput userInput)
        {
            this.game = game;
            base.Init(userInput, 2);
            rander = GetComponent<MeshRenderer>();
            rb = GetComponent<Rigidbody>();
        }

        public override void OnEnter()
        {
            rander.sharedMaterial.SetColor("1Color", Color.red);
        }

        public override void OnExit()
        {
            rander.sharedMaterial.SetColor("1Color", Color.white);
        }

        public override void OnPause()
        {
            rander.sharedMaterial.SetColor("1Color", Color.blue);
        }

        public override void OnResume()
        {
            rander.sharedMaterial.SetColor("1Color", Color.green);
        }

        public override void OnUpdate(float deltaTime)
        {
            if (userInput.GetKeyDown("Jump"))
            {
                rb.velocity = new Vector3(0, jumpForce, 0);
            }
            else if (userInput.GetKeyDown("Switch"))
            {
                game.pdFSM.Pop();
            }
        }
    }
}
