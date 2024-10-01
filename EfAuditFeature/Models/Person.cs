using System.ComponentModel.DataAnnotations;

namespace EfAuditFeathre.Models;

public class Person : ISoftDeletable
{
    [Key]
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
}

public interface ISoftDeletable
{
    public bool IsDeleted  { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
}
