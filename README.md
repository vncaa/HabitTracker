# HabitTracker

HabitTracker - Record your walking!

My first console based CRUD application to track a walking habit.
Developed using C# and MSSQL.

## Requirements

- [x] When the application starts, it should create a SQL database, if one isn’t present.
- [x] It should also create a table in the database, where the habit will be logged.
- [x] The app should show the user a menu of options.
- [x] The users should be able to insert, delete, update and view their logged habit.
- [x] You should handle all possible errors so that the application never crashes.
- [x] You can only interact with the database using raw SQL. You can’t use mappers such as Entity Framework.
- [x] The habit can't be tracked by time (ex. hours of sleep), only by quantity (ex. number of water glasses a day)

## Features

- MSSQL database connection
   - The application uses SQL db connection to store and read information.
   - If no db or table exists, they will be created when the program starts.
   - For managing the db, a SQL Management Studio was used.
   - Using a LocalDB as a server.

- A console based UI where a user can navigate by typing numbers/keys

![usermenu](https://user-images.githubusercontent.com/114943386/229108826-327f8b3b-6f1d-41b0-9c65-3ae37339028b.png)

- Ability to sort all records by ID, QUANTITY or DATE in ascending or descending order

![sorting](https://user-images.githubusercontent.com/114943386/229110049-15edc7e8-2bca-40b7-a45a-442ab40e42d5.png)

- Updating or deleting a record from available records.

## Challenges

- It was my first time using MSSQL in a real application not just entering queries in DB Browser.
- Goal was not to make a spaghetti code and use different classes but this is a very simple application so I was just focusing on learning to work with a db.
- An issue for me was learning to use proper SQL commands, the application kept shuting down with errors end exceptions. So in some problematic areas
I used try/catch blocks for keeping the app running. Also inserting variables into a SQL query was quite tricky at the beginning.
- When I gave my friends the app to test it out, they always pressed something they shouldn't so I made sure to foolproof the app so an error will never show up. (Atleast I hope)

## Areas to Improve

- Using raw SQL commands and Transact-SQL in the code is not the way to go so I will be using something like EntityFramework next time. For obvious reasons...
- Definitely the next step would be to use OOP, split the functions into different classes. But like I said that was not the main focus of this exercise.
- Making a better, more effective and simpler functions. I feel like in my codes and projects there is way too much fluff than it should be.
- Adding a possibility for the user to add custom habits and units.

## Resources Used

- [TheCSharpAcademy](https://www.thecsharpacademy.com/#) - Instructions fot the project
- [MSSQLTips.com](https://www.mssqltips.com/sqlservertip/7220/sql-server-connection-strings-reference-guide/)
- [Microsoft Learn](https://learn.microsoft.com/)
- Various StackOverflow articles
