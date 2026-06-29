CREATE TABLE IF NOT EXISTS freelance_projects (
                                                  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    slug VARCHAR(255) NOT NULL UNIQUE,

    client_name VARCHAR(255),
    project_title VARCHAR(255) NOT NULL,

    summary VARCHAR(500),
    description TEXT NOT NULL,

    services TEXT[] NOT NULL DEFAULT '{}',

    testimonial TEXT,

    website_url VARCHAR(500),
    thumbnail_url VARCHAR(500),

    featured BOOLEAN NOT NULL DEFAULT FALSE,
    published BOOLEAN NOT NULL DEFAULT FALSE,

    completed_at TIMESTAMP,

    display_order INTEGER NOT NULL DEFAULT 0,

    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP NOT NULL DEFAULT NOW()
    );

CREATE INDEX idx_freelance_slug
    ON freelance_projects(slug);

CREATE INDEX idx_freelance_published
    ON freelance_projects(published);

CREATE INDEX idx_freelance_featured
    ON freelance_projects(featured);

CREATE INDEX idx_freelance_completed
    ON freelance_projects(completed_at DESC);

CREATE INDEX idx_freelance_display_order
    ON freelance_projects(display_order);