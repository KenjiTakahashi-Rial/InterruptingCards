using System.Collections.Generic;

using TMPro;
using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Config;
using InterruptingCards.Models;
using System.Linq;

namespace InterruptingCards.Managers
{
    public abstract class AbstractGameManager : NetworkBehaviour
    {
        protected readonly CardConfig _cardConfig = CardConfig.Singleton;

        [SerializeField] protected CardPack _cardPack;
        [SerializeField] protected PlayerManager _playerManager;
        [SerializeField] protected StateMachineManager _stateMachineManager;
        [SerializeField] protected DeckManager _deckManager;
        [SerializeField] protected DeckManager _discardManager;
        [SerializeField] protected HandManager[] _handManagers;
        [SerializeField] protected TextMeshPro _tempInfoText;

        public static AbstractGameManager Singleton { get; protected set; }

        protected virtual int StartingHandCardCount => 4;

        public virtual void Awake()
        {
            Debug.Log("Waking game manager");

            _cardConfig.Load(_cardPack);
            Singleton = this;
        }

        public virtual void Update()
        {
            if (_playerManager.ActivePlayer == null || _tempInfoText == null)
            {
                return;
            }

            _tempInfoText.SetText($"{_playerManager.ActivePlayer.Id}\n{_stateMachineManager.CurrentStateName}");
        }

        public override void OnNetworkSpawn()
        {
            Debug.Log("Network spawned");

            base.OnNetworkSpawn();

            _deckManager.OnClicked += TryDrawCard;

            for (var i = 0; i < _handManagers.Length; i++)
            {
                var j = i;
                _handManagers[i].OnCardClicked += (int k) => TryPlayCard(j, k);
            }
        }

        public override void OnNetworkDespawn()
        {
            Debug.Log("Network despawned");

            _deckManager.OnClicked -= TryDrawCard;

            foreach (var handManager in _handManagers)
            {
                handManager.OnCardClicked = null;
            }

            if (_stateMachineManager.CurrentState != StateMachine.WaitingForClientsState)
            {
                _stateMachineManager.SetTrigger(StateMachine.ForceEndGameTrigger);
            }

            base.OnNetworkDespawn();
        }

        public override void OnDestroy()
        {
            Debug.Log("Destroying game manager");

            Singleton = null;
            base.OnDestroy();
        }

        /******************************************************************************************\
         * State Machine Methods                                                                  *
        \******************************************************************************************/

        public virtual void HandleInitializeGame()
        {
            Debug.Log("Initializing Game");

            _playerManager.Reset();
            AssignHands();
            
            if (IsServer)
            {
                _deckManager.Initialize();
                _deckManager.Shuffle();
                _deckManager.IsFaceUp = false;
                _discardManager.Clear();
                DealHandsServerRpc();
            }

            _stateMachineManager.SetTriggerClientRpc(StateMachine.StartGameTrigger);
        }

        public abstract void HandleStartTurn();

        public virtual void HandleEndTurn(int shifts = 1)
        {
            _playerManager.ShiftTurn(shifts);
        }

        public virtual void HandleEndGame()
        {
            Debug.Log("Ending game");

            _playerManager.Reset();
            _tempInfoText.SetText("Start the game");

            if (IsServer)
            {
                _deckManager.Clear();
                _discardManager.Clear();

                foreach (var handManager in _handManagers)
                {
                    handManager.Clear();
                }
            }
        }

        /******************************************************************************************\
         * Game Flow Methods                                                                      *
        \******************************************************************************************/

        protected virtual void AssignHands()
        {
            _playerManager.AssignHands(_handManagers);
        }

        [ServerRpc]
        protected virtual void DealHandsServerRpc()
        {
            if (_players.Count < MinPlayers)
            {
                Debug.LogWarning($"Cannot deal hands before {MinPlayers} joined");
                return;
            }

            if (CurrentStateId != _stateMachineConfig.GetId(StateMachine.InitializingGameState))
            {
                Debug.LogWarning("Cannot deal hands outside of game initialization state");
                return;
            }

            Debug.Log("Dealing hands");

            for (var i = 0; i < StartingHandCardCount; i++)
            {
                foreach (var hand in _handManagers)
                {
                    hand.Add(_deckManager.DrawTop());
                }
            }
        }

        protected virtual bool CanDrawCard(ulong id)
        {
            if (id != ActivePlayer.Id)
            {
                Debug.Log($"Player {id} cannot draw a card unless it is their turn");
                return false;
            }

            if (CurrentStateId != _stateMachineConfig.GetId(StateMachine.WaitingForDrawCardState))
            {
                Debug.Log($"Player {id} cannot draw a card in the wrong state");
                return false;
            }

            return true;
        }

        protected virtual void TryDrawCard()
        {
            Debug.Log("Trying to draw card");

            if (CanDrawCard(_selfId))
            {
                DrawCardServerRpc();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        protected virtual void DrawCardServerRpc(ServerRpcParams serverRpcParams = default)
        {
            if (!CanDrawCard(serverRpcParams.Receive.SenderClientId))
            {
                return;
            }

            Debug.Log("Drawing card");

            var card = _deckManager.DrawTop();
            ActivePlayer.Hand.Add(card);

            StateTriggerClientRpc(StateMachine.DrawCardTrigger);
        }

        protected virtual bool CanPlayCard(ulong id, int handManagerIndex)
        {
            if (id != ActivePlayer.Id)
            {
                Debug.Log($"Player {id} cannot play a card unless it is their turn or they are interrupting");
                return false;
            }

            if (_handManagers[handManagerIndex] != ActivePlayer.Hand)
            {
                Debug.Log($"Player {id} can only play cards from their own hand");
                return false;
            }

            if (CurrentStateId != _stateMachineConfig.GetId(StateMachine.WaitingForPlayCardState))
            {
                Debug.Log($"Player {id} cannot play a card in the wrong state");
                return false;
            }

            return true;
        }

        protected virtual void TryPlayCard(int handManagerIndex, int cardIndex)
        {
            var cardId = _handManagers[handManagerIndex][cardIndex];

            Debug.Log($"Trying to play card {_cardConfig.GetCardString(cardId)}");

            if (CanPlayCard(_selfId, handManagerIndex))
            {
                PlayCardServerRpc(handManagerIndex, cardIndex);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        protected virtual void PlayCardServerRpc(int handManagerIndex, int cardIndex, ServerRpcParams serverRpcParams = default)
        {
            var senderId = serverRpcParams.Receive.SenderClientId;

            if (!CanPlayCard(senderId, handManagerIndex))
            {
                return;
            }

            Debug.Log("Playing card");

            var cardId = _handManagers[handManagerIndex].RemoveAt(cardIndex);
            _discardManager.PlaceTop(cardId);

            StateTriggerClientRpc(StateMachine.PlayCardTrigger);
        }
    }
}
