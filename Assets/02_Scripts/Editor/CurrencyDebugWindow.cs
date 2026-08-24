using UnityEditor;
using UnityEngine;

public class CurrencyDebugWindow : EditorWindow
{
    private const long DEFAULT_AMOUNT = 1000;
    private const long MIN_AMOUNT = 0;
    private const double REPAINT_INTERVAL = 0.25;

    private static readonly long[] PRESET_AMOUNTS = { 100, 1000, 10000 };

    private enum CurrencyType
    {
        Money,
        DreamPoint,
        Energy,
        DreamFragment,
        DreamScroll,
        Inspiration,
    }

    private static readonly CurrencyType[] CURRENCY_TYPES =
    {
        CurrencyType.Money,
        CurrencyType.DreamPoint,
        CurrencyType.Energy,
        CurrencyType.DreamFragment,
        CurrencyType.DreamScroll,
        CurrencyType.Inspiration,
    };

    private long _amount = DEFAULT_AMOUNT;
    private Vector2 _scrollPosition;
    private double _nextRepaintTime;

    [MenuItem("Tools/Currency Debug")]
    private static void Open()
    {
        GetWindow<CurrencyDebugWindow>("Currency Debug");
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (EditorApplication.timeSinceStartup < _nextRepaintTime)
        {
            return;
        }

        _nextRepaintTime = EditorApplication.timeSinceStartup + REPAINT_INTERVAL;

        Repaint();
    }

    private void OnGUI()
    {
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

        DrawCurrency();

        EditorGUILayout.EndScrollView();
    }

    private void DrawCurrency()
    {
        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("재화 지급은 플레이 모드에서만 동작합니다.", MessageType.Info);
            return;
        }

        if (null == GameManager.Instance)
        {
            EditorGUILayout.HelpBox("GameManager가 아직 준비되지 않았습니다.", MessageType.Warning);
            return;
        }

        CurrencyModel currency = GameManager.Session.Currency;

        if (null == currency)
        {
            EditorGUILayout.HelpBox("재화 데이터가 아직 준비되지 않았습니다.", MessageType.Warning);
            return;
        }

        DrawAmount();
        EditorGUILayout.Space();

        DrawAllCurrencyButtons(currency);
        EditorGUILayout.Space();

