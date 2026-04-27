using Microsoft.Data.Sqlite;

namespace SwiftMt103.Api.Data;

public class Database
{
    private readonly string _connectionString = "Data Source=swift.db";

    public void Initialize()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS SwiftMessages (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                TransactionReference TEXT,
                Currency TEXT,
                Amount TEXT,
                OrderingCustomer TEXT,
                BeneficiaryCustomer TEXT,
                RawMessage TEXT
            );
        ";

        command.ExecuteNonQuery();
    }

    public void Insert(
        string transactionRef,
        string currency,
        string amount,
        string orderingCustomer,
        string beneficiaryCustomer,
        string raw)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO SwiftMessages
            (TransactionReference, Currency, Amount, OrderingCustomer, BeneficiaryCustomer, RawMessage)
            VALUES ($ref, $cur, $amt, $ord, $ben, $raw);
        ";

        command.Parameters.AddWithValue("$ref", transactionRef ?? "");
        command.Parameters.AddWithValue("$cur", currency ?? "");
        command.Parameters.AddWithValue("$amt", amount ?? "");
        command.Parameters.AddWithValue("$ord", orderingCustomer ?? "");
        command.Parameters.AddWithValue("$ben", beneficiaryCustomer ?? "");
        command.Parameters.AddWithValue("$raw", raw ?? "");

        command.ExecuteNonQuery();
    }

    public List<Dictionary<string, object>> GetAll()
{
    var result = new List<Dictionary<string, object>>();

    using var connection = new SqliteConnection(_connectionString);
    connection.Open();

    var command = connection.CreateCommand();
    command.CommandText = "SELECT * FROM SwiftMessages";

    using var reader = command.ExecuteReader();

    while (reader.Read())
    {
        var row = new Dictionary<string, object>();

        for (int i = 0; i < reader.FieldCount; i++)
        {
            row[reader.GetName(i)] = reader.GetValue(i);
        }

        result.Add(row);
    }

    return result;
}
}