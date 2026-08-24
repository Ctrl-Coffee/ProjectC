using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager
{
    private readonly string _baseUrl = "http://localhost:5256";
    private long _userId;
    private string _token;

    public CompanionApi CompanionService { get; private set; }

    public NetworkManager()
    {
        CompanionService = new CompanionApi();
    }

    public UniTask<RegisterResponse> RegisterAsync(string email,  string password, string nickname)
    {
        RegisterRequest request = new()
        {
            Email = email,
            Password = password,
            Nickname = nickname
        };

        return PostAsync<RegisterResponse>("/api/account/register", request);
    }

    public async UniTask<LoginResponse> LoginAsync(string email, string password)
    {
        LoginRequest request = new()
        {
            Email = email,
            Password = password
        };

        LoginResponse response = await PostAsync<LoginResponse>("/api/auth/login", request);

        if (response.result == 0)
        {
            _userId = response.userId;
            _token = response.token;
        }

        return response;
    }

    public UniTask<SaveCurrencyResponse> SaveCurrencyAsync(CurrencyDto currencyData)
    {
        SaveCurrencyRequest request = new()
        {
            UserId = _userId,
            Token = _token,
            CurrencyData = currencyData
        };

        return PostAsync<SaveCurrencyResponse>("/api/currency/save", request);
    }

    public UniTask<LoadCurrencyResponse> LoadCurrencyAsync()
    {
        AuthenticatedRequest request = new()
        {
            UserId = _userId,
            Token = _token
        };

        return PostAsync<LoadCurrencyResponse>("/api/currency/load", request);
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