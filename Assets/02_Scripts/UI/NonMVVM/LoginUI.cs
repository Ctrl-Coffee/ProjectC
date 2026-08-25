using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class LoginUI : UIBase
{
    [SerializeField] private TextMeshProUGUI _titleText;

    [SerializeField] private TMP_InputField _mailInputField;
    [SerializeField] private TMP_InputField _passwordInputField;
    [SerializeField] private TMP_InputField _nicknameInputField;

    [SerializeField] private UIButtonComponent _loginAndSignupButton;
    [SerializeField] private UIButtonComponent _changeButton;

    private void OnEnable()
    {
        _loginAndSignupButton.BindButtonEvent(TryLogin);
        _changeButton.BindButtonEvent(OnSignUp);
    }

    private void OnDisable()
    {
        _loginAndSignupButton.UnBindButtonAllEvent();
        _changeButton.UnBindButtonAllEvent();
    }

    private void Update()
    {
        var keyboard = Keyboard.current;

        if (keyboard == null || !keyboard.tabKey.wasPressedThisFrame)
            return;

        if (_mailInputField.isFocused)
        {
            _passwordInputField.Select();
            _passwordInputField.ActivateInputField();
        }
    }

    private void TryLogin()
    {
        LoginAndLoadAsync().Forget();
    }

    private void TrySingup()
    {
        SignupAndLoginAsync().Forget();
    }

    private async UniTask LoginAndLoadAsync()
    {
        _loginAndSignupButton.SetInteractable(false);

        try
        {
            LoginResponse response = await GameManager.Network.LoginAsync(_mailInputField.text, _passwordInputField.text);

            if (response.result != 0 || response.userId <= 0 || string.IsNullOrEmpty(response.token))
            {
                Logger.LogWarning($"Login Failed: {response.message}");
                ShowLoginFail();
                _loginAndSignupButton.SetInteractable(true);
                return;
            }

            await GameManager.Resource.LoadContentAsync(AddressablePath.Label.LOADDING);
            LoadingUI loadingUI = await GameManager.UI.OpenLoading();
            CloseUI();

            await GameManager.Instance.InitializeAfterLoginAsync(loadingUI.SetProgress);
        }
        catch (Exception)
        {
            _loginAndSignupButton.SetInteractable(true);
            ShowLoginFail();
        }
    }
    private async UniTask SignupAndLoginAsync()
    {
        _loginAndSignupButton.SetInteractable(false);

        try
        {
            RegisterResponse response = await GameManager.Network.RegisterAsync(
                _mailInputField.text, _passwordInputField.text, _nicknameInputField.text);

            if (response.result != 0)
            {
                Logger.LogWarning($"SignUp Failed: {response.message}");
                ConfirmData data = GameManager.DataTable.GetConfirmData(ConfirmDataKey.SIGNUP_FAIL);
                GameManager.UI.OpenConfirmUI(data);
                _loginAndSignupButton.SetInteractable(true);
                return;
            }

            await LoginAndLoadAsync();
        }
        catch (Exception)
        {
            ConfirmData data = GameManager.DataTable.GetConfirmData(ConfirmDataKey.SIGNUP_FAIL);
            GameManager.UI.OpenConfirmUI(data);
            _loginAndSignupButton.SetInteractable(true);
        }
    }

    private void OnSignUp()
    {
        _nicknameInputField.gameObject.SetActive(true);

        _loginAndSignupButton.UnBindButtonAllEvent();
        _loginAndSignupButton.BindButtonEvent(TrySingup);
        _loginAndSignupButton.ChangeButtonText("Sign Up");

        _changeButton.UnBindButtonAllEvent();
        _changeButton.BindButtonEvent(OnLogin);
        _changeButton.ChangeButtonText("Login");

        _titleText.text = "Sign Up";
    }

    private void OnLogin()
    {
        _nicknameInputField.gameObject.SetActive(false);

        _loginAndSignupButton.UnBindButtonAllEvent();
        _loginAndSignupButton.BindButtonEvent(TryLogin);
        _loginAndSignupButton.ChangeButtonText("Login");

        _changeButton.UnBindButtonAllEvent();
        _changeButton.BindButtonEvent(OnSignUp);
        _changeButton.ChangeButtonText("Sign Up");

        _titleText.text = "Login";
    }

    private void ShowLoginFail()
    {
        ConfirmData data = GameManager.DataTable.GetConfirmData(ConfirmDataKey.LOGIN_FAIL);
        GameManager.UI.OpenConfirmUI(data);
    }

    private void GameLoading()
    {

    }
}
