using UnityEngine;

public class AutoButton : MonoBehaviour
{
    [SerializeField] private UIButtonComponent _autobutton;

    [SerializeField] private GameObject _onObject;
    [SerializeField] private GameObject _offObject;

    private bool _isAuto;
    
    private void Awake()
    {
        UnityUtility.ValidateReference(_autobutton, nameof(_autobutton));   
        UnityUtility.ValidateReference(_onObject, nameof(_onObject));   
        UnityUtility.ValidateReference(_offObject, nameof(_offObject));
    }

    private void OnEnable()
    {
        UpdateAutoMode(GameManager.Battle.AutoMode);
        GameManager.Battle.AutoModeChanged += UpdateAutoMode;
        _autobutton.BindButtonEvent(HandleAutoButton);

    }

    private void OnDisable()
    {
        GameManager.Battle.AutoModeChanged -= UpdateAutoMode;
        _autobutton.UnBindButtonAllEvent();
    }

    private void UpdateAutoMode(bool isAuto)
    {
        _isAuto = isAuto;

        _onObject.SetActive(isAuto);
        _offObject.SetActive(!isAuto);
    }

    private void HandleAutoButton()
    {
        GameManager.Battle.SetAutoMode(!_isAuto);
    }
}
