// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QifReader.cs" company="GSD Logic">
//   Copyright © 2024 GSD Logic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GSD.Extensions.Quicken;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Reads text files in Quicken Interchange Format (QIF).
/// </summary>
/// <remarks>
/// For information about the Quicken Interchange Format files, visit the
/// following web sites:
/// http://web.intuit.com/support/quicken/docs/d_qif.html.
/// https://en.wikipedia.org/wiki/Quicken_Interchange_Format
/// https://www.w3.org/2000/10/swap/pim/qif-doc/QIF-doc.htm#:~:text=A%3A%20The%20Quicken%20interchange%20format,that%20supports%20the%20QIF%20format.
/// </remarks>
public class QifReader : IDisposable
{
    /// <summary>
    /// A value indicating that the stream will be closed when the object is destroyed.
    /// </summary>
    private readonly bool isOwner;

    /// <summary>
    /// The stack for pushing items back to the buffer.
    /// </summary>
    private readonly Stack<QifItem> stack = new ();

    /// <summary>
    /// The current <see cref="QifAccount" /> object.
    /// </summary>
    private QifAccount currentAccount;

    /// <summary>
    /// The current line number.
    /// </summary>
    private long lineNumber;

    /// <summary>
    /// The <see cref="StreamReader" /> object referencing the QIF file.
    /// </summary>
    private StreamReader reader;

