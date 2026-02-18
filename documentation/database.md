# Database
## Migrations
This guide outlines how to manage database migrations using Entity Framework Core in the Umiko project.

### Pre-requisites
Ensure you have the dotnet-ef tool installed globally:
```bash
dotnet tool install --global dotnet-ef
```
If the tool is already installed, update it to the latest version:
```bash
dotnet tool update --global dotnet-ef
```

### Migrations
All commands should be run from the `source/MadWorldNL.Umiko.Controllers.Api` directory (the startup project).

#### Create Migration
```bash
dotnet ef migrations add <MigrationName> --context UmikoContext --project ../MadWorldNL.Umiko.Infrastructures.Postgresql -o ../MadWorldNL.Umiko.Infrastructures.Postgresql/Migrations
```

#### Apply Migration
To apply the created migration to the database:
```bash
dotnet ef database update --context UmikoContext --project ../MadWorldNL.Umiko.Infrastructures.Postgresql
```

#### Rollback
##### Listing All Migrations
To view all migrations:
```bash
dotnet ef migrations list --context UmikoContext --project ../MadWorldNL.Umiko.Infrastructures.Postgresql
```

##### Rolling Back to a Specific Migration
To rollback to a specific migration (e.g., InitialCreate):
```bash
dotnet ef database update InitialCreate --context UmikoContext --project ../MadWorldNL.Umiko.Infrastructures.Postgresql
```

##### Rolling Back All Migrations
To revert the database to its initial state (no migrations applied):
```bash
dotnet ef database update 0 --context UmikoContext --project ../MadWorldNL.Umiko.Infrastructures.Postgresql
```

#### Removing the Last Migration
If you need to remove the last migration (without applying it to the database):
```bash
dotnet ef migrations remove --context UmikoContext --project ../MadWorldNL.Umiko.Infrastructures.Postgresql
```