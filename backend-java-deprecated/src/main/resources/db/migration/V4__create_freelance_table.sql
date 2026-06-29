CREATE TABLE IF NOT EXISTS freelance_work (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid () ,
    user_id UUID NOT NULL REFERENCES users ( id ) ,
    client_name VARCHAR (255) ,
    project_title VARCHAR (255) NOT NULL ,
    description TEXT NOT NULL ,
    services TEXT ,
    testimonial TEXT ,
    website_url VARCHAR (500) ,
    featured BOOLEAN DEFAULT false ,
    published BOOLEAN DEFAULT false ,
    completed_at TIMESTAMP ,
    created_at TIMESTAMP DEFAULT NOW () ,
    updated_at TIMESTAMP DEFAULT NOW ()
);

CREATE INDEX idx_freelance_published ON freelance_work ( published );
CREATE INDEX idx_freelance_featured ON freelance_work ( featured );