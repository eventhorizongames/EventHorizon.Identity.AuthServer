docker run -v ~/Configs/Identity/appsettings.Production.json:/app/appsettings.Production.json \
-v ~/Configs/Identity/admins.Production.json:/app/admins.Production.json \
-v ~/EventHorizon.Keys/AuthServer/certificate.pfx:/app/certificate.pfx \
-e ASPNETCORE_ENVIRONMENT=Production \
--entrypoint dotnet ehz/identity/authserver EventHorizon.Identity.AuthServer.dll /initdb