# Operations

How to run, test, and maintain Swiss Event Permit Assistant. For shipping a release to Azure, see [deployment.md](deployment.md).

## Local Development

From the repository root:

```bash
dotnet restore
dotnet run --project src/SwissEventPermitAssistant.Web
```

The application will print the local development URL after startup.

## Testing

Run the test suite with:

```bash
dotnet test SwissEventPermitAssistant.slnx --configuration Release
```

For release verification:

```bash
dotnet restore
dotnet build SwissEventPermitAssistant.slnx --configuration Release --no-restore
dotnet test SwissEventPermitAssistant.slnx --configuration Release --no-build
dotnet publish src/SwissEventPermitAssistant.Web/SwissEventPermitAssistant.Web.csproj \
  --configuration Release \
  --output ./artifacts/publish
```

## Production Health Check

```bash
curl -i https://sepa-fribourg-jinyan.azurewebsites.net/healthz
```

The endpoint should return a successful response. The application is deployed over HTTPS and does not store permit dossiers on the server.

## Official Source Maintenance

Official-source references are maintained in:

- `docs/sources/README.md`
- `src/SwissEventPermitAssistant.Domain/Sources/OfficialSource.cs`

Sources should be reviewed before releases that change permit rules or user-facing administrative guidance.

If an official source becomes unavailable or a requirement cannot be interpreted confidently, the application should surface the affected result as requiring confirmation rather than infer a requirement.

## Current Production Boundaries

The current release does not provide:

- official application submission
- authentication or user accounts
- server-side dossier storage
- document upload
- PDF form filling
- automated authority integration
- legal advice
- approval guarantees

Operational secrets, credentials, Azure publish profiles, and local environment files must not be committed to the repository.
