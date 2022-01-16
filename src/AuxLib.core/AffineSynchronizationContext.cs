using System;
using System.Threading;
using core.audiamus.aux.ex;

namespace core.audiamus.aux {
  public class AffineSynchronizationContext {

#region Private Fields
    private readonly SynchronizationContext _sync;
    private readonly int _managedThreadId;
    
    #endregion Private Fields
    #region Private Props
    private bool Affine => 
      _managedThreadId == Thread.CurrentThread.ManagedThreadId;
    #endregion Private Props
    #region ctor

    public AffineSynchronizationContext () {
      _sync = SynchronizationContext.Current;
      _managedThreadId = Thread.CurrentThread.ManagedThreadId;
    }
    #endregion ctor

    #region Public Methods
    #region Asynchronous Send Methods With Return Value
    public void Post (
      Action delgat
    ) {
      if (Affine)
        delgat ();
      else
        _sync.Post (delgat);
    }

    public void Post<T> (
      Action<T> delgat,
      T p
    ) {
      if (Affine)
        delgat (p);
      else
        _sync.Post (delgat, p);
    }

    public void Post<T1, T2> (
      Action<T1, T2> delgat,
      T1 p1, T2 p2
    ) {
      if (Affine)
        delgat (p1, p2);
      else
        _sync.Post (delgat, p1, p2);
    }

    public void Post<T1, T2, T3> (
      Action<T1, T2, T3> delgat,
      T1 p1, T2 p2, T3 p3
    ) {
      if (Affine)
        delgat (p1, p2, p3);
      else
        _sync.Post (delgat, p1, p2, p3);
    }

    public void Post<T1, T2, T3, T4> (
      Action<T1, T2, T3, T4> delgat,
      T1 p1, T2 p2, T3 p3, T4 p4
    ) {
      if (Affine)
        delgat (p1, p2, p3, p4);
      else
        _sync.Post (delgat, p1, p2, p3, p4);
    }

    public void Post<T1, T2, T3, T4, T5> (
      Action<T1, T2, T3, T4, T5> delgat,
      T1 p1, T2 p2, T3 p3, T4 p4, T5 p5
    ) {
      if (Affine)
        delgat (p1, p2, p3, p4, p5);
      else
        _sync.Post (delgat, p1, p2, p3, p4, p5);
    }

    public void Post<T1, T2, T3, T4, T5, T6> (
      Action<T1, T2, T3, T4, T5, T6> delgat,
      T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6
    ) {
      if (Affine)
        delgat (p1, p2, p3, p4, p5, p6);
      else
        _sync.Post (delgat, p1, p2, p3, p4, p5, p6);
    }

    public void Post<T1, T2, T3, T4, T5, T6, T7> (
      Action<T1, T2, T3, T4, T5, T6, T7> delgat,
      T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7
    ) {
      if (Affine)
        delgat (p1, p2, p3, p4, p5, p6, p7);
      else
        _sync.Post (delgat, p1, p2, p3, p4, p5, p6, p7);
    }

    public void Post<T1, T2, T3, T4, T5, T6, T7, T8> (
      Action<T1, T2, T3, T4, T5, T6, T7, T8> delgat,
      T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8
    ) {
      if (Affine)
        delgat (p1, p2, p3, p4, p5, p6, p7, p8);
      else
        _sync.Post (delgat, p1, p2, p3, p4, p5, p6, p7, p8);
    }
    #endregion Asynchronous Send Methods With Return Value

    #region Synchronous Send Methods With Return Value
    public void Send (
      Action delgat
    ) {
      if (Affine)
        delgat ();
      else
        _sync.Send (delgat);
    }

    public void Send<T> (
      Action<T> delgat,
      T p
    ) {
      if (Affine)
        delgat (p);
      else
        _sync.Send (delgat, p);
    }

    public void Send<T1, T2> (
      Action<T1, T2> delgat,
      T1 p1, T2 p2
    ) {
      if (Affine)
        delgat (p1, p2);
      else
        _sync.Send (delgat, p1, p2);
    }

    public void Send<T1, T2, T3> (
      Action<T1, T2, T3> delgat,
      T1 p1, T2 p2, T3 p3
    ) {
      if (Affine)
        delgat (p1, p2, p3);
      else
        _sync.Send (delgat, p1, p2, p3);
    }

    public void Send<T1, T2, T3, T4> (
      Action<T1, T2, T3, T4> delgat,
      T1 p1, T2 p2, T3 p3, T4 p4
    ) {
      if (Affine)
        delgat (p1, p2, p3, p4);
      else
        _sync.Send (delgat, p1, p2, p3, p4);
    }

