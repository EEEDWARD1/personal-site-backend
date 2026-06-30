ALTER TABLE freelance_projects
    ADD COLUMN hero_url VARCHAR(500),
    ALTER COLUMN hero_url SET DEFAULT 'enter cdn url'