    /// <summary>
    /// Initializes a new instance of the <see cref="QifReader" /> class.
    /// </summary>
    /// <param name="path">The path to the QIF file.</param>
    public QifReader(string path)
        : this(new StreamReader(path, Encoding.ASCII), true)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QifReader" /> class.
    /// </summary>
    /// <param name="stream">A <see cref="Stream" /> object referencing the QIF file to read.</param>
    public QifReader(Stream stream)
        : this(new StreamReader(stream, Encoding.ASCII), true)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QifReader" /> class.
    /// </summary>
    /// <param name="stream">A <see cref="StreamReader" /> object referencing the QIF file to read.</param>
    /// <param name="isOwner">Indicates that the stream will be closed when the object is destroyed.</param>
    public QifReader(StreamReader stream, bool isOwner)
    {
        this.reader = stream;
        this.isOwner = isOwner;
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="QifReader" /> class.
    /// </summary>
    ~QifReader()
    {
        this.Dispose(false);
    }

    /// <summary>
    /// Closes the underlying <see cref="StreamReader" /> object.
    /// </summary>
    public void Close()
    {
        this.reader.Close();
    }

    /// <summary>
    /// Performs tasks associated with freeing, releasing, or resetting
    /// unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Reads the QIF file and returns a <see cref="QifDocument" /> object.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation whose result is a <see cref="QifDocument" /> object.</returns>
    public async Task<QifDocument> ReadDocumentAsync(CancellationToken cancellationToken = default)
    {
        var document = new QifDocument();
        var item = await this.ReadLineAsync(cancellationToken).ConfigureAwait(false);

        while (!item.EndOfFile)
        {
            if (item.Field != '!')
            {
                throw new InvalidOperationException("Invalid token '" + item.Text + "' at line " + this.lineNumber + ".");
            }

            switch (item.Value.ToUpperInvariant())
            {
                case "ACCOUNT": // !Account - Account list or which account follows
                    await this.ReadAccountAsync(document, cancellationToken).ConfigureAwait(false);
                    break;

                case "TYPE:CAT": // !Type:Cat - Category list
                    await this.ReadCategoryAsync(document, cancellationToken).ConfigureAwait(false);
                    break;

                case "TYPE:TAG": // !Type:Tag - Tag list
                    await this.ReadTagAsync(document, cancellationToken).ConfigureAwait(false);
                    break;

                case "TYPE:BANK": // !Type:Bank - Bank account transactions
                case "TYPE:CASH": // !Type:Cash - Cash account transactions
                case "TYPE:CCARD": // !Type:CCard - Credit card account transactions
                case "TYPE:INVST": // !Type:Invst - Investment account transactions
                case "TYPE:OTH A": // !Type:Oth A - Asset account transactions
                case "TYPE:OTH L": // !Type:Oth L - Liability account transactions
                    await this.ReadTransactionAsync(document, cancellationToken).ConfigureAwait(false);
                    break;

                case "OPTION:ALLXFR": // !Option:AllXfr - Import all transfers
                case "OPTION:AUTOSWITCH": // !Option:Autoswitch - Undocumented
                case "CLEAR:AUTOSWITCH": // !Clear:Autoswitch - Undocumented
                    await this.ReadIgnoreAsync(cancellationToken).ConfigureAwait(false);
                    break;

                case "TYPE:CLASS": // !Type:Class - Class list
                case "TYPE:MEMORIZED": // !Type:Memorized - Memorized transaction list
                    throw new NotSupportedException("Unsupported token '" + item.Text + "' at line " + this.lineNumber + ".");

                default:
                    if (!item.Value.StartsWith("Type:", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("Invalid token '" + item.Text + "' at line " + this.lineNumber + ".");
                    }

                    await this.ReadTransactionAsync(document, cancellationToken).ConfigureAwait(false);
                    break;
            }

            item = await this.ReadLineAsync(cancellationToken).ConfigureAwait(false);
        }

        return document;
    }

    /// <summary>
    /// Performs tasks associated with freeing, releasing, or resetting
    /// managed and unmanaged resources.
    /// </summary>
    /// <param name="disposing">
    /// <see langword="true" /> to dispose of
    /// managed and unmanaged resources; <see langword="false" /> to dispose
    /// of unmanaged resources only.
    /// </param>
    /// <remarks>
    /// If disposing equals <see langword="true" />, the method has been
    /// called directly or indirectly by a user's code. Managed and
    /// unmanaged resources can be disposed. If disposing equals
    /// <see langword="false" />, the method has been called by the runtime
    /// from inside the finalizer and you should not reference other
    /// objects. Only unmanaged resources can be disposed.
    /// </remarks>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing || !this.isOwner)
        {
            return;
        }

        this.reader.Dispose();
        this.reader = null;
    }

    /// <summary>
    /// Reads lines from the QIF file until a header or end of file is encountered.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    protected async Task ReadIgnoreAsync(CancellationToken cancellationToken)
    {
        var item = await this.ReadLineAsync(cancellationToken).ConfigureAwait(false);

        while (!item.EndOfRecord)
        {
            item = await this.ReadLineAsync(cancellationToken).ConfigureAwait(false);
        }

        this.PushBack(item);
    }

    /// <summary>
    /// Pushes a single item back to the read buffer.
    /// </summary>
    /// <param name="item">The <see cref="QifItem" /> to push back.</param>
    private void PushBack(QifItem item)
    {
        this.lineNumber--;
        this.stack.Push(item);
    }

    /// <summary>
    /// Reads account information from the QIF file.
    /// </summary>
    /// <param name="document">A <see cref="QifDocument" /> to contain the account information.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    private async Task ReadAccountAsync(QifDocument document, CancellationToken cancellationToken)
    {
        var account = new QifAccount();
        var item = await this.ReadLineAsync(cancellationToken).ConfigureAwait(false);

        while (!item.EndOfRecord)
        {
            switch (item.Field)
            {
                case 'N': // Name
                    account.Name = item.Value;
                    break;

                case 'T': // Type of account
                    account.AccountType = item.Value;
                    break;

                case 'D': // Description
                    account.Description = item.Value;
                    break;

                case 'L': // Credit limit (only fore credit card account)
                    account.CreditLimit = decimal.Parse(item.Value, CultureInfo.InvariantCulture);
                    break;

                case '/': // Statement balance date
                    account.BalanceDate = DateTime.Parse(item.Value, CultureInfo.InvariantCulture);
                    break;

                case '$': // Statement balance
                    account.StatementBalance = decimal.Parse(item.Value, CultureInfo.InvariantCulture);
                    break;

                case '^': // ^ End of entry
                    document.Accounts.Add(account);
                    this.currentAccount = account;
                    account = new QifAccount();
                    break;

                default:
                    throw new InvalidOperationException("Invalid token '" + item.Text + "' at line " + this.lineNumber + ".");
            }

            item = await this.ReadLineAsync(cancellationToken).ConfigureAwait(false);
        }

        this.PushBack(item);
    }

    /// <summary>
    /// Reads category information from the QIF file.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    /// <param name="document">A <see cref="QifDocument" /> to contain the account information.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation requests.</param>
    private async Task ReadCategoryAsync(QifDocument document, CancellationToken cancellationToken)
    {
        var category = new QifCategory();
        var item = await this.ReadLineAsync(cancellationToken).ConfigureAwait(false);

        while (!item.EndOfRecord)
        {
            switch (item.Field)
            {
                case 'N': // N Category name:subcategory name
                    category.Name = item.Value;
                    break;

                case 'D': // D Description
                    category.Description = item.Value;
                    break;

                case 'T': // T Tax related if included, not tax related if omitted
                    category.TaxRelated = true;
                    break;

                case 'I': // I Income category
                    category.IsIncome = true;
                    break;

                case 'E': // E Expense category (if category type is unspecified, quicken assumes expense type)
                    category.IsIncome = false;
                    break;

                case 'B': // B Budget amount (only in a Budget Amounts QIF file)
                    category.BudgetAmount = decimal.Parse(item.Value, CultureInfo.InvariantCulture);
                    break;

                case 'R': // R Tax schedule information
                    category.TaxSchedule = item.Value;
                    break;

                case '^': // ^ End of entry
                    document.Categories.Add(category);
                    category = new QifCategory();
                    break;

                case '!':
                    break;

                default:
                    throw new InvalidOperationException("Invalid token '" + item.Text + "'.");
            }

            item = await this.ReadLineAsync(cancellationToken).ConfigureAwait(false);
        }

        this.PushBack(item);
    }

    /// <summary>
    /// Reads a line from the QIF file.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation whose result is a <see cref="QifItem" /> referencing the line of text.</returns>
    private async Task<QifItem> ReadLineAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        this.lineNumber++;

        if (this.stack.Count > 0)
        {
            var item = this.stack.Pop();
            return item;
        }
        else
        {
            var line = await this.reader.ReadLineAsync().ConfigureAwait(false);
            var item = new QifItem(line);
            return item;
        }
    }

