using Microsoft.AspNetCore.Mvc;
using Xunit;
namespace BankingApiTest.Controllers;

public static class THelper {
   
   // HttpStatusCode.Ok (200)
   public static void IsOk<T>(
      ActionResult<T?> actionResult, 
      T expected
   ) where T : class? {
      // Check if actionResult is not null
      Assert.NotNull(actionResult);
      
      // Check if actionResult! is of type OkObjectResult
      // and evaluate the result
      var(success, result, value) =  
         EvalActionResult<OkObjectResult, T>(actionResult!);
      // Check if success is true
      Assert.True(success);
      // Check if result.StatusCode is 200
      Assert.Equal(200, result.StatusCode);
      Assert.NotNull(value);      
      // Check if value is equivalent to expected
      Assert.Equivalent(expected, value);
   }
   
   // HttpStatusCode.Ok (200)
   public static void IsEnumerableOk<T>(
      ActionResult<T> actionResult, 
      T expected
   ) where T : class {
      // Check if actionResult is not null
      Assert.NotNull(actionResult);
      
      // Check if actionResult! is of type OkObjectResult
      // and evaluate the result
      var(success, result, value) =  
         EvalActionResult<OkObjectResult, T>(actionResult!);
      // Check if success is true
      Assert.True(success);
      // Check if result.StatusCode is 200
      Assert.Equal(200, result.StatusCode);
      Assert.Equivalent(expected, value);
   }
   
   // HttpStatusCode.Created (201)
   public static void IsCreated<T>(
      ActionResult<T?> actionResult, 
      T? expected
   )  where T : class {
      // Check if actionResult is not null
      Assert.NotNull(actionResult);
      
      // Check if actionResult! is of type CreatedResult
      // and evaluate the result
      var(success, result, value) = 
         EvalActionResult<CreatedResult, T?>(actionResult);
      
      // Check if success is true
      Assert.True(success);
      
      // Check if result.StatusCode is 201
      Assert.Equal(201, result.StatusCode);
      
      // Check if value is not null and is equivalent to expected
      Assert.NotNull(value);
      Assert.Equivalent(expected, value);
   }
   
   private static (bool, T, S) EvalActionResult<T, S>(
      ActionResult<S?> actionResult
   ) where T : ObjectResult   // OkObjectResult 
      where S : class? {       // OwnerDto
      
      // Check if actionResult is of type ObjectResult
      Assert.NotNull(actionResult);
      Assert.IsType<T>(actionResult.Result);
      // and cast it to ObjectResult
      var result = (actionResult.Result as T)!; 
      
      // Check if value is not null
      Assert.NotNull(result.Value);

      // and result.Value is of Type s, then cast it to S
      if (result.Value is S resultValue) {
         // return true and result:T and resultValue:S
         return (true, result, resultValue); 
      }
      // return false and result:T and default(S)
      return (false, result, default!);
   }

}