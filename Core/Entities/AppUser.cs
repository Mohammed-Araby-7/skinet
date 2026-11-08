using Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace Core.Enitites;


public class AppUser : IdentityUser
{
    public string? FirstName {get;set;}
    public string? LastName {get;set;}
    public Address? Address {get;set;}
}