        DrawCurrencyList(currency);
    }

    private void DrawAmount()
    {
        EditorGUILayout.LabelField("지급 수량", EditorStyles.boldLabel);

        _amount = EditorGUILayout.LongField("수량", _amount);

        if (_amount < MIN_AMOUNT)
        {
            _amount = MIN_AMOUNT;
        }

        EditorGUILayout.BeginHorizontal();

        for (int i = 0; i < PRESET_AMOUNTS.Length; i++)
        {
            DrawPresetButton(PRESET_AMOUNTS[i]);
        }

        EditorGUILayout.EndHorizontal();
    }

    private void DrawPresetButton(long amount)
    {
        if (!GUILayout.Button(amount.ToString("N0")))
        {
            return;
        }

        _amount = amount;

        GUI.FocusControl(null);
    }

    private void DrawAllCurrencyButtons(CurrencyModel currency)
    {
        EditorGUILayout.LabelField("일괄", EditorStyles.boldLabel);

        EditorGUI.BeginDisabledGroup(_amount <= 0);

        if (GUILayout.Button($"모든 재화 +{_amount:N0}", GUILayout.Height(28f)))
        {
            AddAll(currency, _amount);
        }

        EditorGUI.EndDisabledGroup();

        if (GUILayout.Button("에너지 가득 채우기"))
        {
            currency.AddEnergy(currency.MaxEnergy);
        }
    }

    private void AddAll(CurrencyModel currency, long amount)
    {
        for (int i = 0; i < CURRENCY_TYPES.Length; i++)
        {
            Add(currency, CURRENCY_TYPES[i], amount);
        }

        Logger.Log($"[CurrencyDebug] 모든 재화 +{amount:N0} 지급");
    }

    private void DrawCurrencyList(CurrencyModel currency)
    {
        EditorGUILayout.LabelField("개별", EditorStyles.boldLabel);

        for (int i = 0; i < CURRENCY_TYPES.Length; i++)
        {
            DrawCurrencyRow(currency, CURRENCY_TYPES[i]);
        }
    }

    private void DrawCurrencyRow(CurrencyModel currency, CurrencyType currencyType)
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(GetDisplayName(currencyType), GetValueText(currency, currencyType));

        EditorGUI.BeginDisabledGroup(_amount <= 0);

        if (GUILayout.Button("+", GUILayout.Width(30f)))
        {
            Add(currency, currencyType, _amount);
        }

        if (GUILayout.Button("-", GUILayout.Width(30f)))
        {
            Spend(currency, currencyType, _amount);
        }

        EditorGUI.EndDisabledGroup();

        EditorGUILayout.EndHorizontal();
    }

    private string GetValueText(CurrencyModel currency, CurrencyType currencyType)
    {
        if (CurrencyType.Energy == currencyType)
        {
            return $"{currency.Energy:N0} / {currency.MaxEnergy:N0}";
        }

        return GetValue(currency, currencyType).ToString("N0");
    }

    private long GetValue(CurrencyModel currency, CurrencyType currencyType)
    {
        switch (currencyType)
        {
            case CurrencyType.Money:
                return currency.Money;

            case CurrencyType.DreamPoint:
                return currency.DreamPoint;

            case CurrencyType.Energy:
                return currency.Energy;

            case CurrencyType.DreamFragment:
                return currency.DreamFragment;

            case CurrencyType.DreamScroll:
                return currency.DreamScroll;

            case CurrencyType.Inspiration:
                return currency.Inspiration;
        }

        return 0;
    }

    private void Add(CurrencyModel currency, CurrencyType currencyType, long amount)
    {
        switch (currencyType)
        {
            case CurrencyType.Money:
                currency.AddMoney(amount);
                break;

            case CurrencyType.DreamPoint:
                currency.AddDreamPoint(amount);
                break;

            case CurrencyType.Energy:
                currency.AddEnergy(amount);
                break;

            case CurrencyType.DreamFragment:
                currency.AddDreamFragment(amount);
                break;

            case CurrencyType.DreamScroll:
                currency.AddDreamScroll(amount);
                break;

            case CurrencyType.Inspiration:
                currency.AddInspiration(amount);
                break;
        }
    }

    private void Spend(CurrencyModel currency, CurrencyType currencyType, long amount)
    {
        long current = GetValue(currency, currencyType);
        long spendAmount = amount < current ? amount : current;

        if (spendAmount <= 0)
        {
            return;
        }

        switch (currencyType)
        {
            case CurrencyType.Money:
                currency.TrySpendMoney(spendAmount);
                break;

            case CurrencyType.DreamPoint:
                currency.TrySpendDreamPoint(spendAmount);
                break;

            case CurrencyType.Energy:
                currency.TrySpendEnergy(spendAmount);
                break;

            case CurrencyType.DreamFragment:
                currency.TrySpendDreamFragment(spendAmount);
                break;

            case CurrencyType.DreamScroll:
                currency.TrySpendDreamScroll(spendAmount);
                break;

            case CurrencyType.Inspiration:
                currency.TrySpendInspiration(spendAmount);
                break;
        }
    }

    private string GetDisplayName(CurrencyType currencyType)
    {
        switch (currencyType)
        {
            case CurrencyType.Money:
                return "돈";

            case CurrencyType.DreamPoint:
                return "드림 포인트";

            case CurrencyType.Energy:
                return "에너지";

            case CurrencyType.DreamFragment:
                return "꿈의 조각";

            case CurrencyType.DreamScroll:
                return "몽상의 스크롤";

            case CurrencyType.Inspiration:
                return "영감";
        }

        return currencyType.ToString();
    }
}
