using System.Linq;

using TMPro;
using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Actions;
using InterruptingCards.Behaviours;
using InterruptingCards.Config;
using InterruptingCards.Managers.TheStack;
using InterruptingCards.Models;

namespace InterruptingCards.Managers
{
    public class GameManager : NetworkBehaviour
    {
        private const uint StartingLoot = 3;
        private const uint StartingMoney = 3;

        private readonly CardConfig _cardConfig = CardConfig.Singleton;

        [Header("Config")]
        [SerializeField] private CardPack _cardPack;

        [Header("Managers")]
        [SerializeField] private PlayerManager _playerManager;
        [SerializeField] private PriorityManager _priorityManager;
        [SerializeField] private StateMachineManager _stateMachineManager;
        [SerializeField] private StateMachineManager _theStackStateMachineManager;
        [SerializeField] private TheStackManager _theStackManager;

        [Header("Behaviours")]
        [SerializeField] private CardBehaviour[] _playerCards;
        [SerializeField] private DeckBehaviour _lootDeck;
        [SerializeField] private DeckBehaviour _lootDiscard;
        [SerializeField] private HandBehaviour[] _hands;

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
        [SerializeField] private TextMeshPro _tempStateText;
        [SerializeField] private TextMeshPro _tempPlayerText;

        public static GameManager Singleton { get; private set; }


        public PlayerManager PlayerManager => _playerManager;
        public PriorityManager PriorityManager => _priorityManager;
        public StateMachineManager StateMachineManager => _stateMachineManager;
        public StateMachineManager TheStackStateMachineManager => _theStackStateMachineManager;
        public TheStackManager TheStackManager => _theStackManager;

        public DeckBehaviour LootDeck => _lootDeck;
        public DeckBehaviour LootDiscard => _lootDiscard;

        private LogManager Log => LogManager.Singleton;

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

            if (_tempStateText != null)
            {
                _tempStateText.SetText(
                    $"{_stateMachineManager.CurrentStateName}\n{_theStackStateMachineManager.CurrentStateName}"
                );
            }

            if (_tempPlayerText != null)
            {
                var playerInfo = _playerManager.TempPlayers.Select(
                    p =>
                    {
                        var activeString = p == _playerManager.ActivePlayer ? " A" : " _";
                        var priorityString = p == _priorityManager.PriorityPlayer ? " P" : " _";
                        return $"{p.Name}{activeString}{priorityString}: {p.Money}¢";
                    }
                );

                _tempPlayerText.SetText(string.Join("\n", playerInfo));
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Log.Info("Network spawned");

            foreach (var card in _playerCards)
            {
                card.OnClicked += () => _activateAbility.TryExecute(card.CardId);
            }

            foreach (var hand in _hands)
            {
                hand.OnCardClicked += (int cardId) => TryPlayLoot(cardId);
            }
        }

