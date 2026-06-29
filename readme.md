# Personal Site Backend

Docker Compose stack for the personal site API, PostgreSQL database, and
Cloudflare Tunnel.

The active Spring Boot API lives in `api/`. The deprecated Java backend has
been moved out of the runtime path and is not used by Compose.

## Services

- `postgres` - PostgreSQL 17 with a persistent `postgres_data` volume.
- `api` - Spring Boot API built from `./api`.
- `cloudflared` - Cloudflare Tunnel connector for public access.

Inside Docker, Cloudflare should route to:

```text
http://api:8080
```

No API port is published to the host. The API is exposed to Cloudflare only on
the private Compose network.

## Required Environment

Copy the template and fill in real values:

```bash
cp .env.example .env
```

Required values:

```env
TUNNEL_TOKEN=replace-with-your-cloudflare-tunnel-token
DB_NAME=personal_site
DB_USER=personal_site
DB_PASSWORD=replace-with-a-strong-password
JWT_SECRET=replace-with-a-long-random-secret
```

`DB_HOST` and `DB_PORT` are set by Docker Compose for the API service, so local
development values in `.env` do not need to match the container network.

## Cloudflare Tunnel

Create a remotely managed tunnel in Cloudflare Zero Trust:

1. Open Cloudflare Dashboard > Zero Trust > Networks > Tunnels.
2. Create a Cloudflared tunnel.
3. Choose Docker as the connector environment.
4. Copy the tunnel token into `TUNNEL_TOKEN` in `.env`.
5. Add a public hostname:

```text
Hostname: api.eduardteodor.co.uk
Type: HTTP
URL: http://api:8080
```

The root `config.yml` shows the equivalent ingress rule for reference. The
Compose setup uses the token-based remotely managed tunnel, so the active route
is configured in Cloudflare.

## Run

Build and start the stack:

```bash
docker compose up -d --build
```

Check status:

```bash
docker compose ps
```

View logs:

```bash
docker compose logs -f api
docker compose logs -f cloudflared
```

Stop the stack:

```bash
docker compose down
```

Stop and remove the database volume:

```bash
docker compose down -v
```

## API Configuration

The API reads database and JWT settings from environment variables:

- `DB_HOST`
- `DB_PORT`
- `DB_NAME`
- `DB_USER`
- `DB_PASSWORD`
- `JWT_SECRET`

In Compose, the API connects to PostgreSQL at `postgres:5432`. Flyway migrations
run automatically on startup.

## Useful Local Commands

Run API tests from the `api` folder:

```bash
cd api
./gradlew test
```

Build the API jar:

```bash
cd api
./gradlew bootJar
```

Follow all container logs:

```bash
docker compose logs -f
```

## Security Notes

- Keep `.env` out of git.
- Treat `TUNNEL_TOKEN`, `DB_PASSWORD`, and `JWT_SECRET` as secrets.
- Rotate the Cloudflare tunnel token if it is exposed.
- Do not add a public `ports:` mapping for the API in production.
- Keep admin API routes protected behind JWT authentication and consider
  Cloudflare Access for extra protection.

## Troubleshooting

If the API cannot connect to Postgres, check:

```bash
docker compose logs --tail=100 postgres
docker compose logs --tail=100 api
```

If the public hostname fails, confirm the Cloudflare public hostname points to:

```text
http://api:8080
```

Then inspect the tunnel logs:

```bash
docker compose logs --tail=100 cloudflared
```
