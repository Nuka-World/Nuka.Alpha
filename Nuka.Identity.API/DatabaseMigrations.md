Create migrations:
```
$ dotnet ef migrations add InitialApplicationDbMigration -c ApplicationDbContext -o Migrations\Applications
$ dotnet ef migrations add InitialPersistedGrantDbMigration -c PersistedGrantDbContext -o Migrations\PersistedGrants
$ dotnet ef migrations add InitialConfigurationDbMigration -c ConfigurationDbContext -o Migrations\Configurations
```
(Option) Create your databases by calling dotnet ef database update on each of the DB contexts.
You also can use DBContext.Database.Migrate() in code to update database.
```
$ dotnet ef database update -c ApplicationDbContext
$ dotnet ef database update -c PersistedGrantDbContext
$ dotnet ef database update -c ConfigurationDbContext
```