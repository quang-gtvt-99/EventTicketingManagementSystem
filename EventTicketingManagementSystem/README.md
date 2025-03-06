# Execute the following command to apply Migration to the database
Add-Migration -Name <MigrationName> -OutputDir Migrations -Project EventTicketingManagementSystem.Data
Update-Database -Migration <MigrationName> -Project EventTicketingManagementSystem.Data
Update-Database -Project EventTicketingManagementSystem.Data
Remove-Migration -Project EventTicketingManagementSystem.Data
Drop-Database -Project EventTicketingManagementSystem.Data

  
