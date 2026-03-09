using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeatureFlagSystem.Domain
{
    public class FeatureFlag
    {
        // Private setters = immutability from outside
        public int Id { get; private set; }
        public string FeatureName { get; private set; }
        public string? Description { get; private set; }
        public bool IsEnabled { get; private set; }
        public int RolloutPercentage { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        // EF Core requires parameterless constructor
        private FeatureFlag() { }

        // Factory method pattern - controlled creation
        public static FeatureFlag Create(string featureName, string? description = null, bool isEnabled = false, int rolloutPercentage = 0)
        {
            // Business rule: Feature name is required
            if (string.IsNullOrWhiteSpace(featureName))
                throw new ArgumentException("Feature name cannot be empty.", nameof(featureName));
            // Business rule: Feature name format
            if (featureName.Length > 100)
                throw new ArgumentException("Feature name cannot exceed 100 characters", nameof(featureName));
            // Business rule: Rollout percentage must be 0-100
            if (rolloutPercentage < 0 || rolloutPercentage > 100)
                throw new ArgumentOutOfRangeException(nameof(rolloutPercentage), "Rollout percentage must be between 0 and 100.");

            var now = DateTime.UtcNow;

            return new FeatureFlag
            {
                FeatureName = featureName,
                Description = description,
                IsEnabled = isEnabled,
                RolloutPercentage = rolloutPercentage,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }
        // Business operation: Enable feature
        public void Enable()
        {
            if (IsEnabled)
                return; //Idempotent - Already enabled

            IsEnabled = true;
            UpdatedAt = DateTime.UtcNow;
        }
        // Business operation: Enable feature
        public void Disable()
        {
            if (!IsEnabled)
                return; //Idempotent - Already disabled

            IsEnabled = false;
            UpdatedAt = DateTime.UtcNow;
        }
        // Business operation: Toggle feature
        public void Toggle()
        {
            IsEnabled = !IsEnabled;
            UpdatedAt = DateTime.UtcNow;
        }
        // Business operation: Update rollout percentage
        public void SetRolloutPercentage(int percentage)
        {
            // Business rule validation
            if (percentage < 0 || percentage > 100)
                throw new ArgumentOutOfRangeException(nameof(percentage), "Rollout percentage must be between 0 and 100.");

            if (RolloutPercentage == percentage)
                return; // No change needed

            RolloutPercentage = percentage;
            UpdatedAt = DateTime.UtcNow;
        }
        // Business operation: Update description
        public void UpdateDescription(string? description)
        {
            Description = description?.Trim();
            UpdatedAt = DateTime.UtcNow;
        }
        //Core business logic: Determine if feature is enabled for a user
        // Core business logic: Determine if feature is enabled for a user
        public bool IsEnabledForUser(string userId)
        {
            // Rule 1: Kill switch - if globally disabled, nobody gets it
            if (!IsEnabled)
                return false;

            // Rule 2: 100% rollout - everyone gets it
            if (RolloutPercentage == 100)
                return true;

            // Rule 3: 0% rollout - nobody gets it (even if IsEnabled = true)
            if (RolloutPercentage == 0)
                return false;

            // Rule 4: Deterministic hashing for consistent user experience
            return IsUserInRolloutBucket(userId);
        }

        //Private helper: Deterministic bucket assignment
        private bool IsUserInRolloutBucket(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return false;

            // Create deterministic hash from featureName + userId
            string hashInput = $"{FeatureName}:{userId}";
            int hashCode = GetStableHashCode(hashInput);

            // Convert to bucket 0-99
            int bucket = Math.Abs(hashCode % 100);

            // User is in rollout if their bucket < rollout percentage
            return bucket < RolloutPercentage;
        }

        // Stable hash function 
        private static int GetStableHashCode(string str)
        {
            unchecked
            {
                int hash1 = 5381;
                int hash2 = hash1;

                for (int i = 0; i < str.Length && str[i] != '\0'; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if (i == str.Length - 1 || str[i + 1] == '\0')
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return hash1 + (hash2 * 1566083941);
            }
        }
    }
}
