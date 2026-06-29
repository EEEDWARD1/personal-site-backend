CREATE TABLE IF NOT EXISTS projects (
                                        id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    slug VARCHAR(255) NOT NULL UNIQUE,

    title VARCHAR(255) NOT NULL,
    summary VARCHAR(500),
    description TEXT NOT NULL,

    tech_stack TEXT,

    github_url VARCHAR(500),
    live_url VARCHAR(500),

    status VARCHAR(50) NOT NULL DEFAULT 'COMPLETED',

    featured BOOLEAN NOT NULL DEFAULT FALSE,
    published BOOLEAN NOT NULL DEFAULT FALSE,

    display_order INTEGER NOT NULL DEFAULT 0,

    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP NOT NULL DEFAULT NOW()
    );

CREATE INDEX idx_projects_slug
    ON projects(slug);

CREATE INDEX idx_projects_published
    ON projects(published);

CREATE INDEX idx_projects_featured
    ON projects(featured);

CREATE INDEX idx_projects_status
    ON projects(status);

CREATE INDEX idx_projects_display_order
    ON projects(display_order);