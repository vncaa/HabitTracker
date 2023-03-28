using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

Console.Clear();

List<string> indexInDB = new List<string>();

string connectionString = @"Data Source=(localdb)\MSSQLLocalDB";

CreateDatabase();
LoadIds();
UserMenu();

#region Fce
void UserMenu()
{
    bool appRunning = true;
    while (appRunning)
    {
        ShowMenu();
        Console.Write("> ");
        string userInput = Console.ReadLine();
        switch (userInput)
        {
            case "0":
                appRunning = false;
                break;
            case "1":
                ViewRecord();
                break;
            case "2":
                InsertRecord();
                break;
            case "3":
                DeleteRecord();
                break;
            default:
                break;

        }
    }

}

void ShowMenu()
{
    Console.Clear();
    Console.WriteLine("Welcome to WALKING HABIT tracker.\n");
    Console.WriteLine("Type 1 to VIEW/UPDATE RECORDS");
    Console.WriteLine("Type 2 to INSERT RECORD");
    Console.WriteLine("Type 3 to DELETE RECORD");
    Console.WriteLine("Type 0 to CLOSE APP");
    Console.WriteLine("----------------------------------");

}

void CreateDatabase()
{
    //quantity in string - issue with saving float with a comma insted of a dot into a db
    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        string queryString = @"IF OBJECT_ID(N'walkingHabit', N'U') IS NULL
                               CREATE TABLE walkingHabit
                               (
	                            [Id] INT NOT NULL PRIMARY KEY IDENTITY, 
                                [Date] NVARCHAR(50) NOT NULL,
                                [Quantity] NVARCHAR(50) NOT NULL 
                               )";

        SqlCommand command = new SqlCommand(queryString, connection);
        connection.Open();
        command.ExecuteNonQuery();
        connection.Close();

        Console.WriteLine("DB created/loaded.\n");
    }
}

void LoadIds()
{
    //loading ids into a list for later searching for existing and correct ids
    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        string queryString = "SELECT * FROM [walkingHabit]";
        SqlCommand command = new SqlCommand(queryString, connection);
        connection.Open();

        using (SqlDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                indexInDB.Add($"{reader[0]}");
            }

        }
    }
}

void ViewRecord()
{
    Console.Clear();
    bool noRows = false;

    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        string queryString = "SELECT * FROM [walkingHabit]";
        SqlCommand command = new SqlCommand(queryString, connection);
        connection.Open();

        using (SqlDataReader reader = command.ExecuteReader())
        {
            if (!reader.HasRows)
            {
                Console.WriteLine("No habit has been added.");
                noRows = true;
            }
            else
            {
                while (reader.Read())
                {
                    Console.WriteLine($"{reader[0]}: {reader[1]} - {reader[2]}km");
                }
                Console.WriteLine();
            }

        }
        connection.Close();
        if (noRows == false)
        {
            OrderRecordsMenu(); //+update fce
        }
        else
        {
            Console.WriteLine("\nEnter - MAIN MENU");
            Console.ReadKey();
        }  
    }
}
#region OrderingRecordsBy
void OrderRecordsMenu()
{
    string orderById = "Id";
    string orderByDate = "Date";
    string orderByQuantity = "Quantity";
    bool orderRunning = true;

    while (orderRunning)
    {
        Console.WriteLine("Order records by:");
        Console.WriteLine("Type 1 - Id");
        Console.WriteLine("Type 2 - Date");
        Console.WriteLine("Type 3 - Quantity");
        Console.WriteLine("------------------------");
        Console.WriteLine("Type 4 - Update a record");
        Console.WriteLine("Type 0 - Main Menu");
        Console.Write("> ");
        string orderChoice = Console.ReadLine();
        Console.WriteLine();

        switch (orderChoice)
        {
            case "1":
                OrderRecordsBy(orderById);
                orderRunning = false;
                break;
            case "2":
                OrderRecordsBy(orderByDate);
                orderRunning = false;
                break;
            case "3":
                OrderRecordsBy(orderByQuantity);
                orderRunning = false;
                break;
            case "4":
                UpdateRecordById();
                orderRunning = false;
                break;
            case "0":
                orderRunning = false;
                break;
            default:
                break;
        }
    }
    
}

void OrderRecordsBy(string orderChoice)
{
    //ascending/descending
    string ascDesc = OrderAscendingDescending();
    Console.Clear();

    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        string queryString = @$"SELECT * FROM [walkingHabit]
                                ORDER BY [{orderChoice}] {ascDesc}";

        SqlCommand command = new SqlCommand(queryString, connection);
        connection.Open();

        using (SqlDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine($"{reader[0]}: {reader[1]} - {reader[2]}km");
            }
        }
        connection.Close();
    }
    Console.WriteLine("\nEnter - MAIN MENU");
    Console.ReadKey();
}

