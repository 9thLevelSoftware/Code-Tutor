using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CodeTutor.Wpf.Models;
using CodeTutor.Wpf.Models.Validation;

namespace CodeTutor.Wpf.Services
{
    /// <summary>
    /// Service responsible for secure JSON deserialization with strict schema validation.
    /// Addresses SEC-003: Unsafe JSON Deserialization vulnerability.
    /// </summary>
    public interface IJsonValidationService
    {
        Task<(T? Result, List<string> Errors)> DeserializeAsync<T>(string json, string contentType) where T : class;
        (T? Result, List<string> Errors) Deserialize<T>(string json, string contentType) where T : class;
        bool TryValidateCourse(Course course, out List<string> errors);
        bool TryValidateModule(Module module, out List<string> errors);
        bool TryValidateLesson(Lesson lesson, out List<string> errors);
        bool TryValidateChallenge(Challenge challenge, out List<string> errors);
    }

    public delegate bool TryValidate<T>(T model, out List<string> errors);

    public class JsonValidationService : IJsonValidationService
    {
        // Strict JSON serializer options to prevent type confusion and injection attacks
        private readonly JsonSerializerOptions _strictOptions;
        private readonly ILoggingService _loggingService;

        // Maximum allowed JSON size (10MB)
        private const int MaxJsonSize = 10 * 1024 * 1024;

        public JsonValidationService(ILoggingService loggingService)
        {
            _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));

