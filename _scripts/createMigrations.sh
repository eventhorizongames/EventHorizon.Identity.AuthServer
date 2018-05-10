# Initial Migrations
dotnet ef -v migrations add InitialCreate -c ApplicationDbContext -o Migrations/ApplicationDb
dotnet ef -v migrations add InitialConfigurationDbMigration -c ConfigurationDbContext -o Migrations/ConfigurationDb
dotnet ef -v migrations add InitialPersistedGrantDbMigration -c PersistedGrantDbContext -o Migrations/PersistedGrantDb

## Future Migrations