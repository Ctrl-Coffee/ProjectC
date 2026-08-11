using System;
using System.Collections.Generic;
using UnityEngine;

public class PartyFormationModel : ModelBase
{
    private readonly string _mainCharacterId;

    private readonly string[] _supportCharacterIds = new string[GameSettings.MaxSupportCount];

    public string MainCharacterId
    {
        get { return _mainCharacterId; }
    }

    public IReadOnlyList<string> SupportCharacterIds
    {
        get { return _supportCharacterIds; }
    }

    public event Action<int> SupportCharacterIdChanged;

    public PartyFormationModel()
    {
        //TODO 주인공 아이디 받아오기
        _mainCharacterId = "Red";
    }

    public override void InitializeOnce()
    {
        OnPropertyChanged(nameof(SupportCharacterIds));
    }

    public bool TryAddSupport(string characterId)
    {
        if (string.IsNullOrWhiteSpace(characterId))
        {
            return false;
        }

        if (ContainsSupportCharacter(characterId))
        {
            return false;
        }

        int emptySlot = FindEmptySupportSlot();

        if (emptySlot < 0)
        {
            return false;
        }

        bool isSupportSet = TrySetSupport(emptySlot, characterId);
        return isSupportSet;
    }

    public bool RemoveSupport(int slotIndex)
    {
        if (!IsSupportIndexInRange(slotIndex))
        {
            return false;
        }

        if (IsSlotEmpty(slotIndex))
        {
            return false;
        }

        _supportCharacterIds[slotIndex] = null;

        OnSupportSlotChanged(slotIndex);

        return true;
    }

    public bool SwapSupport(int firstSlotIndex, int secondSlotIndex)
    {
        if (firstSlotIndex == secondSlotIndex)
        {
            return false;
        }

        if (!IsSupportIndexInRange(firstSlotIndex) || !IsSupportIndexInRange(secondSlotIndex))
        {
            return false;
        }

        string first = _supportCharacterIds[firstSlotIndex];
        string second = _supportCharacterIds[secondSlotIndex];

        _supportCharacterIds[firstSlotIndex] = second;
        _supportCharacterIds[secondSlotIndex] = first;

        OnPropertyChanged(nameof(SupportCharacterIds));

        return true;
    }

    private bool ContainsSupportCharacter(string characterId)
    {
        if (string.IsNullOrWhiteSpace(characterId))
        {
            return false;
        }

        for (int index = 0; index < _supportCharacterIds.Length; index++)
        {
            if (_supportCharacterIds[index] == characterId)
            {
                return true;
            }
        }

        return false;
    }

    public bool TrySetSupport(int slotIndex, string characterId)
    {
        if (!IsSupportIndexInRange(slotIndex))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(characterId))
        {
            return false;
        }

        int existingSlotIndex = FindSupportSlotIndex(characterId);

        if (existingSlotIndex >= 0)
        {
            if (existingSlotIndex == slotIndex)
            {
                return false;
            }

            SwapSupport(existingSlotIndex, slotIndex);
            return true;
        }

        _supportCharacterIds[slotIndex] = characterId;

        OnSupportSlotChanged(slotIndex);

        return true;
    }

    private int FindEmptySupportSlot()
    {
        for (int i = 0; i < _supportCharacterIds.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(_supportCharacterIds[i]))
            {
                return i;
            }
        }

        return -1;
    }

    private bool IsSupportIndexInRange(int slotIndex)
    {
        bool isIndexInRange = slotIndex >= 0 && slotIndex < _supportCharacterIds.Length;
        return isIndexInRange;
    }

    public int FindSupportSlotIndex(string characterId)
    {
        if (string.IsNullOrWhiteSpace(characterId))
        {
            return -1;
        }

        for (int i = 0; i < _supportCharacterIds.Length; i++)
        {
            if (_supportCharacterIds[i] == characterId)
            {
                return i;
            }
        }

        return -1;
    }

    private bool IsSlotEmpty(int slotIndex)
    {
        if (!IsSupportIndexInRange(slotIndex))
        {
            Debug.LogError($"[{nameof(IsSlotEmpty)}] 전달된 매개변수가 인덱스 범위를 벗어났습니다. ({slotIndex})");
            return false;
        }

        bool isSlotEmpty = string.IsNullOrWhiteSpace(_supportCharacterIds[slotIndex]);
        return isSlotEmpty;
    }

    private void OnSupportSlotChanged(int slotIndex)
    {
        SupportCharacterIdChanged?.Invoke(slotIndex);
    }
}
