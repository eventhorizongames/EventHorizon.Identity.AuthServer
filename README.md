
# Migration Notes

Need to make sure AutoHistory is not created during any migrations other than the ApplicationDb Context Migration scripts.

## Create Migration

~~~ bash
# Install EF Tools
dotnet tool install --global dotnet-ef --version 5.0.4
~~~

~~~ bash
# Update Database to current migrations
dotnet ef database update --context ApplicationDbContext

dotnet ef database update --context HistoryExtendedConfigurationDbContext

dotnet ef database update --context PersistedGrantDbContext

~~~

~~~ bash
dotnet ef migrations add ProjectDependencyUpdate --context ApplicationDbContext --output-dir Migrations\ApplicationDb

dotnet ef migrations add ProjectDependencyUpdate --context HistoryExtendedConfigurationDbContext --output-dir Migrations\ConfigurationDb

dotnet ef migrations add ProjectDependencyUpdate --context PersistedGrantDbContext --output-dir Migrations\PersistedGrantDb

~~~

## Update Connected DB

~~~ bash
dotnet run /initdb
~~~

## Create Docker Image

~~~ bash
docker build -t canhorn/ehz-identity-authserver:dev . 
~~~