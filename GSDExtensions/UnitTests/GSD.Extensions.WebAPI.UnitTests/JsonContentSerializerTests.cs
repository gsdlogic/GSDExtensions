// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonContentSerializerTests.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.WebAPI.UnitTests;

using System.Text;
using Newtonsoft.Json.Linq;
using Xunit;

/// <summary>
/// Provides unit tests for the <see cref="JsonContentSerializer" /> class.
/// </summary>
public class JsonContentSerializerTests
{
    /// <summary>
    /// Tests the ability to deserialize objects.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    [Fact]
    public async Task DeserializerTest()
    {
        var serializer = new JsonContentSerializer();

        using var request = new HttpRequestMessage(HttpMethod.Get, "/");
        using var response = new HttpResponseMessage();
        using var content = new StringContent("{\"Name\":\"foo\"}", Encoding.UTF8, "application/json");

        response.Content = content;

        var sample = await serializer.DeserializeAsync<JObject>(request, response).ConfigureAwait(true);

        Assert.NotNull(sample);
        Assert.Equal("foo", sample["Name"]);
    }

    /// <summary>
    /// Tests the ability to serialize objects.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    [Fact]
    public async Task SerializerTest()
    {
        var serializer = new JsonContentSerializer();

        using var request = new HttpRequestMessage(HttpMethod.Get, "/");

        var sample = new JObject
        {
            { "Name", "foo" },
        };

        using var content = await serializer.SerializeAsync(request, sample).ConfigureAwait(true);

        var stream = await content.ReadAsStreamAsync().ConfigureAwait(true);
        await using var streamDisposable = stream.ConfigureAwait(true);

        using var streamReader = new StreamReader(stream);
        var result = await streamReader.ReadToEndAsync().ConfigureAwait(true);

        Assert.Equal("{\"Name\":\"foo\"}", result);
    }
}