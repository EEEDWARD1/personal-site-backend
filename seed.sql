-- Development seed data. NOT a migration: keep this outside api/Migrations.
-- Safe to re-run: inserts skip rows that already exist (matched by slug).
--
-- To start from a clean slate first, uncomment:
-- TRUNCATE post_tags, posts, tags CASCADE;

BEGIN;

-- ---------------------------------------------------------------------------
-- Tags
-- ---------------------------------------------------------------------------
INSERT INTO tags (name, slug) VALUES
    ('TypeScript',     'typescript'),
    ('React',          'react'),
    ('Next.js',        'nextjs'),
    ('C#',             'csharp'),
    ('PostgreSQL',     'postgresql'),
    ('Docker',         'docker'),
    ('Spring Boot',    'spring-boot'),
    ('Homelab',        'homelab'),
    ('Career',         'career'),
    ('Draft Only Tag', 'draft-only-tag')   -- only used by UNPUBLISHED posts
ON CONFLICT (slug) DO NOTHING;

-- ---------------------------------------------------------------------------
-- Posts: projects
-- ---------------------------------------------------------------------------
INSERT INTO posts (kind, slug, title, summary, content, featured, published, published_at) VALUES
    ('project', 'personal-site',
     'Personal Site',
     'My personal website built with Next.js and a C# API.',
     E'# Personal Site\n\nThe site you are looking at, backed by a REST API.',
     TRUE,  TRUE,  '2026-10-04 12:00:00+00'),

    ('project', 'dotnet-portfolio-api',
     'Portfolio REST API',
     'Minimal API in .NET 10 with EF Core, DbUp and Keycloak.',
     E'# Portfolio REST API\n\nA learning project that grew into the real backend.',
     TRUE,  TRUE,  '2026-10-01 09:30:00+00'),

    ('project', 'proxmox-homelab',
     'Proxmox Homelab',
     'Self-hosted infrastructure running on Proxmox.',
     E'# Proxmox Homelab\n\nVMs, containers and backups at home.',
     TRUE,  TRUE,  '2026-09-15 18:00:00+00'),

    ('project', 'envisalink-dashboard',
     'Alarm Panel Dashboard',
     'A home dashboard built on a reverse-engineered alarm portal.',
     E'# Alarm Panel Dashboard\n\nPolling the cloud portal and presenting zone status.',
     FALSE, TRUE,  '2026-09-01 10:00:00+00'),

    ('project', 'spring-boot-backend',
     'Spring Boot Backend',
     'The original Java backend for this site.',
     E'# Spring Boot Backend\n\nThe first version, before the move to .NET.',
     FALSE, TRUE,  '2026-08-10 14:15:00+00'),

    ('project', 'anthracite-media-site',
     'Anthracite Media Website',
     'Marketing site for a web and conversion agency.',
     E'# Anthracite Media Website\n\nBuilt for speed and conversion.',
     FALSE, TRUE,  '2026-07-20 11:00:00+00'),

    -- Published project with NO tags and NO summary (edge case)
    ('project', 'cgu-construction-site',
     'CGU Construction Website',
     NULL,
     E'# CGU Construction Website\n\nFreelance work for a construction company.',
     FALSE, TRUE,  '2026-06-05 08:45:00+00'),

    -- UNPUBLISHED but featured: must never be public (BR005)
    ('project', 'secret-project-draft',
     'Secret Project (Draft)',
     'Should never appear in any public response.',
     E'# Secret\n\nNot ready.',
     TRUE,  FALSE, NULL),

    -- UNPUBLISHED, no tags
    ('project', 'old-idea-wip',
     'Old Idea (WIP)',
     NULL,
     E'# Old Idea\n\nParked.',
     FALSE, FALSE, NULL)
ON CONFLICT (slug) DO NOTHING;

