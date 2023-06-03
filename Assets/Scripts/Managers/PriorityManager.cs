using UnityEngine;

using InterruptingCards.Models;

namespace InterruptingCards.Managers
{
    public class PriorityManager : MonoBehaviour
    {
        [SerializeField] private PlayerManager _playerManager;

        public Player PriorityHolder { get; private set; }
    }
}
