using System;
using System.Collections.Generic;

namespace TextRender;

public class DisposableManager : IDisposable
{
    List<IDisposable> _disposables = new List<IDisposable>(); 
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            for (int index = 0; index < _disposables.Count; index++)
            {
                _disposables[index].Dispose();
            }
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }



    protected void RemoveAndDispose<T>(ref T disposable) where T : IDisposable
    {
        _disposables.Remove(disposable);
        disposable.Dispose();
    }

    protected T AddDisposable<T>(T child) where T : IDisposable
    {
        _disposables.Add(child);
        return child;
    }
}
