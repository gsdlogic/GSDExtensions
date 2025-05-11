// <copyright file="ExceptionExtensions.cs" company="GSD Logic">
// Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>

namespace GSD.Extensions.Diagnostics;

using System;
using System.Collections;
using System.Reflection;
using System.Text;

/// <summary>
/// Provides extension methods for the <see cref="Exception" /> class.
/// </summary>
public static class ExceptionExtensions
{
    /// <summary>
    /// Gets a string that includes all the exception details.
    /// </summary>
    /// <param name="ex">The root exception.</param>
    /// <param name="newline">Indicates whether to include a newline before the details.</param>
    /// <returns>A string that includes all the exception details.</returns>
    public static string GetDetails(this Exception ex, bool newline = false)
    {
        if (ex == null)
        {
            return null;
        }

        var builder = new StringBuilder();

        if (newline)
        {
            builder.AppendLine();
        }

        BuildDetails(ex, builder, 0);
        return builder.ToString();
    }

    /// <summary>
    /// Gets a message that includes all the nested exception messages.
    /// </summary>
    /// <param name="ex">The root exception.</param>
    /// <param name="newline">Indicates whether to include a newline before the message.</param>
    /// <returns>A message that includes all the nested exception messages.</returns>
    public static string GetExpandedMessage(this Exception ex, bool newline = false)
    {
        if (ex == null)
        {
            return null;
        }

        var builder = new StringBuilder();

        if (newline)
        {
            builder.AppendLine();
        }

        BuildExpandedMessage(ex, builder, 0);
        return builder.ToString();
    }

    /// <summary>
    /// Builds a string that includes all the exception details.
    /// </summary>
    /// <param name="ex">The root exception.</param>
    /// <param name="builder">The string builder to build the details.</param>
    /// <param name="level">The level of indentation.</param>
    private static void BuildDetails(Exception ex, StringBuilder builder, int level)
    {
        while (true)
        {
            var indentation = new string(' ', level * 3);

            var type = ex.GetType();
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            builder.Append(indentation);
            builder.Append(ex.GetType().FullName);
            builder.Append(':');

            if (ex is AggregateException aex)
            {
                var innerExceptions = aex.Flatten().InnerExceptions;

                for (var i = 0; i < innerExceptions.Count; i++)
                {
                    var aggregate = innerExceptions[i];

                    builder.AppendLine();
                    builder.AppendLine();
                    builder.Append(indentation);
                    builder.Append("   --- Exception ");
                    builder.Append(i + 1);
                    builder.Append('/');
                    builder.Append(innerExceptions.Count);
                    builder.AppendLine(" ---");

                    BuildDetails(aggregate, builder, level + 1);
                }

                break;
            }

            builder.Append(' ');
            builder.Append(ex.Message);

            foreach (var property in properties)
            {
                if (!property.CanRead ||
                    (property.GetIndexParameters().Length != 0) ||
                    (property.Name == "InnerException") ||
                    (property.Name == "StackTrace") ||
                    (property.Name == "Data") ||
                    (property.Name == "Message"))
                {
                    continue;
                }

                object value;

                try
                {
                    value = property.GetValue(ex, null);
                }
                catch (TargetInvocationException)
                {
                    value = "Access failed";
                }

                builder.AppendLine();
                builder.Append(indentation);
                builder.Append("   ");
                builder.Append(property.Name);
                builder.Append(": ");
                builder.Append(value);
            }

            if (ex.Data.Count > 0)
            {
                builder.AppendLine();
                builder.Append(indentation);
                builder.Append("   Data:");
                builder.AppendLine();

                foreach (DictionaryEntry entry in ex.Data)
                {
                    builder.Append(indentation);
                    builder.Append("      ");
                    builder.Append(entry.Key);
                    builder.Append(": ");
                    builder.Append(entry.Value);
                }
            }

            if (ex.StackTrace != null)
            {
                var stackTrace = ex.StackTrace
                    .Replace("\r\n", "\n", StringComparison.Ordinal)
                    .Replace("\n", $"{Environment.NewLine}{indentation}", StringComparison.Ordinal);

                builder.AppendLine();
                builder.Append(indentation);
                builder.Append(stackTrace);
            }

            if (ex.InnerException == null)
            {
                break;
            }

            builder.AppendLine();
            builder.AppendLine();
            builder.Append(indentation);
            builder.AppendLine("   --- Inner Exception ---");
            ex = ex.InnerException;
            level++;
        }
    }

    /// <summary>
    /// Builds a message that includes all the nested exception messages.
    /// </summary>
    /// <param name="ex">The root exception.</param>
    /// <param name="builder">The string builder to build the message.</param>
    /// <param name="level">The level of indentation.</param>
    private static void BuildExpandedMessage(Exception ex, StringBuilder builder, int level)
    {
        while (true)
        {
            var indentation = new string(' ', level * 2);

            builder.Append(indentation);
            builder.Append(ex.GetType().Name);
            builder.Append(':');

            if (ex is AggregateException aex)
            {
                var innerExceptions = aex.Flatten().InnerExceptions;

                foreach (var aggregate in innerExceptions)
                {
                    builder.AppendLine();
                    BuildExpandedMessage(aggregate, builder, level + 1);
                }

                break;
            }

            builder.Append(' ');
            builder.Append(ex.Message);

            if (ex.InnerException == null)
            {
                break;
            }

            builder.AppendLine();
            ex = ex.InnerException;
            level++;
        }
    }
}