        public override void OnNetworkDespawn()
        {
            Log.Info("Network despawned");

            foreach (var card in _playerCards)
            {
                card.OnClicked = null;
            }

            foreach (var hand in _hands)
            {
                hand.OnCardClicked = null;
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
            _playerManager.Initialize(StartingMoney);

            _playerManager.AssignHands(_hands);

            if (IsServer)
            {
                _lootDeck.Initialize(c => c.Suit == CardSuit.Loot);
                _lootDeck.Shuffle();
                _lootDeck.IsFaceUp = false;
                _lootDiscard.Clear();
                foreach (var hand in _hands)
                {
                    hand.Clear();
                }
                DealHands();
                _stateMachineManager.SetTrigger(StateMachine.StartGame);
            }

            SetCardsHidden(false);
        }

        public void RechargeStep()
        {
            if (IsServer)
            {
                foreach (var card in _priorityManager.PriorityPlayer.ActiveItems.Values)
                {
                    card.IsActivated = false;
                }

                _stateMachineManager.SetTrigger(StateMachine.RechargeComplete);
            }
        }

        public void TriggerStartOfTurnAbilities()
        {
            // TODO: Look up the abilities from the player
            if (IsServer)
            {
                //_theStackManager.PushAbility(_playerManager.ActivePlayer, CardAbility.Invalid);
            }
        }

        public void TryPassPriority()
        {
            _passPriority.TryExecute();
        }

        public void LootStep()
        {
            if (IsServer)
            {
                var card = _lootDeck.DrawTop();
                _playerManager.ActivePlayer.Hand.Add(card);
                _stateMachineManager.SetTrigger(StateMachine.LootComplete);
            }
        }

        public void PriorityPasses()
        {
            if (IsServer)
            {
                _theStackManager.Begin();
            }
        }

        public void TryAutoPassPriority()
        {
            if (IsServer)
            {
                _priorityManager.TryAutoPass();
            }
        }

        public void AddLootPlay()
        {
            if (IsServer)
            {
                _playerManager.ActivePlayer.LootPlays++;
                _stateMachineManager.SetTrigger(StateMachine.AddLootPlayComplete);
            }
        }

        public void TryDeclareAttack()
        {
            _declareAttack.TryExecute();
        }

        public void DeclareAttack()
        {
            if (IsServer)
            {
                _theStackManager.Begin();
            }
        }

        public void Attack()
        {
            // TODO: Prompt the active player to select a monster
            if (NetworkManager.LocalClientId == _playerManager.ActivePlayer.Id)
            {
                _attack.TryExecute(CardConfig.InvalidId);
            }
        }

        public void TryDeclarePurchase()
        {
            _declarePurchase.TryExecute();
        }

        public void DeclarePurchase()
        {
            if (IsServer)
            {
                _theStackManager.Begin();
            }
        }

        public void Purchase()
        {
            // TODO: Prompt the active player to select an item
            if (NetworkManager.LocalClientId == _playerManager.ActivePlayer.Id)
            {
                _purchase.TryExecute(CardConfig.InvalidId);
            }
        }

        public void TryPlayLoot(int cardId)
        {
            _playLoot.TryExecute(cardId);
        }

        public void PlayLoot(int cardId)
        {
            var player = _priorityManager.PriorityPlayer;
            player.LootPlays--;
            player.Hand.Remove(cardId);
            _theStackManager.PushLoot(_playerManager.ActivePlayer, cardId);
        }

        public void TryActivateAbility()
        {
            // TODO: This should include which ability is trying to be activated
            _activateAbility.TryExecute(CardConfig.InvalidId);
        }

        public void ActivateAbility(int cardId)
        {
            // TODO: Get the ability to be activated
            if (IsServer)
            {
                var cardBehaviour = _priorityManager.PriorityPlayer.ActiveItems[cardId];
                cardBehaviour.IsActivated = true;
                var card = _cardConfig[cardBehaviour.CardId];
                _theStackManager.PushAbility(_playerManager.ActivePlayer, card.ActivatedAbility);
            }
        }

        public void TryDeclareEndTurn()
        {
            _declareEndTurn.TryExecute();
        }

        public void DeclareEndTurn()
        {
            // TODO: Put the end turn declaration on the stack
            if (IsServer)
            {
                _theStackManager.Begin();
            }
        }

        public void TriggerEndOfTurnAbilities()
        {
            // TODO: Look up player's abilities
            if (IsServer)
            {
                _theStackManager.PushAbility(_playerManager.ActivePlayer, CardAbility.Invalid);
            }
        }

        public void Discard()
        {
            // TODO: Prompt the active player to discard (if necessary)
            if (IsServer)
            {
                _stateMachineManager.SetTrigger(StateMachine.DiscardComplete);
            }
        }

        public void ShiftRoom()
        {
            // TODO: Prompt the active player to 
            if (IsServer)
            {
                _stateMachineManager.SetTrigger(StateMachine.ShiftRoomComplete);
            }
        }

        public void EndTurn()
        {
            // TODO
            if (IsServer)
            {
                _playerManager.ActivePlayer.LootPlays = 0;
                _playerManager.ShiftTurn();
                _stateMachineManager.SetTrigger(StateMachine.EndTurn);
            }
        }

        public void EndGame()
        {
            Log.Info("Ending game");
            _playerManager.Clear();
            SetCardsHidden(true);
        }

        // Helper Methods

        private void SetCardsHidden(bool val)
        {
            Log.Info($"Setting cards hidden: {val}");
            _lootDeck.SetHidden(val);
            _lootDiscard.SetHidden(val);

            foreach (var hand in _hands)
            {
                hand.SetHidden(val);
            }
        }

        private void DealHands()
        {
            if (_stateMachineManager.CurrentState != StateMachine.InitializingGame)
            {
                Log.Warn("Cannot deal hands outside of game initialization state");
                return;
            }

            Log.Info("Dealing hands");
            for (var i = 0; i < StartingLoot; i++)
            {
                foreach (var hand in _hands)
                {
                    hand.Add(_lootDeck.DrawTop());
                }
            }
        }
    }
}
