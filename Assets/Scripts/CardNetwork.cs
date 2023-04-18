using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace InterruptingCards
{
    public class CardNetwork : NetworkBehaviour
    {
        [SerializeField] TextMeshPro _cardName;

        private readonly System.Random _random = new();

        private void Start()
        {
            _cardName.SetText("-1");
        }

        private void Update()
        {
            if (IsOwner && Input.GetKeyDown(KeyCode.Return))
            {
                _cardName.SetText(_random.Next(100).ToString());
            }
        }
    }
}
