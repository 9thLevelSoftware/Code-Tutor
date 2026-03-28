using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CodeTutor.Wpf.Models.Validation
{
    /// <summary>
    /// Represents a validated correct answer that can be either an integer index or a string value.
    /// Replaces the unsafe JsonElement? to prevent type confusion attacks.
    /// </summary>
    public readonly struct ValidatedCorrectAnswer : IEquatable<ValidatedCorrectAnswer>
    {
        private readonly int? _intValue;
        private readonly string? _stringValue;

        public AnswerType Type { get; }

        public int? IntValue => Type == AnswerType.Integer ? _intValue : null;
        public string? StringValue => Type == AnswerType.String ? _stringValue : null;

        private ValidatedCorrectAnswer(int? intValue, string? stringValue, AnswerType type)
        {
            _intValue = intValue;
            _stringValue = stringValue;
            Type = type;
        }

        public static ValidatedCorrectAnswer FromInt(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Answer index cannot be negative.");
            return new ValidatedCorrectAnswer(value, null, AnswerType.Integer);
        }

        public static ValidatedCorrectAnswer FromString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Answer string cannot be null or empty.", nameof(value));
            if (value.Length > 10)
                throw new ArgumentException("Answer string exceeds maximum length.", nameof(value));
            // Only allow alphanumeric characters and common answer format characters
            if (!value.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_'))
                throw new ArgumentException("Answer string contains invalid characters.", nameof(value));
            return new ValidatedCorrectAnswer(null, value, AnswerType.String);
        }

        public int GetAnswerIndex(int optionCount)
        {
            return Type switch
            {
                AnswerType.Integer => _intValue ?? -1,
                AnswerType.String => ParseStringToIndex(_stringValue, optionCount),
                _ => -1
            };
        }

        private static int ParseStringToIndex(string? value, int optionCount)
        {
            if (value == null) return -1;

            // Try direct integer parsing first
            if (int.TryParse(value, out var idx))
                return idx >= 0 && idx < optionCount ? idx : -1;

            // Handle letter answers: a=0, b=1, c=2, d=3 (case-insensitive)
            if (value.Length == 1 && char.IsLetter(value[0]))
            {
                var letterIndex = char.ToLower(value[0]) - 'a';
                return letterIndex >= 0 && letterIndex < optionCount ? letterIndex : -1;
            }

            return -1;
        }

        public bool Equals(ValidatedCorrectAnswer other)
        {
            return Type == other.Type &&
                   _intValue == other._intValue &&
                   _stringValue == other._stringValue;
        }

        public override bool Equals(object? obj)
        {
            return obj is ValidatedCorrectAnswer other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, _intValue, _stringValue);
        }

        public override string ToString()
        {
            return Type switch
            {
                AnswerType.Integer => _intValue?.ToString() ?? "null",
                AnswerType.String => _stringValue ?? "null",
                _ => "unknown"
            };
        }
    }

    public enum AnswerType
    {
        None,
        Integer,
        String
    }

    /// <summary>
    /// JSON converter for ValidatedCorrectAnswer that handles both int and string inputs securely.
    /// </summary>
    public class ValidatedCorrectAnswerConverter : JsonConverter<ValidatedCorrectAnswer>
    {
        public override ValidatedCorrectAnswer Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Number:
                    if (reader.TryGetInt32(out var intValue))
                    {
                        if (intValue < 0 || intValue > 1000)
                            throw new JsonException("Answer index is out of valid range (0-1000).");
                        return ValidatedCorrectAnswer.FromInt(intValue);
                    }
                    throw new JsonException("Answer number must be a valid integer.");

                case JsonTokenType.String:
                    var strValue = reader.GetString();
                    if (string.IsNullOrWhiteSpace(strValue))
                        throw new JsonException("Answer string cannot be null or empty.");
                    if (strValue.Length > 10)
                        throw new JsonException("Answer string exceeds maximum length of 10 characters.");
                    // Whitelist allowed characters: letters, digits, dash, underscore
                    if (!strValue.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_'))
                        throw new JsonException("Answer string contains invalid characters. Only alphanumeric, dash, and underscore are allowed.");
                    return ValidatedCorrectAnswer.FromString(strValue);

                case JsonTokenType.Null:
                    throw new JsonException("CorrectAnswer cannot be null.");

                default:
                    throw new JsonException($"Unexpected token type for CorrectAnswer: {reader.TokenType}");
            }
        }

        public override void Write(Utf8JsonWriter writer, ValidatedCorrectAnswer value, JsonSerializerOptions options)
        {
            switch (value.Type)
            {
                case AnswerType.Integer:
                    writer.WriteNumberValue(value.IntValue ?? 0);
                    break;
                case AnswerType.String:
                    writer.WriteStringValue(value.StringValue ?? string.Empty);
                    break;
                default:
                    writer.WriteNullValue();
                    break;
            }
        }
    }
}
