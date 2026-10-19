# Personal Site CMS

Single-user Next.js CMS deployed on **etvm** at **https://cms.eduardteodor.co.uk**.
Production owner: `user_3KvGpJwyF1axbnsKPdCYemBhkh6`.

## Architecture

```text
Browser → Cloudflare HTTPS → existing tunnel → cms:3000
                                                    ↓
                                              api:8080/v1 → PostgreSQL
```

GitHub Actions builds and runs both the CMS and API on etvm. The CMS has **no
published host ports**. The existing `cloudflared` connector joins `cms_edge`
to reach the CMS, retaining its existing API network and token. CMS-to-API
traffic uses `cms_backend`.

## Required one-time setup

### 1. Finish Clerk production DNS

The existing application is `etcms` (`app_3KvBcHRohc9d0IKcDEGTR31L2P5`). Its
production instance is `ins_3KvGjPsSly2Xh1DMKx1i4sCk3O6`, with issuer
**https://clerk.eduardteodor.co.uk**.

Add the **exact DNS records provided by Clerk's production Domains page** in
Cloudflare DNS. Do not guess their CNAME targets or point the Clerk domain at the
CMS tunnel. Follow Clerk's proxy/DNS instructions and wait for verification and
TLS provisioning. At configuration time, `clerk.eduardteodor.co.uk` did not resolve.

This URL must return valid OpenID discovery JSON before authentication works:

```text
https://clerk.eduardteodor.co.uk/.well-known/openid-configuration
```

Use the production application URL `https://cms.eduardteodor.co.uk` and sign-in
URL `https://cms.eduardteodor.co.uk/sign-in` in Clerk where applicable. Keep sign-ups
disabled and configure any social provider production credentials Clerk requires.
The email alone does not grant admin access: both apps check the verified user ID.

### 2. Add the CMS hostname to the existing Cloudflare tunnel

In Cloudflare Zero Trust / Networks / Tunnels:

1. Select the **existing remotely managed tunnel** that serves the API.
2. Add a published application route/public hostname:
   - Hostname: **`cms.eduardteodor.co.uk`**
   - Service type: **HTTP**
   - Service URL: **`cms:3000`** (equivalent to `http://cms:3000`)
3. Ensure its DNS record is created for the tunnel, not etvm's public IP.
4. Keep the existing GitHub secret **`CLOUDFLARE_TUNNEL_TOKEN`**; no second token is needed.

The single connector can reach both `api:8080` and `cms:3000` through its Docker networks.
Do not use `localhost:3000` as the origin: that would refer to the tunnel container,
not the CMS container. Preserve the public Host header and HTTPS forwarded scheme.
Enforce HTTPS for the hostname using Cloudflare's HTTPS redirect settings. Do not
enable caching of authenticated CMS/API responses in Cloudflare.

The Docker/Actions changes run the connector; they do **not** create your
Cloudflare account's tunnel, DNS route, or Clerk DNS records automatically.

### 3. Configure GitHub Actions

The existing etvm runner needs `self-hosted`, `linux`, `x64`, and **`etvm`** labels,
Docker, and Compose v2 with `up --wait` support.

Set these repository or `production` environment secrets:

| Secret | Purpose |
| --- | --- |
| `POSTGRES_USER` / `POSTGRES_PASSWORD` / `POSTGRES_DB` | Existing database |
| `CLOUDFLARE_TUNNEL_TOKEN` | Existing tunnel serving API and CMS |
| **`NEXT_PUBLIC_CLERK_PUBLISHABLE_KEY`** | Production `pk_live_...` key |
| **`CLERK_SECRET_KEY`** | Matching production `sk_live_...` key |

The optional Actions variable `CLERK_ISSUER` defaults to
`https://clerk.eduardteodor.co.uk`. Do not retain the old development issuer.
The workflow rejects development key prefixes and development issuers.

The public key is a build argument because Next.js includes it in browser code.
The secret key is supplied **only at runtime**, never baked into an image or
passed to .NET. Workstation `.env.local` files are excluded from Docker.

### 4. Deploy and verify

Commit/push to `main`, or manually dispatch the **Build and deploy** workflow.
It runs API Release tests and CMS tests/lint in Docker, builds both images, and
checks Clerk discovery/signing keys before replacing running services. After
deployment it checks CMS health, database-backed API health, public `/v1` queries,
anonymous admin rejection, and production Clerk discovery. Only `/v1` is supported;
update any consumers using the former `/api/v1` prefix before deployment.
The existing PostgreSQL volume is preserved.

Open **https://cms.eduardteodor.co.uk** and sign in using the production owner
account. No workstation server or SSH forwarding is needed. Authenticated users
other than the owner cannot access content. The browser never calls .NET directly.

If the tunnel is healthy but the page is unavailable, check the Cloudflare public
hostname route and DNS. If sign-in fails, check Clerk's production DNS, keys, and
social provider setup. Deployment health checks do not simulate an owner login.

## Authentication and editing

.NET validates Clerk RS256 signatures, issuer, expiration, session status, the
exact `sub` user ID, and `azp: https://cms.eduardteodor.co.uk`. Production does not
allow the old development user or localhost origin. Public post/tag routes remain
anonymous. Separate development API defaults remain in `appsettings.json`;
production overrides are in `appsettings.Production.json`.

The dashboard lists paginated titles (including drafts), edits seven fields as
JSON, and supports create/save/reload/delete. Saves preserve `ETag`/`If-Match`
concurrency protection. Stale-save errors retain the draft. IDs and timestamps
are read-only, and existing tags are preserved. Sign-up UI is removed.

## Local checks

From `services/cms`, using Node.js 22.18+ and pnpm:

```bash
pnpm lint
pnpm test
pnpm build
```

Local development needs a separately configured development Clerk instance and
matching owner ID/origins; do not overwrite working keys or try to use production
Clerk on an unconfigured localhost domain. Production is built/deployed by Docker
and Actions with the configuration above.
