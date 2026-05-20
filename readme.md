# Personal Site API Cloudflare Tunnel

Production-ready Docker Compose setup for exposing the backend API at:

```text
https://api.eduardteodor.co.uk
```

The Java backend listens only on Docker's private network at `http://backend:8000`.
No backend ports are published to the host. Cloudflare Tunnel is the only public
entry point.

## Architecture

```text
Internet
  -> Cloudflare
  -> Cloudflare Tunnel
  -> cloudflared container
  -> backend container on backend_private network
```

Cloudflare Tunnel uses outbound-only connections from `cloudflared` to
Cloudflare, so no ports need to be opened on the firewall or router.

## Files

- `docker-compose.yml` - Cloudflare Tunnel plus the internal Java backend.
- `.env.example` - Template for the tunnel token.
- `.gitignore` - Excludes environment files, Cloudflare credentials, and local secrets.
- `config.yml` - Optional ingress reference for locally managed tunnels.

## Prerequisites

- A Cloudflare account.
- `eduardteodor.co.uk` added to Cloudflare as a zone.
- DNS for `eduardteodor.co.uk` managed by Cloudflare nameservers.
- Docker Engine and Docker Compose v2 installed on the host.
- Outbound internet access from the Docker host.

## Add the domain to Cloudflare

1. Log in to the Cloudflare dashboard.
2. Select **Add a domain**.
3. Enter `eduardteodor.co.uk`.
4. Choose the appropriate plan.
5. Review imported DNS records.
6. Update the domain registrar nameservers to the Cloudflare nameservers shown in the dashboard.
7. Wait until Cloudflare marks the zone as active.

## Create the tunnel in Cloudflare Zero Trust

1. Open the Cloudflare dashboard.
2. Go to **Zero Trust**.
3. Go to **Networks** or **Networking** > **Tunnels**.
4. Select **Create a tunnel**.
5. Choose **Cloudflared**.
6. Name the tunnel, for example `personal-site-api-production`.
7. Save the tunnel.
8. Choose **Docker** as the connector environment.

## Obtain the tunnel token

Cloudflare shows a Docker command similar to:

```bash
docker run cloudflare/cloudflared:latest tunnel --no-autoupdate run --token <TOKEN>
```

Copy only the token value. It usually starts with `eyJ`.

Create your local `.env` file:

```bash
cp .env.example .env
```

Edit `.env`:

```env
TUNNEL_TOKEN=eyJ...
```

Treat this token like a password. Anyone with the token can run a connector for
the tunnel.

## Create the DNS route for api.eduardteodor.co.uk

For a remotely managed tunnel, configure ingress in Cloudflare Zero Trust:

1. Open **Zero Trust** > **Networks** or **Networking** > **Tunnels**.
2. Select the tunnel.
3. Go to **Public Hostnames**.
4. Select **Add a public hostname**.
5. Set:
   - Subdomain: `api`
   - Domain: `eduardteodor.co.uk`
   - Type: `HTTP`
   - URL: `backend:8000`
6. Save the hostname.

Cloudflare will create the DNS route for:

```text
api.eduardteodor.co.uk
```

The optional `config.yml` shows the equivalent ingress shape:

```yaml
ingress:
  - hostname: api.eduardteodor.co.uk
    service: http://backend:8000
  - service: http_status:404
```

## Start the stack

```bash
docker compose up -d
```

Check container status:

```bash
docker compose ps
```

Follow Cloudflare Tunnel logs:

```bash
docker compose logs -f cloudflared
```

Restart the tunnel connector:

```bash
docker compose restart cloudflared
```

## Verify the tunnel is healthy

Check Docker health status:

```bash
docker compose ps cloudflared
```

Inspect recent logs:

```bash
docker compose logs --tail=100 cloudflared
```

Test the public API:

```bash
curl -i https://api.eduardteodor.co.uk/health
```

Test the backend from inside the tunnel container:

```bash
docker compose exec backend wget -qO- http://127.0.0.1:8000/health
```

