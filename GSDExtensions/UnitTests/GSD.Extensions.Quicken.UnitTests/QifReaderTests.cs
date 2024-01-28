// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QifReaderTests.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.Quicken.UnitTests;

using Xunit;

/// <summary>
/// Provides unit tests for the <see cref="QifReader" /> class.
/// </summary>
/// ReSharper disable StringLiteralTypo
public class QifReaderTests
{
    /// <summary>
    /// Ensures that the <see cref="QifReader" /> class can read QIF files.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    [Fact]
    public async Task CanReadQifFiles()
    {
        using var reader = new QifReader(@"SAMPLE.QIF");
        var document = await reader.ReadDocumentAsync().ConfigureAwait(false);

        Assert.Equal(3, document.Accounts.Count);

        Assert.Equal(string.Empty, document.Accounts[0].Name);
        Assert.Equal(string.Empty, document.Accounts[0].AccountType);
        Assert.Equal(-820.63m, document.Accounts[0].Balance);

        Assert.Equal("Joint Brokerage Account", document.Accounts[1].Name);
        Assert.Equal("Invst", document.Accounts[1].AccountType);
        Assert.Equal(11010.00m, document.Accounts[1].Balance);

        Assert.Equal("Sample Checking Account", document.Accounts[2].Name);
        Assert.Equal("Bank", document.Accounts[2].AccountType);
        Assert.Equal(-35.50m, document.Accounts[2].Balance);

        Assert.Equal(10153.87m, document.Balance);
    }
}