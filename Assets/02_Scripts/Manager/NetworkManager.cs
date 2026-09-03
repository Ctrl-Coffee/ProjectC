using Cysharp.Threading.Tasks;
using DG.Tweening.Plugins;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager
{
    private readonly string _baseUrl = "http://13.54.50.48";
    private long _userId;
    private string _token;

    private List<CompanionDto> _companions = new();
    private List<EquipmentDto> _equipments = new();
    private EquipmentLoadoutDto _equipmentLoadoutDto = new();

    private AuthenticatedRequest _authenticatedRequest = new();

    public UniTask<RegisterResponse> RegisterAsync(string email, string password, string nickname)
    {
        RegisterRequest request = new()
        {
            email = email,
            password = password,
            nickname = nickname
        };

        return PostAsync<RegisterResponse>("/api/account/register", request);
    }

    public async UniTask<LoginResponse> LoginAsync(string email, string password)
    {
        LoginRequest request = new()
        {
            email = email,
            password = password
        };

        LoginResponse response = await PostAsync<LoginResponse>("/api/auth/login", request);

        if (response.result == 0)
        {
            _userId = response.userId;
            _token = response.token;

            _authenticatedRequest.userId = _userId;
            _authenticatedRequest.token = _token;
        }

        return response;
    }

    public UniTask<SaveCurrencyResponse> SaveCurrencyAsync(CurrencyModel currencyModel, CoffeePotModel coffeePotModel)
    {
        CurrencyDto currencyDto = new CurrencyDto()
        {
            money = currencyModel.Money,
            dreamPoint = currencyModel.DreamPoint,
            energy = currencyModel.Energy,
            dreamFragment = currencyModel.DreamFragment,
            dreamScroll = currencyModel.DreamScroll,
            inspiration = currencyModel.Inspiration,
            energyRecoveredAt = currencyModel.EnergyRecoveredAt,
            coffeeUsedAt = coffeePotModel.UsedAtTicks
        };

        SaveCurrencyRequest request = new()
        {
            userId = _userId,
            token = _token,
            currencyData = currencyDto
        };

        return PostAsync<SaveCurrencyResponse>("/api/currency/save", request);
    }

    public UniTask<LoadCurrencyResponse> LoadCurrencyAsync()
    {
        return PostAsync<LoadCurrencyResponse>("/api/currency/load", _authenticatedRequest);
    }

    public UniTask<SaveCompanionResponse> SaveCompanionAsync(CompanionModel companionModel)
    {
        _companions.Clear();

        foreach (var companion in companionModel.Companions)
        {
            _companions.Add(new CompanionDto()
            {
                companionId = companion.Key,
                level = companion.Value.Level
            });
        }

        CompanionWrapperDto companionWrapperDto = new CompanionWrapperDto()
        {
            companions = _companions
        };

        SaveCompanionRequest request = new()
        {
            userId = _userId,
            token = _token,
            CompanionData = companionWrapperDto
        };
        return PostAsync<SaveCompanionResponse>("/api/companion/save", request);
    }

    public UniTask<LoadCompanionResponse> LoadCompanionAsync()
    {
        return PostAsync<LoadCompanionResponse>("/api/companion/load", _authenticatedRequest);
    }

    public UniTask<SaveEquipmentResponse> SaveEquipmentAsync(HeroEquipmentModel equipmentModel)
    {
        _equipments.Clear();

        foreach (var equipment in equipmentModel.Equipments)
        {
            _equipments.Add(new EquipmentDto()
            {
                equipmentId = equipment.Key,
                level = equipment.Value.Level
            });
        }

        EquipmentWrapperDto equipmentWrapperDto = new EquipmentWrapperDto()
        {
            equipments = _equipments
        };

        SaveEquipmentRequest request = new()
        {
            userId = _userId,
            token = _token,
            EquipmentData = equipmentWrapperDto
        };

        return PostAsync<SaveEquipmentResponse>("/api/equipment/save", request);
    }

    public UniTask<LoadEquipmentResponse> LoadEquipmentAsync()
    {
        return PostAsync<LoadEquipmentResponse>("/api/equipment/load", _authenticatedRequest);
    }

    public UniTask<SaveEquipmentLoadoutResponse> SaveEquipmentLoadoutAsync(HeroEquipedModel heroEquipedModel)
    {
        _equipmentLoadoutDto.weaponEquipmentId = heroEquipedModel.EquipedWeaponId;
        _equipmentLoadoutDto.armorEquipmentId = heroEquipedModel.EquipedArmorId;
        _equipmentLoadoutDto.accessoryEquipmentId = heroEquipedModel.EquipedAccessoryId;

        SaveEquipmentLoadoutRequest request = new()
        {
            userId = _userId,
            token = _token,
            EquipmentLoadoutData = _equipmentLoadoutDto
        };

        return PostAsync<SaveEquipmentLoadoutResponse>("/api/equipmentloadout/save", request);
    }

    public UniTask<LoadEquipmentLoadoutResponse> LoadEquipmentLoadoutAsync()
    {
        return PostAsync<LoadEquipmentLoadoutResponse>("/api/equipmentloadout/load", _authenticatedRequest);
    }

    public UniTask<SaveStageRecordResponse> SaveStageAsync(string stageId)
    {
        StageRecordRequest request = new()
        {
            userId = _userId,
            token = _token,
            StageRecordData = new StageRecordDto() { lastClearedStage = stageId }
        };

        return PostAsync<SaveStageRecordResponse>("/api/stagerecord/save", request);
    }

    public UniTask<LoadStageRecordResponse> LoadStageAsync()
    {
        return PostAsync<LoadStageRecordResponse>("/api/stagerecord/load", _authenticatedRequest);
    }

    public UniTask<SaveHeroLevelResponse> SaveHeroLevelAsync(HeroInfoModel heroInfoModel)
    {
        HeroLevelRequest request = new()
        {
            userId = _userId,
            token = _token,
            HeroLevelData = new HeroLevelDto() { userLevel = heroInfoModel.Level }
        };

        return PostAsync<SaveHeroLevelResponse>("/api/herolevel/save", request);
    }

    public UniTask<LoadHeroLevelResponse> LoadHeroLevelAsync()
    {
        return PostAsync<LoadHeroLevelResponse>("/api/herolevel/load", _authenticatedRequest);
    }

    public UniTask<SavePerkResponse> SavePerkAsync()
    {
        PerkWrapperDto wrapperDto = new PerkWrapperDto()
        {
            perkNodeIds = new List<string>(GameManager.Perk.GetUnlockedPerkIds())
        };

        PerkRequest request = new()
        {
            userId = _userId,
            token = _token,
            PerkData = wrapperDto
        };

        return PostAsync<SavePerkResponse>("/api/perk/save", request);
    }

    public UniTask<LoadPerkResponse> LoadPerkAsync()
    {
        return PostAsync<LoadPerkResponse>("/api/perk/load", _authenticatedRequest);
    }

    public UniTask<SaveAutoWorkSlotReponse> SaveAutoWorkSlotAsync()
    {
        List<AutoWorkSlotDto> autoWorkSlotDtos = new();

        foreach (var slot in AutoWorkQueue.GetSlots())
        {
            autoWorkSlotDtos.Add(new AutoWorkSlotDto()
            {
                workId = slot.WorkId,
                startTicks = slot.StartTicks,
                endTicks = slot.EndTicks
            });
        }

        AutoWorkSlotWrapperDto wrapperDto = new()
        {
            slots = autoWorkSlotDtos
        };

        AutoWorkSlotRequest request = new()
        {
            userId = _userId,
            token = _token,
            AutoWorkSlotData = wrapperDto
        };

        return PostAsync<SaveAutoWorkSlotReponse>("/api/autoworkslot/save", request);
    }

    public UniTask<LoadAutoWorkSlotResponse> LoadAutoWorkSlotAsync()
    {
        return PostAsync<LoadAutoWorkSlotResponse>("/api/autoworkslot/load", _authenticatedRequest);
    }

    public UniTask<SaveCompanionPartyResponse> SaveCompanionPartyAsync(string[] companionIds)
    {
        CompanionPartyDto companionPartyDto = new();

        companionPartyDto.companionIds[0] = companionIds[0];
        companionPartyDto.companionIds[1] = companionIds[2];

        CompanionPartyRequest request = new()
        {
            userId = _userId,
            token = _token,
            CompanionPartyData = companionPartyDto
        };

        return PostAsync<SaveCompanionPartyResponse>("/api/companionparty/save", request);
    }

    public UniTask<LoadCompanionPartyResponse> LoadCompanionPartyAsync()
    {
        return PostAsync<LoadCompanionPartyResponse>("/api/companionparty/load", _authenticatedRequest);
    }

    public UniTask<ProfileResponse> LoadProfileAsync()
    {
        return PostAsync<ProfileResponse>("/api/account/profile", _authenticatedRequest);
    }



    private async UniTask<TResponse> PostAsync<TResponse>(string path, object requestData)
    {
        string requestJson = JsonUtility.ToJson(requestData);

        string responseJson = await PostJsonAsync(path, requestJson);

        return JsonUtility.FromJson<TResponse>(responseJson);
    }

    private async UniTask<string> PostJsonAsync(string path, string json)
    {
        string url = $"{_baseUrl}{path}";

        using UnityWebRequest request = UnityWebRequest.Post(url, json, "application/json");

        await request.SendWebRequest().ToUniTask();

        if (request.result != UnityWebRequest.Result.Success)
        {
            throw new InvalidOperationException($"{request.responseCode}: {request.downloadHandler.text}");
        }

        return request.downloadHandler.text;
    }
}