If you need deeper Docker DNS debugging, temporarily attach a debug container to
the private network rather than publishing backend ports.

## Logs

Cloudflare connector logs:

```bash
docker compose logs -f cloudflared
```

Backend logs:

```bash
docker compose logs -f backend
```

All services:

```bash
docker compose logs -f
```

## Backend service

The Compose file builds the Spring Boot API from `backend-java` and runs it on
port `8000` inside Docker so the Cloudflare Tunnel route can stay stable. Keep:

- Service name: `backend`
- Internal listening port: `8000`
- No `ports:` mapping
- Connection to the `backend_private` network

The tunnel route depends on this internal Docker DNS name:

```text
http://backend:8000
```

## Security best practices

- Keep `.env` out of git.
- Rotate the tunnel token regularly and immediately after suspected exposure.
- Do not publish backend ports with Docker `ports:`.
- Keep the backend on the private Docker network only.
- Keep `cloudflared` logs at `info` or `warn`; avoid `debug` in production because request details may appear in logs.
- Use least-privilege Cloudflare roles for users who can manage tunnels and DNS.
- Enable Cloudflare dashboard account security, including MFA.
- Keep Docker and the host OS patched.
- Pull fresh Cloudflare images during maintenance:

```bash
docker compose pull cloudflared
docker compose up -d
```

## Securing the API further

Use Cloudflare Access for private or admin API paths. Add an Access application
for `api.eduardteodor.co.uk` or specific path patterns and require identity
providers, service tokens, device posture, or email/domain rules.

Use Cloudflare rate limiting to protect login, MFA, write, and expensive API
routes. Start with low thresholds on authentication endpoints and higher
thresholds on read-only endpoints.

Use the Cloudflare WAF managed rules and custom rules to block suspicious
requests, unwanted countries or ASNs, invalid methods, and known exploit
patterns before traffic reaches the origin.

Use API Shield for stronger API controls. Add schema validation, mTLS where
appropriate, and endpoint discovery/protection for production APIs.

## Troubleshooting

### cloudflared exits immediately

Check that `.env` exists and contains a valid token:

```bash
docker compose logs --tail=100 cloudflared
```

If the token was rotated or exposed, get a new token from Cloudflare Zero Trust
and update `.env`.

### Tunnel is running but the hostname returns an error

Verify the public hostname route in Cloudflare Zero Trust:

```text
api.eduardteodor.co.uk -> http://backend:8000
```

Also confirm the backend is healthy:

```bash
docker compose ps backend
docker compose logs --tail=100 backend
```

### Backend works internally but not publicly

Confirm the hostname was added under the same tunnel that this connector uses.
Check Cloudflare DNS for `api.eduardteodor.co.uk`; it should be routed to the
tunnel by Cloudflare, not to your server IP.

### Health check is unhealthy

The `cloudflared` service sets `TUNNEL_METRICS=0.0.0.0:2000`, which exposes a
metrics listener on port `2000` inside the container. Docker health checks use
the `/ready` endpoint, which reports healthy only when the connector has an
active Cloudflare edge connection.

Check:

```bash
docker compose logs -f cloudflared
```

### Corporate firewall blocks outbound tunnel connections

Cloudflare Tunnel does not require inbound firewall or router ports. It does
need outbound connectivity from the host to Cloudflare. If your network blocks
the default tunnel protocol, review Cloudflare's tunnel connection error
documentation and allow the required outbound traffic.

## Official references

- Cloudflare Tunnel documentation: https://developers.cloudflare.com/tunnel/
- Cloudflare Tunnel setup guide: https://developers.cloudflare.com/tunnel/setup/
- Tunnel tokens documentation: https://developers.cloudflare.com/tunnel/advanced/tunnel-tokens/
- Tunnel routing documentation: https://developers.cloudflare.com/tunnel/routing/
- Tunnel monitoring documentation: https://developers.cloudflare.com/tunnel/monitoring/
- Cloudflared Docker image note: https://developers.cloudflare.com/tunnel/downloads/
