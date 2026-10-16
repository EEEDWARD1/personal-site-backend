# Personal Site API

A .NET 10 Minimal API for personal-site posts and tags, backed by PostgreSQL.

## Run locally

1. Copy `.env.example` to `.env` and set the values.
2. Start the API and database:

   ```bash
   docker compose up --build
   ```

The API is available at `http://localhost:8080`. In Development, Scalar API docs are available at `http://localhost:8080/scalar`.

## Endpoints

- `GET /api/v1/posts` — paginated post summaries; supports `type`, `tag`, `isFeatured`, and `enableTags` filters/options.
- `GET /api/v1/posts/titles` — paginated post titles.
- `GET /api/v1/posts/{slug}` — a published post with tags.
- `GET /api/v1/tags` — paginated tags.

Repeated `tag` parameters match posts with **any** requested tag, for example:

```text
/api/v1/posts?tag=dotnet&tag=postgres&enableTags=true
```

## Tests

```bash
dotnet test services/api/EduardTeodor.slnx
```

## Deployment

Pushing to `main` builds and deploys the production Compose stack. This workflow requires a Linux x64 self-hosted GitHub Actions runner on the deployment server, with Docker and the Compose plugin installed and permission to run Docker commands.

Add these repository or `production` environment secrets in GitHub:

- `POSTGRES_USER`
- `POSTGRES_PASSWORD`
- `POSTGRES_DB`
- `JWT_KEY`
- `CLOUDFLARE_TUNNEL_TOKEN`

Register the runner for this repository and give it the `self-hosted`, `linux`, and `x64` labels. The production workflow uses `compose.yml` and `compose.prod.yml`; it does not remove the database volume.
