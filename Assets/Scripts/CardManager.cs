using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace InterruptingCards
{
    public class CardManager : NetworkBehaviour
    {
        [SerializeField] TextMeshPro _cardName;

        private readonly NetworkVariable<int> _cardNameVar = new(value: -1, writePerm: NetworkVariableWritePermission.Owner);
        private readonly System.Random _random = new();

        private void OnEnable()
        {
            Update();
        }

        private void Update()
        {
            if (IsOwner && Input.GetKeyDown(KeyCode.Return))
            {
                _cardNameVar.Value = _random.Next(100);
            }

            var currentName = _cardNameVar.Value.ToString();
            if (!_cardName.text.Equals(currentName))
            {
                _cardName.SetText(currentName);
            }
        }
    }
}
