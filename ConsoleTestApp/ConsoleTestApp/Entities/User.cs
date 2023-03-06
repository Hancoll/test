using ConsoleTestApp.Entities.Common;
using System.ComponentModel;

namespace ConsoleTestApp.Entities;
internal class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string FullName { get; set; } = null!;

    public DateOnly BirthDay { get; set; }

    public Gender Gender { get; set; }
}
