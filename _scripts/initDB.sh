docker run -v $PWD/appsettings.json:/app/appsettings.Production.json -e ASPNETCORE_ENVIRONMENT=Production --entrypoint dotnet ehz/identity/authserver EventHorizon.Identity.AuthServer.dll /initdb