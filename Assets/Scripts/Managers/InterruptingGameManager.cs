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
        protected Player _interruptingPlayer;

        [SerializeField] protected CardBehaviour _interruptingCard;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _interruptingCard.OnClicked -= TryInterruptPlayCard;
            _interruptingCard.OnClicked += TryInterruptPlayCard;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            _interruptingCard.OnClicked -= TryInterruptPlayCard;
        }

        public override void HandleInitializeGame()
        {
            base.HandleInitializeGame();

            if (IsServer)
            {
                _interruptingCard.Card = CardFactory.Singleton.Create(
                    CardConfig.GetCardId(CardSuit.InterruptingSuit, CardRank.InterruptingRank)
                );
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
                _interruptingCard.Card = null;
            }
        }


        protected virtual void HandleEffect(CardActiveEffect effect)
        {
            Debug.Log($"{effect} active effect");

            switch (effect)
            {
                case CardActiveEffect.PlayCard:
                    StateTriggerClientRpc(StateMachine.PlayCardActiveEffectTrigger);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        protected override bool CanDrawCard(ulong id)
        {
            var playerTurn = id == ActivePlayer.Id;
            var playerInterrupt = id == _interruptingPlayer?.Id;
            var interruptInProgress = _interruptingPlayer != null;

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

        protected override bool CanPlayCard(ulong id)
        {
            var playerTurn = id == ActivePlayer.Id;
            var playerInterrupt = id == _interruptingPlayer?.Id;
            var interruptInProgress = _interruptingPlayer != null;

            if (!playerTurn && !playerInterrupt)
            {
                Debug.Log($"Player {id} cannot play a card unless it is their turn or they are interrupting");
                return false;
            }

            if (playerTurn && interruptInProgress)
            {
                Debug.Log($"Player {id} cannot play a card while they are being interrupted");
                return false;
            }

            if (CurrentStateId != _stateMachineConfig.GetId(StateMachine.WaitingForPlayCardState))
            {
                Debug.Log($"Player {id} cannot play a card in the wrong state");
                return false;
            }

            return true;
        }

        protected virtual bool CanInterrupt(ulong id)
        {
            var playerTurn = id == ActivePlayer.Id;
            var interruptInProgress = _interruptingPlayer != null;

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

        protected virtual void TryInterrupt(CardActiveEffect effect)
        {
            if (CanInterrupt(_selfId))
            {
                InterruptServerRpc(effect);
            }
        }

        // TODO: Temporary
        protected virtual void TryInterruptPlayCard()
        {
            TryInterrupt(_interruptingCard.Card.ActiveEffect);
        }

        [ServerRpc]
        protected virtual void InterruptServerRpc(CardActiveEffect effect, ServerRpcParams serverRpcParams = default)
        {
            var senderId = serverRpcParams.Receive.SenderClientId;

            if (!CanInterrupt(senderId))
            {
                return;
            }

            Debug.Log($"Player {senderId} interrupting player {ActivePlayer.Id}'s turn");

            _interruptingCard.IsActivated = true;
            _interruptingPlayer = _players.Single(p => p.Id == senderId);
            HandleEffect(effect);
        }
    }
}