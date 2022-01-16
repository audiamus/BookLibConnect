using System;
using System.Threading;

namespace core.audiamus.aux.ex {

  /// <summary>
  ///   <para>Type-safe extensions for invoking methods via a synchronization context. 
  ///   Will be executed in the thread of the synchronization context.</para>
  ///   <list type="bullet">
  ///     <item>Post: Action delegate, not waiting for completion.</item>
  ///     <item>Send without return type: Action delegate, waiting for completion.</item>
  ///     <item>Send with return type: Func delegate, waiting for completion.</item>
  ///   </list>
  ///   <para>If synchronization context is <c>null</c>, delegate will be executed directly.</para>
  /// </summary>
  /// <example><code>
  /// mySyncContext.Send (helloWorld, "Me");
  /// 
  /// void helloWorld (string name) => Console.WriteLine ("Hello world from " + name);
  /// </code></example>
  public static class SyncContextExtensions {
    #region Public Methods

    #region Asynchronous Post Methods
    public static void Post (
      this SynchronizationContext sync, 
      Action delgat
    ) {
      if (sync is null)
        delgat ();
      else
        sendOrPost (sync.Post, delgat);
    }

    public static void Post<T> (
      this SynchronizationContext sync, 
      Action<T> delgat, 
      T p1
    ) {
      if (sync is null)
        delgat (p1);
      else
        sendOrPost (sync.Post, delgat, p1);
    }

    public static void Post<T1, T2> (
      this SynchronizationContext sync, 
      Action<T1, T2> delgat, 
      T1 p1, T2 p2
    ) {
      if (sync is null)
        delgat (p1, p2);
      else
        sendOrPost (sync.Post, delgat, p1, p2);
    }

    public static void Post<T1, T2, T3> (
      this SynchronizationContext sync, 
      Action<T1, T2, T3> delgat, 
      T1 p1, T2 p2, T3 p3
    ) {
      if (sync is null)
        delgat (p1, p2, p3);
      else
        sendOrPost (sync.Post, delgat, p1, p2, p3);
    }

    public static void Post<T1, T2, T3, T4> (
      this SynchronizationContext sync, 
      Action<T1, T2, T3, T4> delgat, 
      T1 p1, T2 p2, T3 p3, T4 p4
    ) {
      if (sync is null)
        delgat (p1, p2, p3, p4);
      else
        sendOrPost (sync.Post, delgat, p1, p2, p3, p4);
    }

    public static void Post<T1, T2, T3, T4, T5> (
      this SynchronizationContext sync, 
      Action<T1, T2, T3, T4, T5> delgat, 
      T1 p1, T2 p2, T3 p3, T4 p4, T5 p5
    ) {
      if (sync is null)
        delgat (p1, p2, p3, p4, p5);
      else
        sendOrPost (sync.Post, delgat, p1, p2, p3, p4, p5);
    }

    public static void Post<T1, T2, T3, T4, T5, T6> (
      this SynchronizationContext sync, 
      Action<T1, T2, T3, T4, T5, T6> delgat, 
      T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6
    ) {
      if (sync is null)
        delgat (p1, p2, p3, p4, p5, p6);
      else
        sendOrPost (sync.Post, delgat, p1, p2, p3, p4, p5, p6);
    }

    public static void Post<T1, T2, T3, T4, T5, T6, T7> (
      this SynchronizationContext sync, 
      Action<T1, T2, T3, T4, T5, T6, T7> delgat, 
      T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7
    ) {
      if (sync is null)
        delgat (p1, p2, p3, p4, p5, p6, p7);
      else
        sendOrPost (sync.Post, delgat, p1, p2, p3, p4, p5, p6, p7);
    }

    public static void Post<T1, T2, T3, T4, T5, T6, T7, T8> (
      this SynchronizationContext sync, 
      Action<T1, T2, T3, T4, T5, T6, T7, T8> delgat, 
      T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8
    ) {
      if (sync is null)
        delgat (p1, p2, p3, p4, p5, p6, p7, p8);
      else
        sendOrPost (sync.Post, delgat, p1, p2, p3, p4, p5, p6, p7, p8);
    }
    #endregion Asynchronous Post Methods

    #region Synchronous Send Methods
    public static void Send (
      this SynchronizationContext sync, 
      Action delgat
    ) {
      if (sync is null)
        delgat ();
      else
        sendOrPost (sync.Send, delgat);
    }

    public static void Send<T> (
      this SynchronizationContext sync, 
      Action<T> delgat, 
      T p1
    ) {
      if (sync is null)
        delgat (p1);
      else
        sendOrPost (sync.Send, delgat, p1);
    }

    public static void Send<T1, T2> (
      this SynchronizationContext sync, 
      Action<T1, T2> delgat, 
      T1 p1, T2 p2
    ) {
      if (sync is null)
        delgat (p1, p2);
      else
        sendOrPost (sync.Send, delgat, p1, p2);
    }

