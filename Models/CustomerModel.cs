using System;

public class Customer
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Phone { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}