-- ---------------------------------------------------------------------------
-- Posts: thoughts
-- ---------------------------------------------------------------------------
INSERT INTO posts (kind, slug, title, summary, content, featured, published, published_at) VALUES
    ('thought', 'why-i-moved-to-dotnet',
     'Why I Moved My Backend to .NET',
     'Notes on switching from Spring Boot to C#.',
     E'# Why I Moved to .NET\n\nThe short version: curiosity and a new job stack.',
     TRUE,  TRUE,  '2026-10-05 19:00:00+00'),

    ('thought', 'minimal-apis-vs-controllers',
     'Minimal APIs vs Controllers',
     'What each style is actually doing for you.',
     E'# Minimal APIs vs Controllers\n\nBoth are an HTTP adapter over your services.',
     TRUE,  TRUE,  '2026-10-03 16:30:00+00'),

    ('thought', 'learning-in-public',
     'Learning in Public',
     'Why I build things layer by layer and write it down.',
     E'# Learning in Public\n\nTyping the code is how it sticks.',
     FALSE, TRUE,  '2026-09-20 20:00:00+00'),

    ('thought', 'postgres-enum-gotchas',
     'Postgres Enum Gotchas',
     'Mapping a Postgres enum to a C# enum with Npgsql.',
     E'# Postgres Enum Gotchas\n\nThe type name has to match exactly.',
     FALSE, TRUE,  '2026-09-05 13:00:00+00'),

    -- Published thought with NO summary (edge case)
    ('thought', 'notes-on-homelab-backups',
     'Notes on Homelab Backups',
     NULL,
     E'# Homelab Backups\n\n3-2-1 and why I mostly follow it.',
     FALSE, TRUE,  '2026-08-01 09:00:00+00'),

    -- UNPUBLISHED thought using a draft-only tag
    ('thought', 'half-written-rant',
     'Half-Written Rant',
     'Unfinished.',
     E'# Rant\n\nTo be continued.',
     FALSE, FALSE, NULL)
ON CONFLICT (slug) DO NOTHING;

-- ---------------------------------------------------------------------------
-- Post <-> tag links (by slug, so no UUIDs to copy around)
-- ---------------------------------------------------------------------------
INSERT INTO post_tags (post_id, tag_id)
SELECT p.id, t.id
FROM (VALUES
    ('personal-site',              'typescript'),
    ('personal-site',              'react'),
    ('personal-site',              'nextjs'),
    ('dotnet-portfolio-api',       'csharp'),
    ('dotnet-portfolio-api',       'postgresql'),
    ('dotnet-portfolio-api',       'docker'),
    ('proxmox-homelab',            'homelab'),
    ('proxmox-homelab',            'docker'),
    ('envisalink-dashboard',       'typescript'),
    ('envisalink-dashboard',       'docker'),
    ('spring-boot-backend',        'spring-boot'),
    ('spring-boot-backend',        'postgresql'),
    ('anthracite-media-site',      'nextjs'),
    ('anthracite-media-site',      'typescript'),
    ('anthracite-media-site',      'react'),
    -- cgu-construction-site: deliberately no tags
    ('secret-project-draft',       'csharp'),
    ('secret-project-draft',       'draft-only-tag'),
    ('why-i-moved-to-dotnet',      'csharp'),
    ('why-i-moved-to-dotnet',      'career'),
    ('minimal-apis-vs-controllers','csharp'),
    ('learning-in-public',         'career'),
    ('postgres-enum-gotchas',      'postgresql'),
    ('postgres-enum-gotchas',      'csharp'),
    ('notes-on-homelab-backups',   'homelab'),
    ('half-written-rant',          'draft-only-tag')
) AS v(post_slug, tag_slug)
JOIN posts p ON p.slug = v.post_slug
JOIN tags  t ON t.slug = v.tag_slug
ON CONFLICT DO NOTHING;

COMMIT;

-- Quick check:
-- SELECT kind, published, featured, count(*) FROM posts GROUP BY 1, 2, 3 ORDER BY 1, 2 DESC, 3 DESC;
