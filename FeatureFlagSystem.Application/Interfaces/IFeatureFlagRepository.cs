using System.Collections.Generic;
using System.Threading.Tasks;
using FeatureFlagSystem.Domain.Entities;

namespace FeatureFlagSystem.Application.Interfaces
{
    /// <summary>
    /// Abstraction for feature flag data access.
    /// Application layer depends on this interface, not concrete implementation.
    /// </summary>
    public interface IFeatureFlagRepository
    {
        /// <summary>
        /// Retrieve a feature flag by its unique name.
        /// </summary>
        /// <param name="featureName">The unique feature name</param>
        /// <returns>FeatureFlag if found, null otherwise</returns>
        Task<FeatureFlag?> GetByNameAsync(string featureName);

        /// <summary>
        /// Retrieve a feature flag by its ID.
        /// </summary>
        /// <param name="id">The feature flag ID</param>
        /// <returns>FeatureFlag if found, null otherwise</returns>
        Task<FeatureFlag?> GetByIdAsync(int id);

        /// <summary>
        /// Retrieve all feature flags.
        /// </summary>
        /// <returns>Collection of all feature flags</returns>
        Task<IEnumerable<FeatureFlag>> GetAllAsync();

        /// <summary>
        /// Add a new feature flag to the repository.
        /// </summary>
        /// <param name="featureFlag">The feature flag to add</param>
        Task AddAsync(FeatureFlag featureFlag);

        /// <summary>
        /// Update an existing feature flag.
        /// </summary>
        /// <param name="featureFlag">The feature flag to update</param>
        Task UpdateAsync(FeatureFlag featureFlag);

        /// <summary>
        /// Delete a feature flag.
        /// </summary>
        /// <param name="featureFlag">The feature flag to delete</param>
        Task DeleteAsync(FeatureFlag featureFlag);

        /// <summary>
        /// Check if a feature flag exists by name.
        /// </summary>
        /// <param name="featureName">The feature name to check</param>
        /// <returns>True if exists, false otherwise</returns>
        Task<bool> ExistsAsync(string featureName);

        /// <summary>
        /// Persist changes to the database.
        /// </summary>
        /// <returns>Number of entities affected</returns>
        Task<int> SaveChangesAsync();
    }
}