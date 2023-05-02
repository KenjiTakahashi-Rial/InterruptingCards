using System.Collections.Generic;
using System.Reflection;
using System.Threading;

using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Managers;

namespace InterruptingCards.Tests
{
    [TestFixture]
    public class GameTests
    {
        private readonly BindingFlags _bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;

        private GameObject _hostNetworkManagerObj;
        private GameObject _clientNetworkManagerObj;
        private GameObject _gameManagerObj;
        private GameObject _factoriesObj;
        private GameObject _canvasObj;
        private GameObject _deckManagerObj;
        private GameObject _discardManagerObj;
        private GameObject _handManagerTopObj;
        private GameObject _handManagerBottomObj;

        private NetworkManager _hostNetworkManager;
        private NetworkManager _clientNetworkManager;
        private PlayingCardGameManager _gameManager;
        private PlayingCardDeckManager _deckManager;
        private PlayingCardDeckManager _discardManager;
        private PlayingCardHandManager _handManagerTop;
        private PlayingCardHandManager _handManagerBottom;

        [SetUp]
        public void SetUp()
        {
            _hostNetworkManagerObj = Object.Instantiate(Resources.Load<GameObject>("Prefabs/NetworkManager"));
            _clientNetworkManagerObj = Object.Instantiate(Resources.Load<GameObject>("Prefabs/NetworkManager"));
            _gameManagerObj = Object.Instantiate(Resources.Load<GameObject>("Prefabs/GameManager"));
            _factoriesObj = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Factories"));
            _canvasObj = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Canvas"));
            _deckManagerObj = _canvasObj.transform.Find("Deck").gameObject;
            _discardManagerObj = _canvasObj.transform.Find("Discard").gameObject;
            _handManagerTopObj = _canvasObj.transform.Find("Hand (Top)").gameObject;
            _handManagerBottomObj = _canvasObj.transform.Find("Hand (Bottom)").gameObject;

            _hostNetworkManager = _hostNetworkManagerObj.GetComponent<NetworkManager>();
            _clientNetworkManager = _clientNetworkManagerObj.GetComponent<NetworkManager>();
            _gameManager = _gameManagerObj.GetComponent<PlayingCardGameManager>();
            _deckManager = _deckManagerObj.GetComponent<PlayingCardDeckManager>();
            _discardManager = _discardManagerObj.GetComponent<PlayingCardDeckManager>();
            _handManagerTop = _handManagerTopObj.GetComponent<PlayingCardHandManager>();
            _handManagerBottom = _handManagerBottomObj.GetComponent<PlayingCardHandManager>();

            var deckFieldInfo = typeof(PlayingCardGameManager).GetField("_deckManager", _bindingFlags);
            var discardFieldInfo = typeof(PlayingCardGameManager).GetField("_discardManager", _bindingFlags);
            var handsFieldInfo = typeof(PlayingCardGameManager).GetField("_handManagers", _bindingFlags);

            deckFieldInfo.SetValue(_gameManager, _deckManager);
            discardFieldInfo.SetValue(_gameManager, _discardManager);
            handsFieldInfo.SetValue(_gameManager, new PlayingCardHandManager[] {_handManagerTop, _handManagerBottom});
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(_hostNetworkManagerObj);
            Object.Destroy(_clientNetworkManagerObj);
            Object.Destroy(_gameManagerObj);
            Object.Destroy(_factoriesObj);
            Object.Destroy(_canvasObj);
            Object.Destroy(_deckManagerObj);
            Object.Destroy(_discardManagerObj);
            Object.Destroy(_handManagerTopObj);
            Object.Destroy(_handManagerBottomObj);

            _hostNetworkManager = null;
            _clientNetworkManager = null;
            _gameManager = null;
            _deckManager = null;
            _discardManager = null;
            _handManagerTop = null;
            _handManagerBottom = null;
        }

        [Test]
        public void TestGame()
        {
            Assert.AreEqual("Base.WaitingForClients", _gameManager.CurrentStateName, "Should be waiting for clients with 0 connections");

            List<ulong> lobby;
            FieldInfo lobbyFieldInfo = _gameManager.GetType().GetField("_lobby", _bindingFlags);

            _hostNetworkManager.StartHost();
            Assert.AreEqual("Base.WaitingForClients", _gameManager.CurrentStateName, "Should be waiting for clients with 1 connection");
            lobby = (List<ulong>)lobbyFieldInfo.GetValue(_gameManager);
            Assert.AreEqual(1, lobby.Count, "Should have 1 client in lobby after host connection");
            Assert.AreEqual(0uL, lobby[0], "First connected client should have ID 0 (host)");

            _clientNetworkManager.StartClient();
            Thread.Sleep(3000);
            Assert.AreEqual("Base.WaitingForAllReady", _gameManager.CurrentStateName, "Should be waiting for all ready with 2 connections");
            lobby = (List<ulong>)lobbyFieldInfo.GetValue(_gameManager);
            Assert.AreEqual(1, lobby.Count, "Should have 2 clients in lobby after client 1 connection");
            Assert.AreEqual(1uL, lobby[1], "Second connected client should have ID 1");

            // For n times
            // Check state machine
            // Host draw
            // Check state machine
            // Host play
            // Check state machine
            // Client draw
            // Check state machine
            // Client play
            // Check state machine
            // Client disconnect
            // Check state machine
            // Host disconnect
            // Check state machine
        }
    }
}