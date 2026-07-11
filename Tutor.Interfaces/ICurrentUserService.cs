namespace Tutor.Data.Interfaces
{
    /// <summary>
    /// Per-request identity of the logged-in user, resolved from JWT claims.
    /// </summary>
    public interface ICurrentUserService
    {
        string? Email { get; }
        string? DisplayName { get; }
        int? TeacherId { get; }
        int? StudentId { get; }
        int? AccountHolderId { get; }
        bool IsAuthenticated { get; }

        /// <summary>Throws InvalidOperationException if the token has no teacher identity.</summary>
        int RequireTeacherId();

        /// <summary>Throws InvalidOperationException if the token has no student identity.</summary>
        int RequireStudentId();

        /// <summary>Throws InvalidOperationException if the token has no account holder identity.</summary>
        int RequireAccountHolderId();
    }
}
