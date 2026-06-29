# API Integration Guide

This document describes the current backend API so a frontend or coding agent can integrate against it without reading the backend source first.

## Base URL

- Local development: `http://localhost:8080`
- Website production: `https://wbapi.eduardteodor.co.uk`

## Auth Model

This backend uses a 2-step login flow with TOTP.

### Auth sequence

1. Frontend submits username and password to `POST /api/auth/login`
2. Backend validates credentials
3. If the user has not enabled TOTP yet:
   - backend returns `TOTP_SETUP_REQUIRED`
   - response includes:
     - `preAuthToken`
     - `secret`
     - `qrCode` as base64 PNG
4. Frontend shows TOTP setup UI:
   - display QR code or manual secret
   - user adds it to an authenticator app
5. Frontend submits `preAuthToken` and 6-digit code to `POST /api/auth/totp/verify`
6. Backend enables TOTP on first successful verification
7. Backend returns the full JWT
8. On future logins:
   - `POST /api/auth/login` returns `TOTP_REQUIRED`
   - frontend asks for the TOTP code
   - frontend calls `POST /api/auth/totp/verify`
   - backend returns the full JWT

### Important token rules

- `preAuthToken` is temporary and only for TOTP completion
- `preAuthToken` must not be used for admin routes
- only the final JWT returned from `/api/auth/totp/verify` can access authenticated routes

### JWT usage

For authenticated requests, send:

```http
Authorization: Bearer <jwt>
```

## Auth endpoints

### `POST /api/auth/login`

Request:

```json
{
  "username": "testuser",
  "password": "Password123!"
}
```

Possible responses:

#### TOTP setup required

```json
{
  "status": "TOTP_SETUP_REQUIRED",
  "token": null,
  "preAuthToken": "string",
  "secret": "string",
  "qrCode": "base64-png"
}
```

#### TOTP required

```json
{
  "status": "TOTP_REQUIRED",
  "token": null,
  "preAuthToken": "string",
  "secret": null,
  "qrCode": null
}
```

### `POST /api/auth/totp/verify`

Request:

```json
{
  "preAuthToken": "string",
  "code": "123456"
}
```

Success response:

```json
{
  "status": "AUTHENTICATED",
  "token": "jwt",
  "preAuthToken": null,
  "secret": null,
  "qrCode": null
}
```

## Public vs admin route split

Public read routes require no authentication.

Admin routes require the final JWT.

### Public routes

- `GET /api/blog/posts`
- `GET /api/blog/posts?starredOnly=true`
- `GET /api/blog/posts/{slug}`
- `GET /api/projects`
- `GET /api/projects?featuredOnly=true`
- `GET /api/projects/{slug}`
- `GET /api/freelance`
- `GET /api/freelance?featuredOnly=true`
- `GET /api/freelance/{slug}`

### Admin routes

- `GET /api/admin/blog-posts`
- `GET /api/admin/blog-posts/{id}`
- `POST /api/admin/blog-posts`
- `PUT /api/admin/blog-posts/{id}`
- `DELETE /api/admin/blog-posts/{id}`
- `GET /api/admin/projects`
- `GET /api/admin/projects/{id}`
- `POST /api/admin/projects`
- `PUT /api/admin/projects/{id}`
- `DELETE /api/admin/projects/{id}`
- `GET /api/admin/freelance-projects`
- `GET /api/admin/freelance-projects/{id}`
- `POST /api/admin/freelance-projects`
- `PUT /api/admin/freelance-projects/{id}`
- `DELETE /api/admin/freelance-projects/{id}`

## Blog API

### Public list

`GET /api/blog/posts`

Optional query:

- `starredOnly=true`

Returns published posts only.

### Public item

`GET /api/blog/posts/{slug}`

Returns a published post by slug.

### Admin create/update payload

```json
{
  "slug": "my-first-post",
  "title": "My First Post",
  "content": "Full blog post body",
  "excerpt": "Short summary",
  "starred": true,
  "published": true,
  "publishedAt": "2026-06-29T16:30:00"
}
```

Notes:

- `slug` must be unique
- duplicate slug returns `409`
- public routes only show posts where `published=true`
- if `published=true` and `publishedAt` is missing, backend sets it automatically

## Projects API

### Public list

`GET /api/projects`

Optional query:

- `featuredOnly=true`

Returns published projects only.

### Public item

`GET /api/projects/{slug}`

Returns a published project by slug.

### Admin create/update payload

```json
{
  "slug": "personal-site",
  "title": "Personal Site",
  "summary": "Portfolio and admin platform",
  "description": "Long-form project description",
  "techStack": "Next.js, Spring Boot, PostgreSQL, Docker",
  "githubUrl": "https://github.com/example/personal-site",
  "liveUrl": "https://example.com",
  "status": "COMPLETED",
  "featured": true,
  "published": true,
  "displayOrder": 1
}
```

Notes:

- `slug` must be unique
- duplicate slug returns `409`
- public routes only show projects where `published=true`
- public list is sorted by:
  1. `displayOrder` ascending
  2. `featured` descending
  3. `createdAt` descending

## Freelance API

### Public list

`GET /api/freelance`

Optional query:

- `featuredOnly=true`

Returns published freelance projects only.

### Public item

`GET /api/freelance/{slug}`

Returns a published freelance project by slug.

### Admin create/update payload

```json
{
  "slug": "client-redesign",
  "clientName": "Acme Ltd",
  "projectTitle": "Marketing Site Redesign",
  "summary": "Modernized client website",
  "description": "Full freelance engagement details",
  "services": ["Design", "Frontend", "Backend"],
  "testimonial": "Great work.",
  "websiteUrl": "https://client-site.com",
  "thumbnailUrl": "https://cdn.example.com/thumb.jpg",
  "featured": true,
  "published": true,
  "completedAt": "2026-06-29T17:00:00",
  "displayOrder": 1
}
```

Notes:

- `slug` must be unique
- duplicate slug returns `409`
- `services` is an array of strings
- public routes only show freelance projects where `published=true`
- public list is sorted by:
  1. `displayOrder` ascending
  2. `featured` descending
  3. `completedAt` descending
  4. `createdAt` descending

## Integration rules for frontend agents

### Login UX

- do not expect a one-request login flow
- always treat login as a possible 2-step flow
- after `POST /api/auth/login`:
  - if `status = TOTP_SETUP_REQUIRED`, show QR/secret setup and then prompt for code
  - if `status = TOTP_REQUIRED`, prompt for code
  - if `status = AUTHENTICATED`, store JWT immediately

### JWT storage

- store the final JWT somewhere the frontend can attach it to admin requests
- do not store or reuse `preAuthToken` beyond the TOTP verification step

### Admin UI expectations

- public pages can use public routes only
- admin dashboard/editor pages must attach the JWT to every `/api/admin/**` request
- use slug-based public URLs
- use id-based admin edit/delete requests

### Error handling

Expect these common statuses:

- `200` success
- `201` created
- `204` deleted
- `401` invalid credentials, invalid TOTP, invalid or expired auth token
- `403` authenticated but not allowed, or missing usable JWT on protected route
- `404` item not found
- `409` duplicate slug

## Current backend assumptions

- one authenticated user can access admin routes
- there is no separate role/permission model implemented yet
- there is no refresh token flow implemented
- there is no public health/me endpoint documented here
