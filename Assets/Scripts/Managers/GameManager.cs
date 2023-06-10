using TMPro;
using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Config;
using InterruptingCards.Models;
using InterruptingCards.Actions;

namespace InterruptingCards.Managers
{
    public class GameManager : NetworkBehaviour
    {
        private readonly CardConfig _cardConfig = CardConfig.Singleton;

        [Header("Config")]
        [SerializeField] private CardPack _cardPack;

        [Header("Managers")]
        [SerializeField] private DeckManager _deckManager;
        [SerializeField] private DeckManager _discardManager;
        [SerializeField] private HandManager[] _handManagers;
        [SerializeField] private PlayerManager _playerManager;
        [SerializeField] private StateMachineManager _stateMachineManager;
        [SerializeField] private StateMachineManager _theStackStateMachineManager;
        [SerializeField] private TheStackManager _theStackManager;

        [Header("Actions")]
        [SerializeField] private DeclareAttackAction _declareAttack;
        [SerializeField] private AttackAction _attack;
        [SerializeField] private DeclarePurchaseAction _declarePurchase;
        [SerializeField] private PurchaseAction _purchase;
        [SerializeField] private PlayLootAction _playLoot;
        [SerializeField] private ActivateAbilityAction _activateAbility;
        [SerializeField] private DeclareEndTurnAction _declareEndTurn;
        [SerializeField] private PassPriorityAction _passPriority;

        [Header("Temp")]
        [SerializeField] private TextMeshPro _tempInfoText;

        public static GameManager Singleton { get; private set; }

        private LogManager Log => LogManager.Singleton;

        private int StartingHandCardCount => 4;

        // Unity Methods

        public void Awake()
        {
            Singleton = this;
        }

        public void Start()
        {
            Log.Info("GameManager starting");

            // TODO: Workaround for "network prefabs list not empty" warning. Remove after upgrading to Netcode 1.4.1
            NetworkManager.NetworkConfig.Prefabs.NetworkPrefabsLists.Clear();
            
            _cardConfig.Load(_cardPack);
            SetCardsHidden(true);
        }

        public void Update()
        {
            // TODO: This is temporary

            if (_playerManager.ActivePlayer == null || _tempInfoText == null)
            {
                return;
            }

            _tempInfoText.SetText(
                $"{_playerManager.ActivePlayer.Id}\n" +
                $"{_stateMachineManager.CurrentStateName}\n" +
                $"{_theStackStateMachineManager.CurrentStateName}"
            );
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Log.Info("Network spawned");

            _deckManager.OnClicked += TryDrawCard;

            for (var i = 0; i < _handManagers.Length; i++)
            {
                var j = i;
                _handManagers[i].OnCardClicked += (int k) => TryPlayCard(j, k);
            }
        }

        public override void OnNetworkDespawn()
        {
            Log.Info("Network despawned");

            _deckManager.OnClicked -= TryDrawCard;

            foreach (var handManager in _handManagers)
            {
                handManager.OnCardClicked = null;
            }

            if (_stateMachineManager.CurrentState != StateMachine.WaitingForClients)
            {
                _stateMachineManager.SetTrigger(StateMachine.ForceEndGame);
            }

            base.OnNetworkDespawn();
        }

        public override void OnDestroy()
        {
            Singleton = null;
            base.OnDestroy();
        }

        // State Handlers

        public void Initialize()
        {
            Log.Info("Initializing Game");
            _playerManager.Initialize();

            _playerManager.AssignHands(_handManagers);

            if (IsServer)
            {
                _deckManager.Initialize();
                _deckManager.Shuffle();
                _deckManager.IsFaceUp = false;
                _discardManager.Clear();
                foreach (var hand in _handManagers)
                {
                    hand.Clear();
                }
                DealHandsServerRpc();
                _stateMachineManager.SetTrigger(StateMachine.StartGame);
            }

            SetCardsHidden(false);
        }

        public void RechargeStep()
        {
            // TODO
            _stateMachineManager.SetTrigger(StateMachine.RechargeComplete);
        }

        public void TriggerStartOfTurnAbilities()
        {
            // TODO
            _theStackManager.Begin();
        }

        public void TryPassPriority()
        {
            _passPriority.TryExecute();
        }

        public void LootStep()
        {
            // TODO
            _stateMachineManager.SetTrigger(StateMachine.LootComplete);
        }

        public void TryDeclareAttack()
        {
            _declareAttack.TryExecute();
        }

        public void DeclareAttack()
        {
            // TODO
            _theStackManager.Begin();
        }

        public void Attack()
        {
            // TODO
            _attack.TryExecute();
        }

        public void TryDeclarePurchase()
        {
            _declarePurchase.TryExecute();
        }

        public void DeclarePurchase()
        {
            // TODO
            _theStackManager.Begin();
        }

        public void Purchase()
        {
            // TODO
            _stateMachineManager.SetTrigger(StateMachine.PurchaseComplete);
        }

