using System;
using System.Collections.Generic;
using System.Linq;

using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Factories;
using InterruptingCards.Models;

namespace InterruptingCards.Managers
{
    public class GameNetworkManager : NetworkBehaviour
    {
        private readonly NetworkManagerDecorator _networkManager = NetworkManagerDecorator.Singleton;

        public override void OnNetworkSpawn()
        {
            GetSelfServerRpc();

            _networkManager.OnClientConnectedCallback -= TryAddPlayer;
            _networkManager.OnClientConnectedCallback += TryAddPlayer;
            _networkManager.OnClientDisconnectCallback -= TryRemovePlayer;
            _networkManager.OnClientDisconnectCallback += TryRemovePlayer;

            _deckManager.OnDeckClicked -= TryDrawCard;
            _deckManager.OnDeckClicked += TryDrawCard;

            foreach (var handManager in _handManagers)
            {
                handManager.OnCardClicked -= TryPlayCard;
                handManager.OnCardClicked += TryPlayCard;
            }

            base.OnNetworkSpawn();
        }

        //public override void OnNetworkDespawn()
        //{
        //    _networkManager.OnClientConnectedCallback -= TryAddPlayer;
        //    _networkManager.OnClientDisconnectCallback -= TryRemovePlayer;

        //    _deckManager.OnDeckClicked -= TryDrawCard;

        //    foreach (var handManager in _handManagers)
        //    {
        //        handManager.OnCardClicked -= TryPlayCard;
        //    }

        //    base.OnNetworkDespawn();
        //}

        public void StartGame()
        {
            _deckManager.ResetDeck();
            AssignHands();
            _playerTurnNode = _players.First;
            StateTrigger(_startGameTriggerId);
        }

        //public override void OnDestroy()
        //{
        //    Singleton = null;
        //}

        private void Start()
        {
            Singleton = this;
        }

        private void StateTrigger(int triggerId)
        {
            _gameStateMachine.SetTrigger(triggerId);
        }

        private void ShiftTurn(int times = 1)
        {
            for (var i = 0; i < times; i++)
            {
                _playerTurnNode = _playerTurnNode.Next ?? _players.First;
            }
        }

        private void EndGame()
        {
            _playerTurnNode = null;
        }

        [ServerRpc]
        private void GetSelfServerRpc(ServerRpcParams serverRpcParams = default)
        {
            var clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { serverRpcParams.Receive.SenderClientId }
                }
            };

            GetSelfClientRpc(clientRpcParams);
        }

        [ClientRpc]
        private void GetSelfClientRpc(ClientRpcParams clientRpcParams)
        {
            var selfClientId = clientRpcParams.Send.TargetClientIds.First();
            _self = _players.First(p => p.Id == selfClientId);
        }

        private void AssignHands()
        {
            if (_players.Count > _handManagers.Count())
            {
                throw new TooManyPlayersException();
            }

            _playerHands = new();
            var i = 0;
            foreach (var player in _players)
            {
                var hand = _handManagers[i++];
                player.Hand = hand;
                _playerHands[player] = hand;
            }
        }

        private void TryAddPlayer(ulong clientId)
        {
            //if (IsServer && CurrentStateId == _waitingForClientsStateId && _players.Count < RequiredPlayersCount)
            //{
            //    AddPlayerServerRpc(clientId);
            //}
        }

        [ServerRpc]
        private void AddPlayerServerRpc(ulong clientId)
        {
            if (CurrentStateId != _waitingForClientsStateId)
            {
                Debug.LogWarning($"Did not add player ({clientId}): game already started");
                return;
            }

            if (_players.Count == RequiredPlayersCount)
            {
                Debug.LogWarning($"Did not add player ({clientId}): lobby is full");
                return;
            }

            _players.AddLast(_playerFactory.CreatePlayer(clientId, clientId.ToString()));

            if (_players.Count == RequiredPlayersCount)
            {
                StartGame();
            }
        }

        private void TryRemovePlayer(ulong clientId)
        {
            //if (!IsServer)
            //{
            //    Debug.LogWarning($"Client attempted to remove player ({clientId})");
            //    return;
            //}

            var player = _players.FirstOrDefault(p => p.Id == clientId) ?? throw new PlayerNotFoundException();

            if (CurrentStateId != _waitingForClientsStateId)
            {
                if (player == _playerTurnNode.Value)
                {
                    StateTrigger(_forceEndTurnTriggerId);
                    
                }
                Debug.LogWarning($"Player ({clientId}) removed while game is playing");
            }

            _players.Remove(player);

            if (_players.Count <= 1)
            {
                StateTrigger(_forceEndGameTriggerId);
            }
        }

        private void TryDrawCard()
        {
            if (IsSelfTurn && CurrentStateId == _waitingForDrawCardStateId)
            {
                DrawCardServerRpc();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void DrawCardServerRpc(ServerRpcParams serverRpsParams = default)
        {
            var senderId = serverRpsParams.Receive.SenderClientId;
            
            if (senderId != _playerTurnNode.Value.Id)
            {
                Debug.Log($"Player ({senderId}) tried to draw a card out of turn");
                return;
            }
            
            if (CurrentStateId == _waitingForDrawCardStateId)
            {
                Debug.Log($"Player ({senderId}) tried to draw when they weren't allowed to");
                return;
            }

            var card = _deckManager.DrawTop();
            _playerHands[_playerTurnNode.Value].Add(card);

            StateTrigger(_drawCardTriggerId);
        }

        private void TryPlayCard(ICard<S, R> card)
        {
            if (IsSelfTurn && CurrentStateId == _waitingForPlayCardStateId)
            {
                PlayCardServerRpc(card.Suit, card.Rank);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void PlayCardServerRpc(S suit, R rank, ServerRpcParams serverRpsParams = default)
        {
            var senderId = serverRpsParams.Receive.SenderClientId;
            
            if (senderId != _playerTurnNode.Value.Id)
            {
                Debug.Log($"Player ({senderId}) tried to play a card out of turn");
                return;
            }

            if (CurrentStateId != _waitingForPlayCardStateId)
            {
                Debug.Log($"Player ({senderId}) tried to play a card when they weren't allowed to");
                return;
            }

            var card = _playerHands[_playerTurnNode.Value].Remove(suit, rank);
            _discardManager.PlaceTop(card);

            StateTrigger(_playCardTriggerId);
        }
    }
}
