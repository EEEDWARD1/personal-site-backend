CREATE TABLE IF NOT EXISTS projects (
                                        id UUID PRIMARY KEY DEFAULT gen_random_uuid () ,
    user_id UUID NOT NULL REFERENCES users ( id ) ,
    title VARCHAR (255) NOT NULL ,
    description TEXT NOT NULL ,
    summary VARCHAR (500) ,
    tech_stack TEXT ,
    github_url VARCHAR (500) ,
    live_url VARCHAR (500) ,
    status VARCHAR (50) ,
    featured BOOLEAN DEFAULT false ,
    published BOOLEAN DEFAULT false ,
    created_at TIMESTAMP DEFAULT NOW () ,
    updated_at TIMESTAMP DEFAULT NOW ()
    );

CREATE INDEX idx_projects_published ON projects ( published );
CREATE INDEX idx_projects_featured ON projects ( featured );