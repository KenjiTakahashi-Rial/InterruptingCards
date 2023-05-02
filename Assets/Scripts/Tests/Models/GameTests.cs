using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using InterruptingCards.Managers;

namespace InterruptingCards.Tests
{
    [TestFixture]
    public class GameTests
    {
        private GameObject _gameManagerObj;
        private GameObject _factoriesObj;
        private GameObject _canvasObj;
        private PlayingCardGameManager _gameManager;

        [SetUp]
        public void SetUp()
        {
            _gameManagerObj = Object.Instantiate(Resources.Load<GameObject>("Prefabs/GameManager"));
            _factoriesObj = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Factories"));
            _canvasObj = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Canvas"));

            _gameManager = _gameManagerObj.GetComponent<PlayingCardGameManager>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(_gameManagerObj);
            Object.Destroy(_factoriesObj);
            Object.Destroy(_canvasObj);

            _gameManager = null;
        }

        [Test]
        public void TestGame()
        {
            // Host and clients connect
            // For n times
            // Host draw
            // Host play
            // Client draw
            // Client play
            // Client disconnect
            // Host disconnect
        }
    }
}