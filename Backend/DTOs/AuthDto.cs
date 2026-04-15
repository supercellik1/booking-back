namespace Backend.DTOs;

public record RegisterRequest(string Email, string PasswordHash, string FullName);
public record LoginRequest(string Email, string PasswordHash);
public record AuthResponse(string Token, UserDto User);
public record UserDto(Guid Id, string Email, string FullName);
