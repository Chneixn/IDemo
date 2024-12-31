using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DRockInputBridge.Sample
{
    public class TestPlayer : DRockUnityInputReceiver
    {
        [SerializeField] private float moveSpeed;
        private MeshRenderer render;
        private TestGameManager gameManager;

        private void Awake()
        {
            render = GetComponent<MeshRenderer>();
        }

        /// <summary>
        /// 注册操作体，传入manager和fsm
        /// </summary>
        /// <param name="game"></param>
        /// <param name="useInput"></param>
        public void Init(TestGameManager game, IDRockUserInput useInput)
        {
            this.gameManager = game;
            base.Init(useInput, 0);
        }

        public override void OnEnter()
        {

            render.sharedMaterial.SetColor("2Color", Color.red);
        }

        public override void OnExit()
        {
            render.sharedMaterial.SetColor("2Color", Color.white);
        }

        public override void OnPause()
        {
            render.sharedMaterial.SetColor("2Color", Color.blue);
        }

        public override void OnResume()
        {
            render.sharedMaterial.SetColor("2Color", Color.green);
        }

        public override void OnUpdate(float deltaTime)
        {
            Vector2 dir = userInput.GetVector2("Movement");
            transform.Translate(deltaTime * moveSpeed * new Vector3(dir.x, 0, dir.y));

            if (userInput.GetKeyDown("Switch"))
            {
                gameManager.pdFSM.Push(gameManager.subPlayer);
            }
        }
    }
}