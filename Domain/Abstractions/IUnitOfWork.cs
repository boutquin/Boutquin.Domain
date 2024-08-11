// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Boutquin.Domain.Abstractions;

/// <summary>
/// Defines a unit of work that groups one or more operations (such as insertions, updates, deletions)
/// into a single transaction to ensure data integrity and consistency.
/// </summary>
/// <remarks>
/// <para>
/// The IUnitOfWork interface is a key part of the repository pattern and is typically used in the context of domain-driven design.
/// It encapsulates the concept of a "unit of work" that can be used to maintain the integrity of the domain model.
/// </para>
/// <para>
/// Implementing this interface allows for managing transactions, ensuring that all operations either complete successfully as a whole
/// or are all rolled back in case of failure. This helps maintain consistency in the underlying data store.
/// </para>
/// <para>
/// The interface is commonly used in services or business logic layers to define the boundaries of a transaction.
/// </para>
/// </remarks>
/// <example>
/// Example of using the IUnitOfWork interface in a service class:
/// <code>
/// public sealed class MyService
/// {
///     private readonly IRepository&lt;MyEntity&gt; _repository;
///     private readonly IUnitOfWork _unitOfWork;
/// 
///     public MyService(IRepository&lt;MyEntity&gt; repository, IUnitOfWork unitOfWork)
///     {
///         _repository = repository;
///         _unitOfWork = unitOfWork;
///     }
/// 
///     public async Task AddEntityAsync(MyEntity entity, CancellationToken cancellationToken = default)
///     {
///         _repository.Add(entity);
///         await _unitOfWork.SaveChangesAsync(cancellationToken);
///     }
/// }
/// </code>
/// </example>
public interface IUnitOfWork
{
    /// <summary>
    /// Commits all changes made in the current unit of work asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>The number of objects written to the underlying database.</returns>
    /// <remarks>
    /// This method applies any changes made within the unit of work to the database, ensuring atomicity of the operations.
    /// It returns the number of objects affected, which can be useful for understanding the impact of the operation.
    /// </remarks>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
