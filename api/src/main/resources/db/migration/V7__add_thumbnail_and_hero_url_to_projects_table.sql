ALTER TABLE projects
    ADD COLUMN thumbnail_url VARCHAR(500),
    ADD COLUMN hero_url VARCHAR(500),
    ALTER COLUMN hero_url SET DEFAULT 'enter_cdn_url',
    ALTER COLUMN hero_url SET DEFAULT 'enter_cdn_url'