        public void TryPlayLoot()
        {
            _playLoot.TryExecute();
        }

        public void PlayLoot()
        {
            // TODO
            _theStackManager.Begin();
        }

        public void TryActivateAbility()
        {
            _activateAbility.TryExecute();
        }

        public void ActivateAbility()
        {
            // TODO
            _theStackManager.Begin();
        }

        public void TryDeclareEndTurn()
        {
            // TODO
            _declareEndTurn.TryExecute();
        }

        public void DeclareEndTurn()
        {
            // TODO
            _theStackManager.Begin();
        }

        public void TriggerEndOfTurnAbilities()
        {
            // TODO
            _theStackManager.Begin();
        }

        public void Discard()
        {
            // TODO
            _stateMachineManager.SetTrigger(StateMachine.DiscardComplete);
        }

        public void ShiftRoom()
        {
            // TODO
            _stateMachineManager.SetTrigger(StateMachine.ShiftRoomComplete);
        }

        public void EndTurn()
        {
            // TODO
            _playerManager.ShiftTurn();
            _stateMachineManager.SetTrigger(StateMachine.EndTurn);
        }

        public void EndGame()
        {
            Log.Info("Ending game");
            _tempInfoText.SetText("Start the game");
            _playerManager.Clear();
            SetCardsHidden(true);
        }

        // ServerRpc Methods & Tries

        [ServerRpc]
        private void DealHandsServerRpc()
        {
            if (_stateMachineManager.CurrentState != StateMachine.InitializingGame)
            {
                Log.Warn("Cannot deal hands outside of game initialization state");
                return;
            }

            Log.Info("Dealing hands");
            for (var i = 0; i < StartingHandCardCount; i++)
            {
                foreach (var hand in _handManagers)
                {
                    hand.Add(_deckManager.DrawTop());
                }
            }
        }

        private bool CanDrawCard(ulong id)
        {
            if (id != _playerManager.ActivePlayer.Id)
            {
                Log.Info($"Cannot draw a card unless it is their turn");
                return false;
            }

            if (_stateMachineManager.CurrentState != StateMachine.Looting)
            {
                Log.Info($"Player {id} cannot draw a card in the wrong state");
                return false;
            }

            return true;
        }

        private void TryDrawCard()
        {
            Log.Info("Trying to draw card");
            if (CanDrawCard(_playerManager.SelfId))
            {
                DrawCardServerRpc();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void DrawCardServerRpc(ServerRpcParams serverRpcParams = default)
        {
            if (!CanDrawCard(serverRpcParams.Receive.SenderClientId))
            {
                return;
            }

            Log.Info("Drawing card");
            var card = _deckManager.DrawTop();
            _playerManager.ActivePlayer.Hand.Add(card);
            _stateMachineManager.SetTrigger(StateMachine.LootComplete);
        }

        private bool CanPlayCard(ulong id, int handManagerIndex, int cardIndex)
        {
            if (id != _playerManager.ActivePlayer.Id)
            {
                Log.Info($"Player {id} cannot play a card unless it is their turn or they are interrupting");
                return false;
            }

            var hand = _handManagers[handManagerIndex];

            if (hand != _playerManager.ActivePlayer.Hand)
            {
                Log.Info($"Player {id} can only play cards from their own hand");
                return false;
            }

            if (cardIndex < 0 || hand.Count <= cardIndex)
            {
                Log.Info($"Player {id} cannot play a card from an invalid index of their hand");
                return false;
            }

            if (_stateMachineManager.CurrentState != StateMachine.PlayingLoot)
            {
                Log.Info($"Player {id} cannot play a card in the wrong state");
                return false;
            }

            return true;
        }

        private void TryPlayCard(int handManagerIndex, int cardIndex)
        {
            var cardId = _handManagers[handManagerIndex][cardIndex];
            Log.Info($"Trying to play card {_cardConfig.GetCardString(cardId)}");

            if (CanPlayCard(_playerManager.SelfId, handManagerIndex, cardIndex))
            {
                PlayCardServerRpc(handManagerIndex, cardIndex);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void PlayCardServerRpc(int handManagerIndex, int cardIndex, ServerRpcParams serverRpcParams = default)
        {
            var senderId = serverRpcParams.Receive.SenderClientId;
            if (!CanPlayCard(senderId, handManagerIndex, cardIndex))
            {
                return;
            }

            Log.Info("Playing card");
            var cardId = _handManagers[handManagerIndex].RemoveAt(cardIndex);
            _discardManager.PlaceTop(cardId);
            _theStackManager.Begin();
        }

        private void SetCardsHidden(bool val)
        {
            Log.Info($"Setting cards hidden: {val}");
            _deckManager.SetHidden(val);
            _discardManager.SetHidden(val);

            foreach (var hand in _handManagers)
            {
                hand.SetHidden(val);
            }
        }
    }
}
