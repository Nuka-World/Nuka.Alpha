Create migrations:
```
$ dotnet ef migrations add InitialIdentityServerMigration -c ApplicationDbContext -o Migrations\Applications
$ dotnet ef migrations add InitialIdentityServerMigration -c PersistedGrantDbContext -o Migrations\PersistedGrants
$ dotnet ef migrations add InitialIdentityServerMigration -c ConfigurationDbContext -o Migrations\Configurations
```
Create your databases by calling dotnet ef database update on each of the DB contexts
```
$ dotnet ef database update -c ApplicationDbContext
$ dotnet ef database update -c PersistedGrantDbContext
$ dotnet ef database update -c ConfigurationDbContext
```