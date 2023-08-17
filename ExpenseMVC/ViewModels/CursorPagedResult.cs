namespace ExpenseMVC;

public sealed record CursorPagedResult<T>(DateTimeOffset Cursor, T Data);