    public static void Send<T1, T2, T3> (
      this SynchronizationContext sync, 
      Action<T1, T2, T3> delgat, 
      T1 p1, T2 p2, T3 p3
    ) {
      if (sync is null)
        delgat (p1, p2, p3);
      else
        sendOrPost (sync.Send, delgat, p1, p2, p3);
    }

    public static void Send<T1, T2, T3, T4> (
      this SynchronizationContext sync, 
      Action<T1, T2, T3, T4> delgat, 
      T1 p1, T2 p2, T3 p3, T4 p4
    ) {
      if (sync is null)
        delgat (p1, p2, p3, p4);
      else
        sendOrPost (sync.Send, delgat, p1, p2, p3, p4);
    }

    public static void Send<T1, T2, T3, T4, T5> (
      this SynchronizationContext sync, 
      Action<T1, T2, T3, T4, T5> delgat, 
      T1 p1, T2 p2, T3 p3, T4 p4, T5 p5
    ) {
      if (sync is null)
        delgat (p1, p2, p3, p4, p5);
      else
        sendOrPost (sync.Send, delgat, p1, p2, p3, p4, p5);
    }

    public static void Send<T1, T2, T3, T4, T5, T6> (
      this SynchronizationContext sync, 
      Action<T1, T2, T3, T4, T5, T6> delgat, 
      T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6
    ) {
      if (sync is null)
        delgat (p1, p2, p3, p4, p5, p6);
      else
        sendOrPost (sync.Send, delgat, p1, p2, p3, p4, p5, p6);
    }

    public static void Send<T1, T2, T3, T4, T5, T6, T7> (
      this SynchronizationContext sync, 
      Action<T1, T2, T3, T4, T5, T6, T7> delgat, 
      T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7
    ) {
      if (sync is null)
        delgat (p1, p2, p3, p4, p5, p6, p7);
      else
        sendOrPost (sync.Send, delgat, p1, p2, p3, p4, p5, p6, p7);
    }

    public static void Send<T1, T2, T3, T4, T5, T6, T7, T8> (
      this SynchronizationContext sync, 
      Action<T1, T2, T3, T4, T5, T6, T7, T8> delgat, 
      T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8
    ) {
      if (sync is null)
        delgat (p1, p2, p3, p4, p5, p6, p7, p8);
      else
        sendOrPost (sync.Send, delgat, p1, p2, p3, p4, p5, p6, p7, p8);
    }
    #endregion Synchronous Send Methods

    #region Synchronous Send Methods With Return Value
    public static TResult Send<TResult> (
      this SynchronizationContext sync, 
      Func<TResult> delgat
    ) {
      if (sync is null)
        return delgat ();
      else
        return sendOrPost (sync.Send, delgat);
    }

    public static TResult Send<T, TResult> (
      this SynchronizationContext sync, 
      Func<T, TResult> delgat, 
      T p1
    ) {
      if (sync is null)
        return delgat (p1);
      else
        return sendOrPost (sync.Send, delgat, p1);
    }

    public static TResult Send<T1, T2, TResult> (
      this SynchronizationContext sync, 
      Func<T1, T2, TResult> delgat, 
      T1 p1, T2 p2
    ) {
      if (sync is null)
        return delgat (p1, p2);
      else
        return sendOrPost (sync.Send, delgat, p1, p2);
    }

    public static TResult Send<T1, T2, T3, TResult> (
      this SynchronizationContext sync, 
      Func<T1, T2, T3, TResult> delgat, 
      T1 p1, T2 p2, T3 p3
    ) {
      if (sync is null)
        return delgat (p1, p2, p3);
      else
        return sendOrPost (sync.Send, delgat, p1, p2, p3);
    }

    public static TResult Send<T1, T2, T3, T4, TResult> (
      this SynchronizationContext sync, 
      Func<T1, T2, T3, T4, TResult> delgat, 
      T1 p1, T2 p2, T3 p3, T4 p4
    ) {
      if (sync is null)
        return delgat (p1, p2, p3, p4);
      else
        return sendOrPost (sync.Send, delgat, p1, p2, p3, p4);
    }

    public static TResult Send<T1, T2, T3, T4, T5, TResult> (
      this SynchronizationContext sync, 
      Func<T1, T2, T3, T4, T5, TResult> delgat, 
      T1 p1, T2 p2, T3 p3, T4 p4, T5 p5
    ) {
      if (sync is null)
        return delgat (p1, p2, p3, p4, p5);
      else
        return sendOrPost (sync.Send, delgat, p1, p2, p3, p4, p5);
    }

