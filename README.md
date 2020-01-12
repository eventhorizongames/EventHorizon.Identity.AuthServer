
# Migration Notes

Need to make sure AutoHistory is not created during any migrations other than the ApplicationDb Context Migration scripts.

## Create Migration

dotnet ef migrations add DotnetUpdate --context ApplicationDbContext --output-dir Migrations\ApplicationDb

dotnet ef migrations add DotnetUpdate --context HistoryExtendedConfigurationDbContext --output-dir Migrations\ConfigurationDb

dotnet ef migrations add DotnetUpdate --context PersistedGrantDbContext --output-dir Migrations\PersistedGrantDb

## Update Connected DB

dotnet run /initdb