    /// <summary>
    /// Reads tag information from the QIF file.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    /// <param name="document">A <see cref="QifDocument" /> to contain the account information.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation requests.</param>
    private async Task ReadTagAsync(QifDocument document, CancellationToken cancellationToken)
    {
        var tag = new QifTag();
        var item = await this.ReadLineAsync(cancellationToken).ConfigureAwait(false);

        while (!item.EndOfRecord)
        {
            switch (item.Field)
            {
                case 'N': // N Tag name
                    tag.Name = item.Value;
                    break;

                case '^': // ^ End of entry
                    document.Tags.Add(tag);
                    tag = new QifTag();
                    break;

                case '!':
                    break;

                default:
                    throw new InvalidOperationException("Invalid token '" + item.Text + "'.");
            }

            item = await this.ReadLineAsync(cancellationToken).ConfigureAwait(false);
        }

        this.PushBack(item);
    }

    /// <summary>
    /// Reads a transaction from the QIF file.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    /// <param name="document">A <see cref="QifDocument" /> to contain the account information.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" /> to monitor for cancellation requests.</param>
    private async Task ReadTransactionAsync(QifDocument document, CancellationToken cancellationToken)
    {
        var transaction = new QifTransaction();
        var split = new QifSplit();
        var item = await this.ReadLineAsync(cancellationToken).ConfigureAwait(false);

        while (!item.EndOfRecord)
        {
            switch (item.Field)
            {
                case 'D': // Date
                    transaction.Date = DateTime.Parse(item.Value.Replace('\'', '/'), CultureInfo.InvariantCulture);
                    break;

                case 'U': // Undocumented
                    transaction.Unknown = decimal.Parse(item.Value, CultureInfo.InvariantCulture);
                    break;

                case 'T': // Amount
                    transaction.Amount = decimal.Parse(item.Value, CultureInfo.InvariantCulture);
                    break;

                case 'C': // Cleared status
                    transaction.Cleared = item.Value[0];
                    break;

                case 'N': // Num (check or reference number)
                    transaction.Number = item.Value;
                    break;

                case 'P': // Payee
                    transaction.Payee = item.Value;
                    break;

                case 'M': // M Memo
                    transaction.Memo = item.Value;
                    break;

                case 'A': // Address (up to five lines; the sixth line is an optional message)
                    transaction.Address = item.Value;
                    break;

                case 'L': // Category (Category/Subcategory/Transfer/Class)
                    transaction.Category = item.Value;
                    break;

                case 'S': // Category in split (Category/Transfer/Class)
                    split.Category = item.Value;
                    break;

                case 'E': // Memo in split
                    split.Memo = item.Value;
                    break;

                case 'Y': // Security name
                    transaction.SecurityName = item.Value;
                    break;

                case 'Q': // Quantity of shares
                    transaction.QuantityOfShares = decimal.Parse(item.Value, CultureInfo.InvariantCulture);
                    break;

                case 'I': // Security price
                    transaction.SecurityPrice = decimal.Parse(item.Value, CultureInfo.InvariantCulture);
                    break;

                case '$': // Dollar amount of split
                    split.Amount = decimal.Parse(item.Value, CultureInfo.InvariantCulture);
                    transaction.Splits.Add(split);
                    split = new QifSplit();
                    break;

                case '^': // ^ End of entry
                {
                    transaction.Validate("Split total does not match transaction amount at line " + this.lineNumber + ".");

                    if (this.currentAccount == null)
                    {
                        this.currentAccount = new QifAccount();
                        document.Accounts.Add(this.currentAccount);
                    }

                    this.currentAccount.Transactions.Add(transaction);
                    transaction = new QifTransaction();
                    break;
                }

                default:
                    throw new InvalidOperationException("Invalid token '" + item.Text + "' at line " + this.lineNumber + ".");
            }

            item = await this.ReadLineAsync(cancellationToken).ConfigureAwait(false);
        }

        this.PushBack(item);
    }
}