    public static TResult Send<T1, T2, T3, T4, T5, T6, TResult> (
      this SynchronizationContext sync, 
      Func<T1, T2, T3, T4, T5, T6, TResult> delgat, 
      T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6
    ) {
      if (sync is null)
        return delgat (p1, p2, p3, p4, p5, p6);
      else
        return sendOrPost (sync.Send, delgat, p1, p2, p3, p4, p5, p6);
    }

    public static TResult Send<T1, T2, T3, T4, T5, T6, T7, TResult> (
      this SynchronizationContext sync, 
      Func<T1, T2, T3, T4, T5, T6, T7, TResult> delgat, 
      T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7
    ) {
      if (sync is null)
        return delgat (p1, p2, p3, p4, p5, p6, p7);
      else
        return sendOrPost (sync.Send, delgat, p1, p2, p3, p4, p5, p6, p7);
    }

    public static TResult Send<T1, T2, T3, T4, T5, T6, T7, T8, TResult> (
      this SynchronizationContext sync, 
      Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> delgat, 
      T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8
    ) {
      if (sync is null)
        return delgat (p1, p2, p3, p4, p5, p6, p7, p8);
      else
        return sendOrPost (sync.Send, delgat, p1, p2, p3, p4, p5, p6, p7, p8);
    }
    #endregion Synchronous Send Methods With Return Value

    #endregion Public Methods



    #region Private Methods

    private static void sendOrPost (
      Action<SendOrPostCallback, object> sendOrPost, 
      Action delgat
    ) {
      sendOrPost (o => delgat (), null);
    }

    private static void sendOrPost<T1> (
      Action<SendOrPostCallback, object> sendOrPost,
      Action<T1> delgat,
      T1 p1
    ) {
      sendOrPost (o => delgat ((T1)o), p1);
    }

    private static void sendOrPost<T1, T2> (
      Action<SendOrPostCallback, object> sendOrPost,
      Action<T1, T2> delgat,
      T1 p1, T2 p2
    ) {
      sendOrPost (o =>
      {
        var p = o as object[];
        delgat ((T1)p[0], (T2)p[1]);
      }, new object[] { p1, p2 });
    }

    private static void sendOrPost<T1, T2, T3> (
      Action<SendOrPostCallback, object> sendOrPost,
      Action<T1, T2, T3> delgat,
      T1 p1, T2 p2, T3 p3
    ) {
      sendOrPost (o =>
      {
        var p = o as object[];
        delgat ((T1)p[0], (T2)p[1], (T3)p[2]);
      }, new object[] { p1, p2, p3 });
    }

    private static void sendOrPost<T1, T2, T3, T4> (
      Action<SendOrPostCallback, object> sendOrPost,
      Action<T1, T2, T3, T4> delgat,
      T1 p1, T2 p2, T3 p3, T4 p4
    ) {
      sendOrPost (o =>
      {
        var p = o as object[];
        delgat ((T1)p[0], (T2)p[1], (T3)p[2], (T4)p[3]);
      }, new object[] { p1, p2, p3, p4 });
    }

    private static void sendOrPost<T1, T2, T3, T4, T5> (
      Action<SendOrPostCallback, object> sendOrPost,
      Action<T1, T2, T3, T4, T5> delgat,
      T1 p1, T2 p2, T3 p3, T4 p4, T5 p5
    ) {
      sendOrPost (o =>
      {
        var p = o as object[];
        delgat ((T1)p[0], (T2)p[1], (T3)p[2], (T4)p[3], (T5)p[4]);
      }, new object[] { p1, p2, p3, p4, p5 });
    }

    private static void sendOrPost<T1, T2, T3, T4, T5, T6> (
      Action<SendOrPostCallback, object> sendOrPost, 
      Action<T1, T2, T3, T4, T5, T6> delgat, 
      T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6
    ) {
      sendOrPost (o =>
      {
        var p = o as object[];
        delgat ((T1)p[0], (T2)p[1], (T3)p[2], (T4)p[3], (T5)p[4], (T6)p[5]);
      }, new object[] { p1, p2, p3, p4, p5, p6 });
    }

    private static void sendOrPost<T1, T2, T3, T4, T5, T6, T7> (
      Action<SendOrPostCallback, object> sendOrPost, 
      Action<T1, T2, T3, T4, T5, T6, T7> delgat, 
      T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7
    ) {
      sendOrPost (o =>
      {
        var p = o as object[];
        delgat ((T1)p[0], (T2)p[1], (T3)p[2], (T4)p[3], (T5)p[4], (T6)p[5], (T7)p[6]);
      }, new object[] { p1, p2, p3, p4, p5, p6, p7 });
    }