            _strictOptions = new JsonSerializerOptions
            {
                // Case sensitivity is STRICT - prevents type confusion via property name variations
                PropertyNameCaseInsensitive = false,

                // Throw on comments - prevents comment-based attacks
                ReadCommentHandling = JsonCommentHandling.Throw,

                // No trailing commas allowed - strict parsing
                AllowTrailingCommas = false,

                // Limit maximum depth to prevent stack overflow attacks
                MaxDepth = 32,

                // Use strict number handling
                NumberHandling = JsonNumberHandling.Strict,

                // Do not allow unknown types
                UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,

                // Property naming policy - match JSON exactly
                PropertyNamingPolicy = null,

                // Converters for secure deserialization
                Converters =
                {
                    new ValidatedCorrectAnswerConverter()
                }
            };
        }

        /// <summary>
        /// Asynchronously deserializes and validates JSON content.
        /// </summary>
        public async Task<(T? Result, List<string> Errors)> DeserializeAsync<T>(string json, string contentType) where T : class
        {
            return await Task.Run(() => Deserialize<T>(json, contentType));
        }

        /// <summary>
        /// Deserializes and validates JSON content with strict schema validation.
        /// </summary>
        public (T? Result, List<string> Errors) Deserialize<T>(string json, string contentType) where T : class
        {
            var errors = new List<string>();

            try
            {
                // Validate JSON size
                if (string.IsNullOrEmpty(json))
                {
                    errors.Add("JSON content is empty.");
                    _loggingService.LogWarning($"Empty JSON content received for {contentType}");
                    return (null, errors);
                }

                if (json.Length > MaxJsonSize)
                {
                    errors.Add($"JSON content exceeds maximum size of {MaxJsonSize} bytes.");
                    _loggingService.LogWarning($"Oversized JSON content rejected for {contentType}: {json.Length} bytes");
                    return (null, errors);
                }

                // Check for obvious injection patterns
                if (ContainsDangerousPatterns(json))
                {
                    errors.Add("JSON content contains potentially dangerous patterns.");
                    _loggingService.LogSecurityEvent($"Dangerous pattern detected in {contentType} JSON");
                    return (null, errors);
                }

                // Deserialize with strict options
                T? result;
                try
                {
                    result = JsonSerializer.Deserialize<T>(json, _strictOptions);
                }
                catch (JsonException ex)
                {
                    errors.Add($"JSON deserialization error: {SanitizeErrorMessage(ex.Message)}");
                    _loggingService.LogError($"JSON deserialization failed for {contentType}", ex);
                    return (null, errors);
                }

                if (result == null)
                {
                    errors.Add("Failed to deserialize JSON content.");
                    return (null, errors);
                }

                // Perform model-specific validation
                var validationErrors = ValidateModel(result, contentType);
                if (validationErrors.Count > 0)
                {
                    errors.AddRange(validationErrors);
                    _loggingService.LogWarning($"Validation failed for {contentType}: {string.Join("; ", validationErrors)}");
                    return (null, errors);
                }

                _loggingService.LogInfo($"Successfully validated {contentType}");
                return (result, errors);
            }
            catch (Exception ex)
            {
                errors.Add($"Unexpected error during deserialization: {SanitizeErrorMessage(ex.Message)}");
                _loggingService.LogError($"Unexpected error deserializing {contentType}", ex);
                return (null, errors);
            }
        }

        /// <summary>
        /// Validates a Course model.
        /// </summary>
        public bool TryValidateCourse(Course course, out List<string> errors)
        {
            errors = new List<string>();

            if (course == null)
            {
                errors.Add("Course cannot be null.");
                return false;
            }

            // Validate required fields
            ValidateId(course.Id, "Course.Id", errors);
            ValidateString(course.Title, "Course.Title", required: true, maxLength: 200, errors);
            ValidateString(course.Language, "Course.Language", required: true, maxLength: 50, errors);
            ValidateString(course.Description, "Course.Description", required: false, maxLength: 5000, errors);
            ValidateString(course.Difficulty, "Course.Difficulty", required: false, maxLength: 50, errors);

            // Validate estimated hours is reasonable
            if (course.EstimatedHours < 0 || course.EstimatedHours > 1000)
            {
                errors.Add("Course.EstimatedHours must be between 0 and 1000.");
            }

            // Validate modules collection
            if (course.Modules != null)
            {
                if (course.Modules.Count > 100)
                {
                    errors.Add("Course.Modules exceeds maximum of 100 modules.");
                }

                for (int i = 0; i < course.Modules.Count; i++)
                {
                    var moduleErrors = new List<string>();
                    if (!TryValidateModule(course.Modules[i], out moduleErrors))
                    {
                        errors.AddRange(moduleErrors.Select(e => $"Module[{i}]: {e}"));
                    }
                }
            }

            return errors.Count == 0;
        }

        /// <summary>
        /// Validates a Module model.
        /// </summary>
        public bool TryValidateModule(Module module, out List<string> errors)
        {
            errors = new List<string>();

            if (module == null)
            {
                errors.Add("Module cannot be null.");
                return false;
            }

            ValidateId(module.Id, "Module.Id", errors);
            ValidateString(module.Title, "Module.Title", required: true, maxLength: 200, errors);
            ValidateString(module.Description, "Module.Description", required: false, maxLength: 2000, errors);

            // Validate lessons collection
            if (module.Lessons != null)
            {
                if (module.Lessons.Count > 50)
                {
                    errors.Add("Module.Lessons exceeds maximum of 50 lessons.");
                }

                for (int i = 0; i < module.Lessons.Count; i++)
                {
                    var lessonErrors = new List<string>();
                    if (!TryValidateLesson(module.Lessons[i], out lessonErrors))
                    {
                        errors.AddRange(lessonErrors.Select(e => $"Lesson[{i}]: {e}"));
                    }
                }
            }

            return errors.Count == 0;
        }

        /// <summary>
        /// Validates a Lesson model.
        /// </summary>
        public bool TryValidateLesson(Lesson lesson, out List<string> errors)
        {
            errors = new List<string>();

            if (lesson == null)
            {
                errors.Add("Lesson cannot be null.");
                return false;
            }

            ValidateId(lesson.Id, "Lesson.Id", errors);
            ValidateId(lesson.ModuleId, "Lesson.ModuleId", errors);
            ValidateString(lesson.Title, "Lesson.Title", required: true, maxLength: 200, errors);
            ValidateString(lesson.Difficulty, "Lesson.Difficulty", required: false, maxLength: 50, errors);

            // Validate order is non-negative
            if (lesson.Order < 0)
            {
                errors.Add("Lesson.Order cannot be negative.");
            }

            // Validate estimated minutes is reasonable
            if (lesson.EstimatedMinutes < 0 || lesson.EstimatedMinutes > 1000)
            {
                errors.Add("Lesson.EstimatedMinutes must be between 0 and 1000.");
            }

            // Validate content sections
            if (lesson.ContentSections != null && lesson.ContentSections.Count > 100)
            {
                errors.Add("Lesson.ContentSections exceeds maximum of 100 sections.");
            }

            // Validate challenges
            if (lesson.Challenges != null)
            {
                if (lesson.Challenges.Count > 50)
                {
                    errors.Add("Lesson.Challenges exceeds maximum of 50 challenges.");
                }

                for (int i = 0; i < lesson.Challenges.Count; i++)
                {
                    var challengeErrors = new List<string>();
                    if (!TryValidateChallenge(lesson.Challenges[i], out challengeErrors))
                    {
                        errors.AddRange(challengeErrors.Select(e => $"Challenge[{i}]: {e}"));
                    }
                }
            }

            return errors.Count == 0;
        }

        /// <summary>
        /// Validates a Challenge model.
        /// </summary>
        public bool TryValidateChallenge(Challenge challenge, out List<string> errors)
        {
            errors = new List<string>();

            if (challenge == null)
            {
                errors.Add("Challenge cannot be null.");
                return false;
            }

            // Validate type is from allowed set
            var allowedTypes = new[] { "FREE_CODING", "QUIZ", "MULTI_QUIZ", "TRUE_FALSE", "CODE_OUTPUT" };
            if (string.IsNullOrEmpty(challenge.Type) || !allowedTypes.Contains(challenge.Type))
            {
                errors.Add($"Challenge.Type must be one of: {string.Join(", ", allowedTypes)}.");
            }

            ValidateId(challenge.Id, "Challenge.Id", errors);
            ValidateString(challenge.Title, "Challenge.Title", required: true, maxLength: 200, errors);
            ValidateString(challenge.Description, "Challenge.Description", required: true, maxLength: 5000, errors);
            ValidateString(challenge.Instructions, "Challenge.Instructions", required: true, maxLength: 10000, errors);
            ValidateString(challenge.Language, "Challenge.Language", required: false, maxLength: 50, errors);
            ValidateString(challenge.Difficulty, "Challenge.Difficulty", required: false, maxLength: 50, errors);

            // Validate code size limits
            if (challenge.StarterCode?.Length > 50000)
            {
                errors.Add("Challenge.StarterCode exceeds maximum size of 50000 characters.");
            }
            if (challenge.Solution?.Length > 50000)
            {
                errors.Add("Challenge.Solution exceeds maximum size of 50000 characters.");
            }

            // Validate collections
            if (challenge.Hints?.Count > 20)
            {
                errors.Add("Challenge.Hints exceeds maximum of 20 hints.");
            }

            if (challenge.TestCases?.Count > 50)
            {
                errors.Add("Challenge.TestCases exceeds maximum of 50 test cases.");
            }

            if (challenge.CommonMistakes?.Count > 20)
            {
                errors.Add("Challenge.CommonMistakes exceeds maximum of 20 common mistakes.");
            }

            // Type-specific validation
            switch (challenge.Type)
            {
                case "QUIZ":
                    ValidateQuizChallenge(challenge, errors);
                    break;
                case "MULTI_QUIZ":
                    ValidateMultiQuizChallenge(challenge, errors);
                    break;
                case "TRUE_FALSE":
                    ValidateTrueFalseChallenge(challenge, errors);
                    break;
            }

            return errors.Count == 0;
        }

        private void ValidateQuizChallenge(Challenge challenge, List<string> errors)
        {
            if (string.IsNullOrEmpty(challenge.Question) && string.IsNullOrEmpty(challenge.Instructions))
            {
                errors.Add("QUIZ challenge requires Question or Instructions.");
            }

            if (challenge.Options == null || challenge.Options.Count < 2)
            {
                errors.Add("QUIZ challenge requires at least 2 options.");
            }
            else if (challenge.Options.Count > 10)
            {
                errors.Add("QUIZ challenge cannot have more than 10 options.");
            }

            // Validate options content
            if (challenge.Options != null)
            {
                for (int i = 0; i < challenge.Options.Count; i++)
                {
                    if (challenge.Options[i]?.Length > 1000)
                    {
                        errors.Add($"QUIZ option[{i}] exceeds maximum length of 1000 characters.");
                    }
                }
            }

            // CorrectAnswer is now strongly-typed and validated during deserialization
            // via the ValidatedCorrectAnswerConverter
        }

        private void ValidateMultiQuizChallenge(Challenge challenge, List<string> errors)
        {
            if (challenge.Questions == null || challenge.Questions.Count == 0)
            {
                errors.Add("MULTI_QUIZ challenge requires at least 1 question.");
            }
            else if (challenge.Questions.Count > 20)
            {
                errors.Add("MULTI_QUIZ challenge cannot have more than 20 questions.");
            }

            if (challenge.Questions != null)
            {
                for (int i = 0; i < challenge.Questions.Count; i++)
                {
                    var q = challenge.Questions[i];
                    if (string.IsNullOrEmpty(q.Question) && string.IsNullOrEmpty(q.Text))
                    {
                        errors.Add($"MULTI_QUIZ question[{i}] requires Question or Text.");
                    }

                    if (q.Options == null || q.Options.Count < 2)
                    {
                        errors.Add($"MULTI_QUIZ question[{i}] requires at least 2 options.");
                    }
                    else if (q.Options.Count > 10)
                    {
                        errors.Add($"MULTI_QUIZ question[{i}] cannot have more than 10 options.");
                    }

                    // CorrectAnswer validation is handled by ValidatedCorrectAnswerConverter
                }
            }
        }

        private void ValidateTrueFalseChallenge(Challenge challenge, List<string> errors)
        {
            if (string.IsNullOrEmpty(challenge.Statement) && string.IsNullOrEmpty(challenge.Question))
            {
                errors.Add("TRUE_FALSE challenge requires Statement or Question.");
            }

            // isTrue must be explicitly set
            if (!challenge.IsTrue.HasValue)
            {
                errors.Add("TRUE_FALSE challenge requires IsTrue value.");
            }
        }

        private List<string> ValidateModel<T>(T model, string contentType) where T : class
        {
            return contentType.ToLowerInvariant() switch
            {
                "course" => ValidateAndReturn(model as Course, TryValidateCourse),
                "module" => ValidateAndReturn(model as Module, TryValidateModule),
                "lesson" => ValidateAndReturn(model as Lesson, TryValidateLesson),
                "challenge" => ValidateAndReturn(model as Challenge, TryValidateChallenge),
                _ => new List<string> { $"Unknown content type: {contentType}" }
            };
        }

        private List<string> ValidateAndReturn<T>(T? model, TryValidate<T> validator)
            where T : class
        {
            if (model == null)
                return new List<string> { "Model is null" };

            var errors = new List<string>();
            validator(model, errors);
            return errors;
        }

        private void ValidateId(string? value, string propertyName, List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                errors.Add($"{propertyName} is required and cannot be empty.");
                return;
            }

            if (value.Length > 100)
            {
                errors.Add($"{propertyName} exceeds maximum length of 100 characters.");
            }

            // ID format: alphanumeric, dash, underscore, dot
            if (!value.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_' || c == '.'))
            {
                errors.Add($"{propertyName} contains invalid characters. Only alphanumeric, dash, underscore, and dot are allowed.");
            }
        }

        private void ValidateString(string? value, string propertyName, bool required, int maxLength, List<string> errors)
        {
            if (required && string.IsNullOrWhiteSpace(value))
            {
                errors.Add($"{propertyName} is required and cannot be empty.");
                return;
            }

            if (value != null && value.Length > maxLength)
            {
                errors.Add($"{propertyName} exceeds maximum length of {maxLength} characters.");
            }
        }

        private bool ContainsDangerousPatterns(string json)
        {
            // Check for patterns that might indicate injection attempts
            var dangerousPatterns = new[]
            {
                "__proto__",
                "constructor",
                "prototype",
                "<script",
                "javascript:",
                "onerror=",
                "onload=",
                "eval(",
                "Function(",
                "setTimeout(",
                "setInterval("
            };

            var lowerJson = json.ToLowerInvariant();
            return dangerousPatterns.Any(pattern => lowerJson.Contains(pattern));
        }

        private string SanitizeErrorMessage(string message)
        {
            // Sanitize error messages to prevent information leakage
            if (message == null)
                return "Unknown error";

            // Truncate very long messages
            if (message.Length > 500)
            {
                message = message.Substring(0, 500) + "...";
            }

            // Remove potentially sensitive path information
            var index = message.IndexOf("Path: ");
            if (index > 0)
            {
                message = message.Substring(0, index).Trim();
            }

            return message;
        }
    }

    /// <summary>
    /// Logging service interface for security events.
    /// </summary>
    public interface ILoggingService
    {
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message, Exception? ex = null);
        void LogSecurityEvent(string message);
    }
}
