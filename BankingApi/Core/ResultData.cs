namespace BankingApi.Core;

public abstract record ResultData<T>(
   int? Status = 0,
   string Message = "",
   T? Data = null
) where T : class?;

public record Success<T>(int? Status, T? Data) 
   : ResultData<T>(Status, "", Data)
   where T : class?;

public record Error<T>(int? Status, string Message) 
   : ResultData<T>(Status, Message, null)
   where T : class?;