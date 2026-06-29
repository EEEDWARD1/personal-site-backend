# Personal Site

Standalone Next.js frontend for `eduardteodor.co.uk`.

The site is deployed independently on Netlify. It does not rely on files or
environment variables from the repository root at deploy time.

## API

The frontend talks to the public API at:

```text
https://wbapi.eduardteodor.co.uk
```

Set this in Netlify as:

```env
NEXT_PUBLIC_API_BASE_URL=https://wbapi.eduardteodor.co.uk
```

If the variable is not set, the app defaults to the same production API host.
For local backend development, override it in `.env.local`:

```env
NEXT_PUBLIC_API_BASE_URL=http://localhost:8080
```

## Local Development

Install dependencies and run the frontend:

```bash
npm install
npm run dev
```

Open `http://localhost:3000`.

## Netlify

Use these build settings:

```text
Base directory: personal-site
Build command: npm run build
Publish directory: .next
```

If the Netlify project is connected to the repository root, set the base
directory to `personal-site`. If the Netlify project is connected directly to
the `personal-site` folder, leave the base directory empty.

Required environment variable:

```env
NEXT_PUBLIC_API_BASE_URL=https://wbapi.eduardteodor.co.uk
```

## Checks

```bash
npm run lint
npm run build
```
