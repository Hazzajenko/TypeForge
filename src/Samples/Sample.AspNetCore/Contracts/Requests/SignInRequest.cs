namespace Sample.AspNetCore.Contracts.Requests;

public class SignInRequest
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
}
