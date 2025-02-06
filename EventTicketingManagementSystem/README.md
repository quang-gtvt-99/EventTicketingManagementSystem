# Execute the following command to apply Migration to the database
dotnet ef database update
# Add connectString to file appsettings.json
  "ConnectionStrings": {
    "DefaultConnection": "Server=LocalServer;Database=LocalDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },