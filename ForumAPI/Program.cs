using Microsoft.Data.Sqlite;
using Dapper;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình đường dẫn cho file Database SQLite 
var connectionString = "Data Source=app.db";
builder.Services.AddTransient(sp => new SqliteConnection(connectionString));

var app = builder.Build();

// Dapper tự động tạo bảng khi chạy project lần đầu
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SqliteConnection>();
    db.Open();
    db.Execute(@"
        CREATE TABLE IF NOT EXISTS Users (
            Id INTEGER PRIMARY KEY AUTOINCREMENT, 
            Username TEXT NOT NULL
        );
        CREATE TABLE IF NOT EXISTS Tags (
            Id INTEGER PRIMARY KEY AUTOINCREMENT, 
            Name TEXT NOT NULL
        );
    ");
}

app.MapGet("/", () => "Hệ thống Backend diễn đàn đang chạy!");
app.Run();