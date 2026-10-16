CREATE EXTENSION IF NOT EXISTS citext;

CREATE TABLE posts (
    id UUID PRIMARY KEY DEFAULT uuidv7(),

    type TEXT NOT NULL DEFAULT 'thought'
        CONSTRAINT posts_type_valid CHECK (type IN ('project', 'thought')),

    slug CITEXT UNIQUE NOT NULL,
    title TEXT NOT NULL,
    summary TEXT NOT NULL DEFAULT '',
    content TEXT NOT NULL,

    featured BOOLEAN NOT NULL DEFAULT FALSE,

    -- NULL = draft, set = published (can be future-dated for scheduling)
    published_at TIMESTAMPTZ,

    -- project-only fields (NULL for thoughts)
    repo_url TEXT,
    live_url TEXT,

    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE tags (
    id UUID PRIMARY KEY DEFAULT uuidv7(),
    name CITEXT UNIQUE NOT NULL,
    slug CITEXT UNIQUE NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE post_tags (
    post_id UUID NOT NULL REFERENCES posts(id) ON DELETE CASCADE,
    tag_id  UUID NOT NULL REFERENCES tags(id)  ON DELETE CASCADE,
    PRIMARY KEY (post_id, tag_id)
);

-- "Published posts of type X, newest first": the main listing query
CREATE INDEX idx_posts_type_published_at
    ON posts (type, published_at DESC)
    WHERE published_at IS NOT NULL;

-- "Featured items on the homepage"
CREATE INDEX idx_posts_featured
    ON posts (published_at DESC)
    WHERE featured AND published_at IS NOT NULL;

-- "Posts with tag X" (the PK already covers lookups by post_id)
CREATE INDEX idx_post_tags_tag_id
    ON post_tags (tag_id);

    