string OrderAscendingDescending()
{
    bool validInput = false;
    string ascDesc = "";

    Console.WriteLine("\nOrder by ascending or descending order: ");
    Console.WriteLine("Type 1 - Ascending Order");
    Console.WriteLine("Type 2 - Descending Order");
    Console.Write("> ");
    string input = Console.ReadLine();
    Console.WriteLine();

    while (validInput == false)
    {

        switch (input)
        {
            case "1":
                ascDesc = "ASC";
                validInput = true;
                break;
            case "2":
                ascDesc = "DESC";
                validInput = true;
                break;
            default:
                Console.WriteLine("Invalid input, please try again.");
                break;
        }
    }

    return ascDesc;
}
#endregion
void InsertRecord()
{
    Console.Clear();
    string date = "";
    bool validDate = false;

    while (!validDate)
    {
        try
        {
            date = GetDate();
            validDate = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("Invalid date, please try again.");
        }
    }


    string quantity = GetQuantity();

    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        string queryString = @$"INSERT INTO [walkingHabit] (Date, Quantity)
                               VALUES ('{date}', '{quantity}')";
        SqlCommand command = new SqlCommand(queryString, connection);
        connection.Open();
        try
        {
            command.ExecuteNonQuery();
            Console.WriteLine("\nRecord successfully added.");
            Console.WriteLine("\nEnter - MAIN MENU");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        connection.Close();
    }
}

#region DateAndQuantity
string GetQuantity()
{
    bool validInput;
    float quantity;

    //converting to float to check for valid quantity
    Console.Write("Enter the quantity in km (use a comma for a decimal point): ");
    validInput = float.TryParse(Console.ReadLine(), out quantity);
    while (validInput == false)
    {
        Console.WriteLine("Invalid input, please try again.");
        Console.Write("Enter the quantity in km: ");
        validInput = float.TryParse(Console.ReadLine(), out quantity);
    }
    //replacing comma with a dot
    string quantityString = quantity.ToString();
    if (quantityString.Contains(','))
        quantityString = quantityString.Replace(',', '.');

    return quantityString;
}
string GetDate()
{
    Console.Write("Enter the date (dd-mm-yyyy): ");
    string date = Console.ReadLine();
    while (!IsValidDate(date))
    {
        Console.WriteLine("Invalid date, please try again.\n");
        Console.Write("Enter the date (dd-mm-yyyy): ");
        date = Console.ReadLine();
    }
    //if user intpus dot or slash instead of dash
    if (date.Contains("."))
        date = date.Replace('.', '-');
    else if (date.Contains("/"))
        date = date.Replace('/', '-');

    return date;
}
bool IsValidDate(string date)
{
    var successDay = int.TryParse(date.Substring(0, 2), out int day);
    var successMonth = int.TryParse(date.Substring(3, 2), out int month);
    var successYear = int.TryParse(date.Substring(6), out int year);

    if (!successDay || !successMonth || !successYear && date.Substring(6).Length != 4)
    {
        return false;
    }

    return IsValidDay(day, month, year);
}
bool IsValidDay(int day, int month, int year)
{
    switch (month)
    {
        case 2:
            if (IsLeapYear(year) && day > 29 || !IsLeapYear(year) && day > 28)
                return false;
            break;
        case 4:
        case 6:
        case 9:
        case 11:
            if (day > 30)
            {
                return false;
            }
            break;
        case 1:
        case 3:
        case 5:
        case 7:
        case 8:
        case 10:
        case 12:
            if (day > 31)
            {
                return false;
            }
            break;
        default:
            return false;
    }

    return true;
}
bool IsLeapYear(int year)
{
    return year % 4 == 0 && year % 100 != 0 || year % 400 == 0;
}
#endregion region
void UpdateRecordById()
{
    string id = ExistingId("update");

    string date = "";
    bool validDate = false;

    while (!validDate)
    {
        try
        {
            date = GetDate();
            validDate = true;
        }
        catch
        {
            Console.WriteLine("Invalid date, please try again.\n");
        }
    }

    string quantity = GetQuantity();

    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        string queryString = @$"UPDATE [walkingHabit]
                                SET [Date] = '{date}', [Quantity] = '{quantity}'
                                WHERE [Id] = {id}";

        SqlCommand command = new SqlCommand(queryString, connection);
        connection.Open();
        command.ExecuteNonQuery();
        connection.Close();
    }
    Console.WriteLine("\nRecord successfully updated.\n");
    Console.WriteLine("ENTER - Main Menu");
    Console.ReadKey();
}

void DeleteRecord()
{
    DeleteRecordView();
    string id = ExistingId("delete");

    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        string queryString = @$"DELETE FROM [walkingHabit]
                                WHERE [Id] = {id}";

        SqlCommand command = new SqlCommand(queryString, connection);
        connection.Open();
        command.ExecuteNonQuery();
        connection.Close();
    }
    Console.WriteLine("\nRecord successfully deleted.\n");
    Console.WriteLine("ENTER - Main Menu");
    Console.ReadKey();
}

string ExistingId(string edit)
{
    bool idFound = false;
    string inputId = "";

    while (idFound == false)
    {
        Console.WriteLine($"Type an ID of a record you wish to {edit}:");
        Console.Write("> ");
        inputId = Console.ReadLine();

        foreach (string s in indexInDB)
        {
            if (s == inputId)
            {
                idFound = true;
                break;
            }
        }
        if(idFound==false)
            Console.WriteLine("Invalid Id, please try again.\n");
    }
    return inputId;
}

void DeleteRecordView()
{
    Console.Clear();
    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        string queryString = "SELECT * FROM [walkingHabit]";
        SqlCommand command = new SqlCommand(queryString, connection);
        connection.Open();

        using (SqlDataReader reader = command.ExecuteReader())
        {
            if (!reader.HasRows)
            {
                Console.WriteLine("No habit has been added.");
            }
            else
            {
                while (reader.Read())
                {
                    Console.WriteLine($"{reader[0]}: {reader[1]} - {reader[2]}km");
                }
                Console.WriteLine();
            }

        }
        connection.Close();
    }
}
#endregion