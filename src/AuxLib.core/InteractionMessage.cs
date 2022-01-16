namespace core.audiamus.aux {
  public enum ECallbackType { info, infoCancel, warning, error, errorQuestion, errorQuestion3, question, question3 }

  public record InteractionMessage (
    ECallbackType Type,
    string Message
  );

  public record InteractionMessage<T> (ECallbackType Type, string Message, T Custom) : 
    InteractionMessage (Type, Message);

}