    private static void sendOrPost<T1, T2, T3, T4, T5, T6, T7, T8> (
      Action<SendOrPostCallback, object> sendOrPost, 
      Action<T1, T2, T3, T4, T5, T6, T7, T8> delgat, 
      T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8
    ) {
      sendOrPost (o =>
      {
        var p = o as object[];
        delgat ((T1)p[0], (T2)p[1], (T3)p[2], (T4)p[3], (T5)p[4], (T6)p[5], (T7)p[6], (T8)p[7]);
      }, new object[] { p1, p2, p3, p4, p5, p6, p7, p8 });
    }

    private static TResult sendOrPost<TResult> (
      Action<SendOrPostCallback, object> sendOrPost, 
      Func<TResult> delgat
    ) {
      TResult retval = default;
      sendOrPost (o => retval = delgat (), null);
      return retval;
    }

    private static TResult sendOrPost<T1, TResult> (
      Action<SendOrPostCallback, object> sendOrPost,
      Func<T1, TResult> delgat,
      T1 p1
    ) {
      TResult retval = default;
      sendOrPost (o => retval = delgat ((T1)o), p1);
      return retval;
    }

    private static TResult sendOrPost<T1, T2, TResult> (
      Action<SendOrPostCallback, object> sendOrPost,
      Func<T1, T2, TResult> delgat,
      T1 p1, T2 p2
    ) {
      TResult retval = default;
      sendOrPost (o =>
      {
        var p = o as object[];
        retval = delgat ((T1)p[0], (T2)p[1]);
      }, new object[] { p1, p2 });
      return retval;
    }

    private static TResult sendOrPost<T1, T2, T3, TResult> (
      Action<SendOrPostCallback, object> sendOrPost,
      Func<T1, T2, T3, TResult> delgat,
      T1 p1, T2 p2, T3 p3
    ) {
      TResult retval = default;
      sendOrPost (o =>
      {
        var p = o as object[];
        retval = delgat ((T1)p[0], (T2)p[1], (T3)p[2]);
      }, new object[] { p1, p2, p3 });
      return retval;
    }

    private static TResult sendOrPost<T1, T2, T3, T4, TResult> (
      Action<SendOrPostCallback, object> sendOrPost,
      Func<T1, T2, T3, T4, TResult> delgat,
      T1 p1, T2 p2, T3 p3, T4 p4
    ) {
      TResult retval = default;
      sendOrPost (o =>
      {
        var p = o as object[];
        retval = delgat ((T1)p[0], (T2)p[1], (T3)p[2], (T4)p[3]);
      }, new object[] { p1, p2, p3, p4 });
      return retval;
    }

    private static TResult sendOrPost<T1, T2, T3, T4, T5, TResult> (
      Action<SendOrPostCallback, object> sendOrPost,
      Func<T1, T2, T3, T4, T5, TResult> delgat,
      T1 p1, T2 p2, T3 p3, T4 p4, T5 p5
    ) {
      TResult retval = default;
      sendOrPost (o =>
      {
        var p = o as object[];
        retval = delgat ((T1)p[0], (T2)p[1], (T3)p[2], (T4)p[3], (T5)p[4]);
      }, new object[] { p1, p2, p3, p4, p5 });
      return retval;
    }

    private static TResult sendOrPost<T1, T2, T3, T4, T5, T6, TResult> (
      Action<SendOrPostCallback, object> sendOrPost,
      Func<T1, T2, T3, T4, T5, T6, TResult> delgat,
      T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6
    ) {
      TResult retval = default;
      sendOrPost (o =>
      {
        var p = o as object[];
        retval = delgat ((T1)p[0], (T2)p[1], (T3)p[2], (T4)p[3], (T5)p[4], (T6)p[5]);
      }, new object[] { p1, p2, p3, p4, p5, p6 });
      return retval;
    }

    private static TResult sendOrPost<T1, T2, T3, T4, T5, T6, T7, TResult> (
      Action<SendOrPostCallback, object> sendOrPost,
      Func<T1, T2, T3, T4, T5, T6, T7, TResult> delgat,
      T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7
    ) {
      TResult retval = default;
      sendOrPost (o =>
      {
        var p = o as object[];
        retval = delgat ((T1)p[0], (T2)p[1], (T3)p[2], (T4)p[3], (T5)p[4], (T6)p[5], (T7)p[6]);
      }, new object[] { p1, p2, p3, p4, p5, p6, p7 });
      return retval;
    }

    private static TResult sendOrPost<T1, T2, T3, T4, T5, T6, T7, T8, TResult> (
      Action<SendOrPostCallback, object> sendOrPost,
      Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> delgat,
      T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8
    ) {
      TResult retval = default;
      sendOrPost (o =>
      {
        var p = o as object[];
        retval = delgat ((T1)p[0], (T2)p[1], (T3)p[2], (T4)p[3], (T5)p[4], (T6)p[5], (T7)p[6], (T8)p[7]);
      }, new object[] { p1, p2, p3, p4, p5, p6, p7, p8 });
      return retval;
    }

    #endregion Private Methods
  }
}
