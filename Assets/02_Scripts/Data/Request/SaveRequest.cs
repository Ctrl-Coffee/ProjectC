using Cysharp.Threading.Tasks;
using System;

public class SaveRequest
{
    private readonly Func<UniTask> _saveAsync;

    private bool _isSaving;
    private bool _isPending;

    public SaveRequest(Func<UniTask> saveAsync)
    {
        _saveAsync = saveAsync;
    }

    public void Request()
    {
        _isPending = true;

        if (_isSaving)
        {
            return;
        }

        ProcessAsync().Forget();
    }

    private async UniTask ProcessAsync()
    {
        _isSaving = true;

        try
        {
            while (_isPending)
            {
                _isPending = false;

                try
                {
                    await _saveAsync();
                }
                catch (Exception exception)
                {
                    Logger.LogWarning(exception.Message);
                }
            }
        }
        finally
        {
            _isSaving = false;
        }
    }

}
