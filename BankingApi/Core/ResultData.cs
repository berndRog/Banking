namespace BankingApi.Core; 
public abstract class ResultData<T>(
   int?   status  = 0,
   string message = "",
   T? data = null
) where T : class? {
   public int?   Status  { get; } = status;
   public string Message { get; } = message;
   public T?     Data    { get; } = data;
}

public class Success<T>(int? status, T? data) : ResultData<T>(status: status, data: data)
   where T : class?;

public class Error<T>(int? status, string error) : ResultData<T>(status: status, message: error)
   where T : class?;

public class Loading<T>(string message) : ResultData<T>(message: message)
   where T : class?;