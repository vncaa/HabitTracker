using System.Data.SqlClient;
using System.Reflection.Metadata.Ecma335;

Console.Clear();

string connectionString = @"Data Source=(localdb)\MSSQLLocalDB";

CreateDatabase(connectionString);
UserMenu();

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
                UpdateRecord();
                break;
            case "4":
                //DeleteRecord();
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
    Console.WriteLine("Type 0 to CLOSE APP");
    Console.WriteLine("Type 1 to VIEW ALL RECORDS");
    Console.WriteLine("Type 2 to INSERT RECORD");
    Console.WriteLine("Type 3 to UPDATE RECORD");
    Console.WriteLine("Type 4 to DELETE RECORD");
    Console.WriteLine("----------------------------------");

}

void CreateDatabase(string connectionString)
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
            }

        }
        connection.Close();
        if (noRows == false)
        {
            FilterRecords();
        }


        Console.WriteLine("\nEnter - MAIN MENU");
        Console.ReadKey();
    }
}
void FilterRecords()
{
    string filterById = "Id";
    string filterByDate = "Date";
    string filterByQuantity = "Quantity";

    Console.WriteLine("\nFilter records by:");
    Console.WriteLine("Type 1 - Id");
    Console.WriteLine("Type 2 - Date");
    Console.WriteLine("Type 3 - Quantity");
    //Console.WriteLine("Type 0 - Main Menu");
    Console.Write("> ");
    string filterChoice = Console.ReadLine();

    switch (filterChoice)
    {
        case "1":
            FilterRecordsBy(filterById);
            break;
        case "2":
            FilterRecordsBy(filterByDate);
            break;
        case "3":
            FilterRecordsBy(filterByQuantity);
            break;
        default:
            break;
    }
}

void FilterRecordsBy(string filterChoice)
{
    //ascending/descending
    string ascDesc = FilterAscendingDescending();

    using(SqlConnection connection = new SqlConnection(connectionString))
    {
        string queryString = @$"SELECT * FROM [walkingHabit]
                                ORDER BY [{filterChoice}] {ascDesc}";

        SqlCommand command = new SqlCommand(queryString, connection);
        connection.Open();

        using (SqlDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine($"{reader[0]}: {reader[1]} - {reader[2]}km");
            }
        }
    }
}

string FilterAscendingDescending()
{
    bool validInput = false;
    string ascDesc = "";

    Console.Write("\nFilter by ascending or descending order (asc/desc): ");
    while(validInput == false)
    {
        string input = Console.ReadLine().ToUpper();
        if (input == "ASC" || input == "DESC")
        {
            ascDesc = input;
            validInput = true;
        }
        else
            Console.WriteLine("Invalid input, please try again.");
    }

    return ascDesc;
}

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
            Console.WriteLine("Record successfully added.");
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
        Console.WriteLine("Invalid date, please try again.");
        Console.Write("Enter the date (dd-mm-yyyy): ");
        date = Console.ReadLine();
    }
    //if user intpus dot or slash instead of dash
    if (date.Contains("."))
        date = date.Replace('.', '-');

    return date;
}

#region DateValidation
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

void UpdateRecord()
{
    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        string queryString = @$"";

        SqlCommand command = new SqlCommand(queryString, connection);
        connection.Open();
        command.ExecuteNonQuery();
        connection.Close();
    }
}

