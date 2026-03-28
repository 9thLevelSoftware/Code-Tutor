using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CodeTutor.Wpf.Models.Validation
{
    /// <summary>
    /// Represents the result of JSON schema validation.
    /// </summary>
    public sealed class ValidationResult
    {
        public bool IsValid { get; }
        public List<string> Errors { get; }
        public string? SanitizedJson { get; }

        private ValidationResult(bool isValid, List<string> errors, string? sanitizedJson = null)
        {
            IsValid = isValid;
            Errors = errors;
            SanitizedJson = sanitizedJson;
        }

        public static ValidationResult Success(string? sanitizedJson = null)
        {
            return new ValidationResult(true, new List<string>(), sanitizedJson);
        }

        public static ValidationResult Failure(List<string> errors)
        {
            return new ValidationResult(false, errors);
        }

        public static ValidationResult Failure(string error)
        {
            return new ValidationResult(false, new List<string> { error });
        }
    }

    /// <summary>
    /// Base class for validated model entities with common validation logic.
    /// </summary>
    public abstract class ValidatedModel
    {
        /// <summary>
        /// Maximum allowed string length for any property.
        /// </summary>
        protected const int MaxStringLength = 10000;

        /// <summary>
        /// Maximum allowed nested depth for collections.
        /// </summary>
        protected const int MaxCollectionDepth = 10;

        /// <summary>
        /// Maximum allowed items in any collection.
        /// </summary>
        protected const int MaxCollectionSize = 1000;

        /// <summary>
        /// Validates that a string property meets security requirements.
        /// </summary>
        protected static ValidationResult ValidateString(string? value, string propertyName, bool required = true, int maxLength = 5000)
        {
            var errors = new List<string>();

            if (required && string.IsNullOrWhiteSpace(value))
            {
                errors.Add($"{propertyName} is required and cannot be empty.");
                return ValidationResult.Failure(errors);
            }

            if (value != null)
            {
                // Check maximum length
                if (value.Length > maxLength)
                {
                    errors.Add($"{propertyName} exceeds maximum length of {maxLength} characters (found {value.Length}).");
                }

                // Check for potentially dangerous characters (basic XSS prevention)
                var dangerousChars = new[] { '<', '>', '"', '\'', '&' };
                if (value.IndexOfAny(dangerousChars) >= 0)
                {
                    // This is just a warning for content - we'll sanitize during output
                    // but flag it for logging
                }

                // Check for null bytes
                if (value.Contains('\0'))
                {
                    errors.Add($"{propertyName} contains invalid null bytes.");
                }

                // Check for control characters (except common whitespace)
                for (int i = 0; i < value.Length; i++)
                {
                    char c = value[i];
                    if (char.IsControl(c) && c != '\n' && c != '\r' && c != '\t')
                    {
                        errors.Add($"{propertyName} contains invalid control character at position {i}.");
                        break;
                    }
                }
            }

            return errors.Count > 0 ? ValidationResult.Failure(errors) : ValidationResult.Success();
        }

        /// <summary>
        /// Validates that a collection does not exceed size limits.
        /// </summary>
        protected static ValidationResult ValidateCollection<T>(ICollection<T>? collection, string propertyName, int maxSize = MaxCollectionSize)
        {
            if (collection == null)
                return ValidationResult.Success();

            if (collection.Count > maxSize)
            {
                return ValidationResult.Failure($"{propertyName} exceeds maximum size of {maxSize} items (found {collection.Count}).");
            }

            return ValidationResult.Success();
        }

        /// <summary>
        /// Validates that an identifier meets requirements.
        /// </summary>
        protected static ValidationResult ValidateId(string? value, string propertyName)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(value))
            {
                errors.Add($"{propertyName} is required and cannot be empty.");
                return ValidationResult.Failure(errors);
            }

            // ID length limit
            if (value.Length > 100)
            {
                errors.Add($"{propertyName} exceeds maximum length of 100 characters.");
            }

            // ID format: alphanumeric, dash, underscore, dot
            if (!value.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_' || c == '.'))
            {
                errors.Add($"{propertyName} contains invalid characters. Only alphanumeric, dash, underscore, and dot are allowed.");
            }

            return errors.Count > 0 ? ValidationResult.Failure(errors) : ValidationResult.Success();
        }

        /// <summary>
        /// Sanitizes a string value by truncating if necessary.
        /// </summary>
        protected static string? SanitizeString(string? value, int maxLength = MaxStringLength)
        {
            if (value == null)
                return null;

            if (value.Length > maxLength)
            {
                return value.Substring(0, maxLength);
            }

            return value;
        }

        /// <summary>
        /// Abstract method for model-specific validation.
        /// </summary>
        public abstract ValidationResult Validate();
    }
}
