# Personal Site API

A .NET 10 Minimal API for personal-site posts and tags, backed by PostgreSQL.

## Run locally

1. Copy `.env.example` to `.env` and set the values.
2. Start the API and database:

   ```bash
   docker compose up --build
   ```

The API is available at `http://localhost:8080`. In Development, Scalar API docs are available at `http://localhost:8080/scalar`.

## CMS on etvm

GitHub Actions builds and runs the Next.js CMS in `services/cms` **on etvm** as
part of the production Docker Compose stack, alongside the API and PostgreSQL.
The CMS uses `http://api:8080/v1` on Docker's private network, not the public API
hostname. The existing Cloudflare Tunnel also serves **https://cms.eduardteodor.co.uk**;
no CMS ports are published on etvm. No workstation server or SSH tunnel is needed. The app
provides paginated titles and a JSON editor with create/save/reload/delete, and
requires your allowlisted Clerk user. Next.js forwards your session token
server-side, and the API independently verifies it. See
[`services/cms/README.md`](services/cms/README.md) for deployment and access details.

## Endpoints

- `GET /v1/posts` — paginated post summaries; supports `type`, `tag`, `isFeatured`, and `enableTags` filters/options.
- `GET /v1/posts/titles` — paginated post titles.
- `GET /v1/posts/{slug}` — a published post with tags.
- `GET /v1/tags` — paginated tags.
- `GET /health` — checks database credentials, table access, and mapped post/tag columns; returns `200` when healthy or `503` otherwise, without exposing exception details.

All versioned API routes use **`/v1` only**, including at
`https://api.eduardteodor.co.uk/v1`. The former `/api/v1` prefix is intentionally
not supported. Update existing API consumers when deploying this change, and
keep the tunnel pointed directly at `http://api:8080` without adding a path prefix.

Repeated `tag` parameters match posts with **any** requested tag, for example:

```text
/v1/posts?tag=dotnet&tag=postgres&enableTags=true
```

### Admin posts

Admin routes require a valid Clerk session JWT from the configured issuer, with
`sub` exactly `user_3KvGpJwyF1axbnsKPdCYemBhkh6` in production. Every other user receives `403`;
missing, invalid, expired, or wrong-origin tokens receive `401`. The API verifies
RS256 signatures using Clerk's public JWKS, issuer, expiration, and the `azp`
authorized-party claim. Default Clerk session tokens have no audience claim, so
origin validation is used rather than requiring `aud`.

`Clerk:Issuer`, `Clerk:AdminUserId`, and `Clerk:AuthorizedParties` are configured in
`services/api/src/EduardTeodor.Api/appsettings.Production.json` in production.
The issuer is `https://clerk.eduardteodor.co.uk` and the authorized origin is
`https://cms.eduardteodor.co.uk`. Compose supports `CLERK_ISSUER` to override the issuer.
For a different CMS origin, update both `CMS_ORIGIN` in the CMS and
`Clerk__AuthorizedParties__0` in the API environment. Public API routes remain
anonymous. Legacy `JWT_KEY`/role-based admin tokens are no longer accepted.

- `POST /v1/admin/posts` — create a post; returns `201` with `{ id, title }`, `400` for invalid input, or `409` for a duplicate slug.
- `GET /v1/admin/posts?page=1&pageSize=20` — all posts, including drafts and scheduled posts, newest-created first. Returns paginated `{ id, title }` items only. Page size is clamped to 1–100.
- `DELETE /v1/admin/posts/{id}` — delete by GUID; returns `204`, or `404` when missing.
- `GET /v1/admin/posts/{id}` — load a post, including drafts, with editable fields plus read-only `id`, `createdAt`, and `updatedAt`. Returns an `ETag` header or `404`.
- `PUT /v1/admin/posts/{id}` — replace all editable fields. Requires `If-Match` with the ETag from GET; returns `204` on success, `400` for invalid input, `404` when missing, `409` for a duplicate slug, `412` if another save changed the post, or `428` if `If-Match` is missing.

Create request example (omit `publishedAt` or use `null` for a draft):

```json
{
  "slug": "my-post",
  "title": "My post",
  "content": "Post content",
  "summary": "A short summary",
  "type": "thought",
  "featured": false,
  "publishedAt": null
}
```

Slugs remain public URLs, but admin routes identify posts by GUID.
The slug `titles` is reserved and rejected by both create and update.

### JSON editor workflow

1. Load `GET /v1/admin/posts/{id}` and retain its `ETag` header (including quotes).
2. Edit the JSON and send it to `PUT /v1/admin/posts/{id}` with `If-Match: "123"` (using the actual ETag).
3. On `204`, reload GET for the saved JSON and fresh ETag before another save. On `412`, reload and reconcile changes rather than retrying with stale content.

PUT requires all seven editable fields shown in the example above, including an explicit `publishedAt` (use `null` to unpublish, or a future timestamp to schedule). Read-only metadata in the submitted JSON is ignored. Existing tag associations are preserved, not edited by this route.

Concurrency uses PostgreSQL's built-in `xmin` row version, so no schema migration is required.

## Tests

```bash
dotnet test services/api/EduardTeodor.slnx
```

## Deployment

Pushing to `main` builds and deploys **both the API and CMS** in the production
Compose stack on etvm. The workflow requires a Linux x64 self-hosted runner on
etvm with Docker, Compose v2 (`up --wait` support), and permission to run Docker.

Add these repository or `production` environment secrets in GitHub:

- `POSTGRES_USER`
- `POSTGRES_PASSWORD`
- `POSTGRES_DB`
- `CLOUDFLARE_TUNNEL_TOKEN`
- `NEXT_PUBLIC_CLERK_PUBLISHABLE_KEY` — CMS public build/runtime key.
- `CLERK_SECRET_KEY` — CMS runtime only; never baked into the image.

Optionally set the GitHub Actions **variable** `CLERK_ISSUER` to override the
configured Clerk issuer. The API itself uses public signing keys and does not
need either Clerk application key; the CMS container needs both matching keys.
Use matching **production** Clerk keys (`pk_live_...` and `sk_live_...`). Configure
an additional public hostname on the existing tunnel to forward to `http://cms:3000`. Finish the
production Clerk DNS records for `clerk.eduardteodor.co.uk`; that hostname did not
resolve when checked during configuration. Exact setup steps are in
[`services/cms/README.md`](services/cms/README.md).

Register the runner for this repository and give it the `self-hosted`, `linux`,
`x64`, and **`etvm`** labels. The workflow uses `compose.yml` and `compose.prod.yml`,
waits for health checks, and checks connectivity from the CMS to the private API.
Release API tests and CMS tests/lint run in Docker test targets before deployment;
the runner does not need host-installed Node.js, pnpm, or .NET. Production Clerk
discovery and signing keys are checked before replacing running services. After
deployment, checks exercise database-backed health and public `/v1` queries.
It does not remove the database volume. Cloudflare tunnel/hostname provisioning
and Clerk DNS setup must be completed in their dashboards before deployment.
