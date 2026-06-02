using Helpdesk.Core.Entities;

namespace Helpdesk.Core.Repositories;

/// <summary>Persistence contract for <see cref="User"/> accounts (admin/agent).</summary>
public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Looks up a user by email (the login identifier). Null if not found.</summary>
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);

    Task<IReadOnlyList<User>> ListAsync(CancellationToken ct = default);

    /// <summary>Stages a new user for insertion. Call <see cref="SaveChangesAsync"/> to persist.</summary>
    Task AddAsync(User user, CancellationToken ct = default);

    Task SaveChangesAsync(CancellationToken ct = default);
}
