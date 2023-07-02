using System.Linq;

using TMPro;
using Unity.Netcode;
using UnityEngine;

using InterruptingCards.Actions;
using InterruptingCards.Behaviours;
using InterruptingCards.Config;
using InterruptingCards.Managers.TheStack;
using InterruptingCards.Models;
using InterruptingCards.Utilities;

namespace InterruptingCards.Managers
{
    public class GameManager : NetworkBehaviour
    {
        private const uint StartingLootCount = 3;
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
        [SerializeField] private CardBehaviour[] _characters;
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

        [Header("Debug")]
        [SerializeField] private TextMeshPro _debugStateText;
        [SerializeField] private TextMeshPro _debugPlayerText;
        [SerializeField] private TextMeshPro _debugTheStackText;
        [SerializeField] private int _debugTheStackTextCount;

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
            _theStackManager.OnEnd += TriggerPriorityPassComplete;
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

            if (_debugStateText != null)
            {
                _debugStateText.SetText(
                    $"{_stateMachineManager.CurrentStateName}\n{_theStackStateMachineManager.CurrentStateName}"
                );
            }

            if (_debugPlayerText != null)
            {
                try
                {
                    var playerInfo = _playerManager.DebugPlayers.Select(
                        p =>
                        {
                            var activeString = p == _playerManager.ActivePlayer ? " A" : " _";
                            var priorityString = p == _priorityManager.PriorityPlayer ? " P" : " _";
                            return $"{p.Name}{activeString}{priorityString}: {p.Money}¢";
                        }
                    );

                    _debugPlayerText.SetText(string.Join("\n", playerInfo));
                }
                catch (System.InvalidOperationException)
                {
                    // Sometimes players has two ID 0 at the beginning of the game
                    // It resolves itself within about a frame, so it's fine because this is just a test log
                }
            }

            if(_debugTheStackText != null)
            {
                var topN = TheStackManager.DebugTopN(_debugTheStackTextCount);
                var elementStrings = new string[_debugTheStackTextCount];

                for (var i = 0; i < topN.Length; i++)
                {
                    var e = topN[i];
                    var s = $"{e.Type} {e.PushedById} {e.Value}";
                    elementStrings[i] = i == 0 ? "TOP -> " + s : s;
                }

                var topNString = string.Join("\n", elementStrings);
                _debugTheStackText.SetText(topN.Length == 0 ? "The Stack is empty" : topNString);
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Log.Info("Network spawned");

            // TODO: Need to do this for all activated ability cards
            foreach (var card in _characters)
            {
                card.OnClicked += () => _activateAbility.TryExecute(card.CardId);
            }

            foreach (var hand in _hands)
            {
                hand.OnCardClicked += _playLoot.TryExecute;
            }
        }

        public override void OnNetworkDespawn()
        {
            Log.Info("Network despawned");

            foreach (var card in _characters)
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
            _theStackManager.OnEnd -= TriggerPriorityPassComplete;
            base.OnDestroy();
        }

        // State Handlers

        public void Initialize()
        {
            Log.Info("Initializing Game");

            if (IsServer)
            {
                InitializeCharacters();
                InitializeLoot();
                _playerManager.Initialize(StartingMoney);
                _stateMachineManager.SetTrigger(StateMachine.StartGame);

                foreach (var character in _characters)
                {
                    character.IsDeactivated = true;
                }
            }

            _playerManager.AssignCharacters(_characters);
            _playerManager.AssignHands(_hands);
            SetCardsHidden(false);
        }

        public void RechargeStep()
        {
            if (IsServer)
            {
                Log.Info("Recharging activated cards");

                var player = _playerManager.ActivePlayer;
                player.CharacterCard.IsDeactivated = false;

                //foreach (var card in player.Items)
                //{
                //    if (card.ActivatedAbility != CardAbility.Invalid)
                //    {
                //        card.IsDeactivated = false;
                //    }
                //}

                _stateMachineManager.SetTrigger(StateMachine.RechargeComplete);
            }
        }

        public void TriggerStartOfTurnAbilities()
        {
            if (IsServer)
            {
                // TODO: Look up the abilities from the player and push them to the stack
                _stateMachineManager.SetTrigger(StateMachine.StartPhaseTriggerAbilitiesComplete);
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
            if (IsServer)
            {
                // TODO: Look up the abilities from the player and push them to the stack
                _stateMachineManager.SetTrigger(StateMachine.EndPhaseTriggerAbilitiesComplete);
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

            foreach (var card in _characters)
            {
                card.SetHidden(val);
            }

            foreach (var hand in _hands)
            {
                hand.SetHidden(val);
            }
        }

        private void InitializeCharacters()
        {
            if (_stateMachineManager.CurrentState != StateMachine.InitializingGame)
            {
                Log.Warn("Cannot initialize characters outside of game initialization state");
                return;
            }

            Log.Info("Initializing characters");

            var characterDeck = _cardConfig.GenerateIdDeck(c => c.Suit == CardSuit.Characters);
            Functions.Shuffle(characterDeck);

            for (var i = 0; i < _characters.Length; i++)
            {
                _characters[i].CardId = characterDeck[i];
            }
        }

        private void InitializeLoot()
        {
            if (_stateMachineManager.CurrentState != StateMachine.InitializingGame)
            {
                Log.Warn("Cannot initialize loot outside of game initialization state");
                return;
            }

            Log.Info("Initializing loot");

            _lootDeck.Initialize(c => c.Suit == CardSuit.Loot);
            _lootDeck.Shuffle();
            _lootDeck.IsFaceUp = false;
            _lootDiscard.Clear();
            
            foreach (var hand in _hands)
            {
                hand.Clear();
            }

            for (var i = 0; i < StartingLootCount; i++)
            {
                foreach (var hand in _hands)
                {
                    hand.Add(_lootDeck.DrawTop());
                }
            }
        }

        private void TriggerPriorityPassComplete()
        {
            var currentState = StateMachineManager.CurrentState;
            switch (currentState)
            {
                case StateMachine.StartPhaseTriggeringAbilities:
                    StateMachineManager.SetTrigger(StateMachine.StartPhaseTriggerAbilitiesComplete);
                    return;
                case StateMachine.GamePriorityPassing:
                    StateMachineManager.SetTrigger(StateMachine.GamePriorityPassComplete);
                    return;
                case StateMachine.PlayingLoot:
                    StateMachineManager.SetTrigger(StateMachine.PlayLootComplete);
                    return;
                case StateMachine.ActivatingAbility:
                    StateMachineManager.SetTrigger(StateMachine.ActivateAbilityComplete);
                    return;
                case StateMachine.DeclaringAttack:
                    StateMachineManager.SetTrigger(StateMachine.DeclareAttackComplete);
                    return;
                case StateMachine.DeclaringPurchase:
                    StateMachineManager.SetTrigger(StateMachine.DeclarePurchaseComplete);
                    return;
                case StateMachine.DeclaringEndTurn:
                    StateMachineManager.SetTrigger(StateMachine.DeclareEndTurnComplete);
                    return;
                case StateMachine.EndPhaseTriggeringAbilities:
                    StateMachineManager.SetTrigger(StateMachine.EndPhaseTriggerAbilitiesComplete);
                    return;
                default:
                    Log.Warn($"Priority pass completed, but state machine is in {currentState}");
                    break;
            }
        }
    }
}