    public void Send<T1, T2, T3, T4, T5> (
      Action<T1, T2, T3, T4, T5> delgat,
      T1 p1, T2 p2, T3 p3, T4 p4, T5 p5
    ) {
      if (Affine)
        delgat (p1, p2, p3, p4, p5);
      else
        _sync.Send (delgat, p1, p2, p3, p4, p5);
    }

    public void Send<T1, T2, T3, T4, T5, T6> (
      Action<T1, T2, T3, T4, T5, T6> delgat,
      T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6
    ) {
      if (Affine)
        delgat (p1, p2, p3, p4, p5, p6);
      else
        _sync.Send (delgat, p1, p2, p3, p4, p5, p6);
    }

    public void Send<T1, T2, T3, T4, T5, T6, T7> (
      Action<T1, T2, T3, T4, T5, T6, T7> delgat,
      T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7
    ) {
      if (Affine)
        delgat (p1, p2, p3, p4, p5, p6, p7);
      else
        _sync.Send (delgat, p1, p2, p3, p4, p5, p6, p7);
    }

    public void Send<T1, T2, T3, T4, T5, T6, T7, T8> (
      Action<T1, T2, T3, T4, T5, T6, T7, T8> delgat,
      T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8
    ) {
      if (Affine)
        delgat (p1, p2, p3, p4, p5, p6, p7, p8);
      else
        _sync.Send (delgat, p1, p2, p3, p4, p5, p6, p7, p8);
    }
    #endregion Synchronous Send Methods With Return Value

    #region Synchronous Send Methods With Return Value
    public TResult Send<TResult> (
      Func<TResult> delgat
    ) {
      if (Affine)
        return delgat ();
      else
        return _sync.Send (delgat);
    }

    public TResult Send<T, TResult> (
      Func<T, TResult> delgat,
      T p
    ) {
      if (Affine)
        return delgat (p);
      else
        return _sync.Send (delgat, p);
    }

    public TResult Send<T1, T2, TResult> (
      Func<T1, T2, TResult> delgat,
      T1 p1, T2 p2
    ) {
      if (Affine)
        return delgat (p1, p2);
      else
        return _sync.Send (delgat, p1, p2);
    }

    public TResult Send<T1, T2, T3, TResult> (
      Func<T1, T2, T3, TResult> delgat,
      T1 p1, T2 p2, T3 p3
    ) {
      if (Affine)
        return delgat (p1, p2, p3);
      else
        return _sync.Send (delgat, p1, p2, p3);
    }

    public TResult Send<T1, T2, T3, T4, TResult> (
      Func<T1, T2, T3, T4, TResult> delgat,
      T1 p1, T2 p2, T3 p3, T4 p4
    ) {
      if (Affine)
        return delgat (p1, p2, p3, p4);
      else
        return _sync.Send (delgat, p1, p2, p3, p4);
    }

    public TResult Send<T1, T2, T3, T4, T5, TResult> (
      Func<T1, T2, T3, T4, T5, TResult> delgat,
      T1 p1, T2 p2, T3 p3, T4 p4, T5 p5
    ) {
      if (Affine)
        return delgat (p1, p2, p3, p4, p5);
      else
        return _sync.Send (delgat, p1, p2, p3, p4, p5);
    }

    public TResult Send<T1, T2, T3, T4, T5, T6, TResult> (
      Func<T1, T2, T3, T4, T5, T6, TResult> delgat,
      T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6
    ) {
      if (Affine)
        return delgat (p1, p2, p3, p4, p5, p6);
      else
        return _sync.Send (delgat, p1, p2, p3, p4, p5, p6);
    }

    public TResult Send<T1, T2, T3, T4, T5, T6, T7, TResult> (
      Func<T1, T2, T3, T4, T5, T6, T7, TResult> delgat,
      T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7
    ) {
      if (Affine)
        return delgat (p1, p2, p3, p4, p5, p6, p7);
      else
        return _sync.Send (delgat, p1, p2, p3, p4, p5, p6, p7);
    }

    public TResult Send<T1, T2, T3, T4, T5, T6, T7, T8, TResult> (
      Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> delgat,
      T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8
    ) {
      if (Affine)
        return delgat (p1, p2, p3, p4, p5, p6, p7, p8);
      else
        return _sync.Send (delgat, p1, p2, p3, p4, p5, p6, p7, p8);
    }
    #endregion Synchronous Send Methods With Return Value

    #endregion Public Methods
  }
}
