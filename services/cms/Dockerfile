# syntax=docker/dockerfile:1
FROM node:22-bookworm-slim AS build
WORKDIR /app
ENV NEXT_TELEMETRY_DISABLED=1
RUN npm install --global pnpm@12.11.2

COPY package.json pnpm-lock.yaml pnpm-workspace.yaml ./
RUN pnpm install --frozen-lockfile
COPY . .

# CI explicitly builds this target; no Clerk credentials are required for tests.
FROM build AS test
RUN pnpm lint && pnpm test

FROM build AS publish

# Publishable keys are public and must be present when Next.js builds the client.
# CLERK_SECRET_KEY is supplied only at runtime, never as a build argument.
ARG NEXT_PUBLIC_CLERK_PUBLISHABLE_KEY
ENV NEXT_PUBLIC_CLERK_PUBLISHABLE_KEY=$NEXT_PUBLIC_CLERK_PUBLISHABLE_KEY
ENV NEXT_PUBLIC_CLERK_SIGN_IN_URL=/sign-in
RUN test -n "$NEXT_PUBLIC_CLERK_PUBLISHABLE_KEY" && pnpm build

FROM node:22-bookworm-slim AS runtime
WORKDIR /app
ENV NODE_ENV=production \
    NEXT_TELEMETRY_DISABLED=1 \
    HOSTNAME=0.0.0.0 \
    PORT=3000
COPY --from=publish --chown=node:node /app/.next/standalone ./
COPY --from=publish --chown=node:node /app/.next/static ./.next/static
COPY --from=publish --chown=node:node /app/public ./public
USER node
EXPOSE 3000
CMD ["node", "server.js"]
