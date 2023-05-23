using System.Linq;

using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Behaviours;
using InterruptingCards.Config;
using InterruptingCards.Models;
using System;

namespace InterruptingCards.Managers
{
    public class InterruptingGameManager : AbstractGameManager
    {
        protected NetworkVariable<PlayerId> _interruptingPlayer = new(null);
        protected int? _stateBeforeInterrupt;

        [SerializeField] protected CardBehaviour _interruptingCard;

        public static new InterruptingGameManager Singleton { get; protected set; }

        public override void Awake()
        {
            base.Awake();
            Singleton = this;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _interruptingCard.OnClicked += TryInterrupt;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            _interruptingCard.OnClicked -= TryInterrupt;
        }

        public override void OnDestroy()
        {
            Singleton = null;
            base.OnDestroy();
        }

        public override void HandleInitializeGame()
        {
            base.HandleInitializeGame();

            if (IsServer)
            {
                _interruptingCard.CardId = CardConfig.GetCardId(CardSuit.InterruptingSuit, CardRank.InterruptingRank);
                _interruptingPlayer.Value = null;
            }
        }

        public override void HandleStartTurn()
        {
            Debug.Log($"Starting player {ActivePlayer.Id} turn");

            if (IsServer)
            {
                _interruptingCard.IsActivated = false;
            }
        }

        public override void HandleEndGame()
        {
            base.HandleEndGame();

            if (IsServer)
            {
                _interruptingCard.CardId = CardConfig.InvalidId;
            }
        }


        protected virtual void HandleEffect(CardActiveEffect effect)
        {
            Debug.Log($"{effect} active effect");

            switch (effect)
            {
                case CardActiveEffect.PlayCard:
                    StateTrigger(StateMachine.PlayCardActiveEffectTrigger);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        protected override bool CanDrawCard(ulong id)
        {
            var playerTurn = id == ActivePlayer.Id;
            var playerInterrupt = id == _interruptingPlayer.Value?.Id;
            var interruptInProgress = _interruptingPlayer.Value != null;

            if (!playerTurn && !playerInterrupt)
            {
                Debug.Log($"Player {id} cannot draw a card unless it is their turn or they are interrupting");
                return false;
            }

            if (playerTurn && interruptInProgress)
            {
                Debug.Log($"Player {id} cannot draw a card while they are being interrupted");
                return false;
            }

            if (CurrentStateId != _stateMachineConfig.GetId(StateMachine.WaitingForDrawCardState))
            {
                Debug.Log($"Player {id} cannot draw a card in the wrong state");
                return false;
            }

            return true;
        }

        protected override bool CanPlayCard(ulong id, int handManagerIndex)
        {
            var isPlayerTurn = id == ActivePlayer.Id;
            var isPlayerInterrupting = id == _interruptingPlayer.Value?.Id;
            var isInterruptInProgress = _interruptingPlayer.Value != null;
            var hand = _handManagers[handManagerIndex];
            var isPlayerHand = hand == _players.Single(p => p.Id == id).Hand;

            if (!isPlayerTurn && !isPlayerInterrupting)
            {
                Debug.Log($"Player {id} cannot play a card unless it is their turn or they are interrupting");
                return false;
            }

            if (isPlayerTurn && isInterruptInProgress)
            {
                Debug.Log($"Player {id} cannot play a card while they are being interrupted");
                return false;
            }

            if (!isPlayerHand)
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

        [ServerRpc(RequireOwnership = false)]
        protected override void PlayCardServerRpc(int handManagerIndex, int cardIndex, ServerRpcParams serverRpcParams = default)
        {
            base.PlayCardServerRpc(handManagerIndex, cardIndex, serverRpcParams);

            if (_interruptingPlayer != null)
            {
                _interruptingPlayer = null;
                var triggerId = _stateMachineConfig.StateToTrigger(_stateBeforeInterrupt.Value);
                _stateBeforeInterrupt = null;
                StateTriggerClientRpc(triggerId);
            }
        }

        protected virtual bool CanInterrupt(ulong id)
        {
            var playerTurn = id == ActivePlayer.Id;
            var interruptInProgress = _interruptingPlayer.Value != null;

            if (playerTurn)
            {
                Debug.Log($"Player {id} cannot interrupt their own turn");
                return false;
            }

            if (interruptInProgress)
            {
                Debug.Log($"Player {id} cannot interrupt an interrupt");
                return false;
            }

            return true;
        }

        // TODO: Temporary
        protected virtual void TryInterrupt()
        {
            TryInterrupt(_cardConfig[_interruptingCard.CardId].ActiveEffect);
        }

        protected virtual void TryInterrupt(CardActiveEffect effect)
        {
            if (CanInterrupt(_selfId))
            {
                StartInterruptServerRpc(effect);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        protected virtual void StartInterruptServerRpc(CardActiveEffect effect, ServerRpcParams serverRpcParams = default)
        {
            var senderId = serverRpcParams.Receive.SenderClientId;

            if (!CanInterrupt(senderId))
            {
                return;
            }

            Debug.Log($"Player {senderId} interrupting player {ActivePlayer.Id}'s turn with {effect}");

            StateTriggerClientRpc();

            _stateBeforeInterrupt = CurrentStateId;
            _interruptingCard.IsActivated = true;
            _interruptingPlayer.Value = new PlayerId(senderId);
            HandleEffect(effect);
        }

        protected class PlayerId : INetworkSerializable
        {
            private ulong _id;

            // Empty constructor required for INetworkSerializable
            public PlayerId() { }

            public PlayerId(ulong id)
            {
                _id = id;
            }

            public ulong Id => _id;

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref _id);
            }
        }
    }
}