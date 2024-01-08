using Dapper;
using FlashCards.Doc415.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace FlashCards.Doc415;

internal class DataAccess
{
    IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

    private string ConnectionString;

    public DataAccess()
    {
        ConnectionString = configuration.GetSection("ConnectionStrings")["DefaultConnection"];
    }

    internal void CreateTables()
    {
        try
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                string createStackTableSql = @"IF NOT EXISTS (Select * FROM sys.tables WHERE name='Stacks')
                                            CREATE TABLE Stacks(
                                            ID int IDENTITY (1,1) NOT NULL,
                                            Name NVARCHAR(30) NOT NULL UNIQUE,
                                            Primary Key(Id)
                                            );";
                conn.Execute(createStackTableSql);

                string createFlashcardTableSql = @"IF NOT EXISTS (Select * FROM sys.tables WHERE name='Flashcards')
                                            CREATE TABLE Flashcards(
                                            ID int IDENTITY (1,1) NOT NULL PRIMARY KEY,
                                            Question NVARCHAR(30) NOT NULL,
                                            Answer NVARCHAR(30) NOT NULL,
                                            StackId int NOT NULL
                                                FOREIGN KEY 
                                                REFERENCES Stacks(Id)
                                                ON DELETE CASCADE
                                                ON UPDATE CASCADE
                                            );";

                conn.Execute(createFlashcardTableSql);

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"There was a problem creating the tables: {ex.Message}");
        }
    }

    internal void AddStack(CardStack _stack)
    {
        try
        {
            using var connection = new SqlConnection(ConnectionString);
            connection.Open();
            string insertQuery = @"INSERT INTO Stacks (name) Values (@Name)";
            connection.Execute(insertQuery, new { _stack.Name });

        }
        catch (Exception ex)
        {
            Console.WriteLine($"There was a problem inserting the stack: {ex.Message}");
        }
    }

    internal IEnumerable<CardStack> GetAllStacks()
    {
        try
        {
            using var connection = new SqlConnection(ConnectionString);
            connection.Open();
            string getAllStacksQuery = @"SELECT * FROM Stacks";
            var records = connection.Query<CardStack>(getAllStacksQuery);
            return records;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"There was a problem getting stack list: {ex.Message}");
            Console.ReadLine();
            return new List<CardStack>();
        }
    }

   /* internal string GetStackById(int Id)
    {
        using var connection = new SqlConnection(ConnectionString);
        connection.Open();
        string getStackByIdQuery = $"SELECT Name FROM Stacks WHERE Id=@Id";
        var stackName=connection.Query<string>(getStackByIdQuery,new {Id}).FirstOrDefault();
        return stackName;
        
    }*/

    internal void AddFlashcard(Flashcard _flashcard)
    {
        try
        {
            using var connection = new SqlConnection(ConnectionString);
            connection.Open();
            string addFlashCardQuery = @"INSERT INTO Flashcards (Question,Answer,StackId) VALUES (@Question,@Answer,@StackId)";
            connection.Execute(addFlashCardQuery, new
            {
                _flashcard.Question,
                _flashcard.Answer,
                _flashcard.StackId
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"There was a problem inserting the flashcard: {ex.Message}");
            Console.ReadLine();
        }
    }

    internal IEnumerable<Flashcard> GetAllFlashcards()
    {
        try
        {
            using var connection = new SqlConnection(ConnectionString);
            connection.Open();
            string getAllStacksQuery = @"SELECT * FROM Flashcards";
            var records = connection.Query<Flashcard>(getAllStacksQuery);
            return records;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"There was a problem getting stack list: {ex.Message}");
            Console.ReadLine();
            return new List<Flashcard>();
        }
    }
}
