// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Base64UrlTests.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.Http.UnitTests;

using System.Text;
using Xunit;

/// <summary>
/// Provides unit tests for the <see cref="Base64Url" /> class.
/// </summary>
public class Base64UrlTests
{
  /// <summary>
  /// Verifies that the <see cref="Base64Url.Decode" /> method correctly decodes a Base64url string into the expected byte array.
  /// </summary>
  /// <param name="input">The Base64url encoded string.</param>
  /// <param name="expected">The expected decoded string.</param>
  [Theory]
  [InlineData("SGVsbG8gd29ybGQ=", "Hello world")]
  [InlineData("U29tZSBkYXRh", "Some data")]
  [InlineData("SGVsbG8gV29ybGQh", "Hello World!")]
  public void DecodeShouldReturnExpectedBytes(string input, string expected)
  {
    // Arrange
    var expectedBytes = Encoding.UTF8.GetBytes(expected);

    // Act
    var actualBytes = Base64Url.Decode(input);

    // Assert
    Assert.Equal(expectedBytes, actualBytes);
  }

  /// <summary>
  /// Verifies that the <see cref="Base64Url.Decode" /> method returns <c>null</c> when the input is <c>null</c>.
  /// </summary>
  [Fact]
  public void DecodeShouldReturnNullWhenInputIsNull()
  {
    // Act
    var result = Base64Url.Decode(null);

    // Assert
    Assert.Null(result);
  }

  /// <summary>
  /// Verifies that the <see cref="Base64Url.Decode" /> method throws an <see cref="ArgumentException" /> when given an invalid Base64url string.
  /// </summary>
  /// <param name="input">The invalid Base64url string.</param>
  /// <param name="expectedExceptionType">The type of exception expected to be thrown.</param>
  [Theory]
  [InlineData("Invalid Base64url string", typeof(FormatException))]
  public void DecodeShouldThrowArgumentExceptionWhenInputIsInvalid(string input, Type expectedExceptionType)
  {
    // Act & Assert
    var exception = Assert.Throws(expectedExceptionType, () => Base64Url.Decode(input));
    Assert.IsType(expectedExceptionType, exception);
  }

  /// <summary>
  /// Verifies that the <see cref="Base64Url.Encode" /> method correctly encodes a byte array into the expected Base64url string.
  /// </summary>
  /// <param name="input">The input string to encode.</param>
  /// <param name="expected">The expected Base64url encoded string.</param>
  [Theory]
  [InlineData("Hello world", "SGVsbG8gd29ybGQ")]
  [InlineData("Some data", "U29tZSBkYXRh")]
  [InlineData("Hello World!", "SGVsbG8gV29ybGQh")]
  public void EncodeShouldReturnExpectedBase64Url(string input, string expected)
  {
    // Arrange
    var inputBytes = Encoding.UTF8.GetBytes(input);

    // Act
    var actualBase64Url = Base64Url.Encode(inputBytes);

    // Assert
    Assert.Equal(expected, actualBase64Url);
  }

  /// <summary>
  /// Verifies that the <see cref="Base64Url.Encode" /> method returns <c>null</c> when the input is <c>null</c>.
  /// </summary>
  [Fact]
  public void EncodeShouldReturnNullWhenInputIsNull()
  {
    // Act
    var result = Base64Url.Encode(null);

    // Assert
    Assert.Null(result);
  }
}