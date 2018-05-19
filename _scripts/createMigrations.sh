# Initial Migrations
dotnet ef -v migrations add InitialCreate -c ApplicationDbContext -o Migrations/ApplicationDb
dotnet ef -v migrations add InitialConfigurationDbMigration -c HistoryExtendedConfigurationDbContext -o Migrations/ConfigurationDb
dotnet ef -v migrations add InitialPersistedGrantDbMigration -c PersistedGrantDbContext -o Migrations/PersistedGrantDb

# History
dotnet ef -v migrations add UserHistory -c ApplicationDbContext -o Migrations/ApplicationDb

## Future Migrations