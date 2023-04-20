using TMPro;

using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace InterruptingCards.Controllers
{
    public class CardController : NetworkBehaviour
    {
        private const string DefaultCardName = "";

        private readonly NetworkVariable<FixedString32Bytes> _cardNameNetwork = new(value: new FixedString32Bytes(DefaultCardName), writePerm: NetworkVariableWritePermission.Owner);
        private readonly System.Random _random = new();

        [SerializeField] private TextMeshPro _cardName;

        public override void OnNetworkSpawn()
        {
            _cardName.SetText(_cardNameNetwork.Value.ToString());
            _cardNameNetwork.OnValueChanged += (FixedString32Bytes _, FixedString32Bytes val) => _cardName.SetText(val.ToString());
        }

        private void Update()
        {
            if (IsOwner && Input.GetKeyDown(KeyCode.Return))
            {
                _cardNameNetwork.Value = new FixedString32Bytes(_random.Next(100).ToString());
            }
        }
    }
}
