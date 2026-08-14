using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager
{
    public IReadOnlyList<OwnedEquipmentData> GetOwnedEquipments()
    {
        return GameManager.User.Equipments;
    }

    public OwnedEquipmentData GetOwnedEquipment(string equipmentId)
    {
        foreach(OwnedEquipmentData owned in GameManager.User.Equipments)
        {
            if (owned.EquipmentId == equipmentId)
            {
                return owned;
            }
        }

        return null;
    }

    public bool AddEquipment(string equipmentId)
    {
        if(GetOwnedEquipment(equipmentId) != null)
        {
            //TODO 희준 : 중복획득시 꿈의 조각 지급
            return false;
        }

        OwnedEquipmentData ownedData = new OwnedEquipmentData();
        ownedData.EquipmentId = equipmentId;
        ownedData.Level = 1;
        GameManager.User.Equipments.Add(ownedData);
        return true;
    }

    public bool Equip(string equipmentId)
    {
        if (GetOwnedEquipment(equipmentId) == null)
        {
            Debug.LogError($"{equipmentId}를 보유하지 않음");
            return false;
        }

        GameManager.User.Player.EquippedEquipmentId = equipmentId;
        return true;
    }

    public void Unequip()
    {
        GameManager.User.Player.EquippedEquipmentId = string.Empty;
    }

    public OwnedEquipmentData GetEquippedEquipment()
    {
        string equippedId = GameManager.User.Player.EquippedEquipmentId;

        if(string.IsNullOrEmpty(equippedId))
        {
            return null;
        }
        
        return GetOwnedEquipment(equippedId);
    }
}
