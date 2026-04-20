namespace NMKApp.Core;

/// <summary>
/// Generic result wrapper for service operations.
/// Replaces NMK_M_Return{T}.
/// </summary>
public class Result<T>
{
  public bool Success { get; init; }
  public T? Data { get; init; }
  public string? Error { get; init; }

  public static Result<T> Ok(T data) => new() { Success = true, Data = data };
  public static Result<T> Fail(string error) => new() { Success = false, Error = error };
}
