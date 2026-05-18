CREATE TABLE IF NOT EXISTS blog_posts (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid () ,
    user_id UUID NOT NULL REFERENCES users ( id ) ,
    slug VARCHAR (255) NOT NULL UNIQUE ,
    title VARCHAR (255) NOT NULL ,
    content TEXT NOT NULL ,
    excerpt VARCHAR (500) ,
    starred BOOLEAN DEFAULT false ,
    published BOOLEAN DEFAULT false ,
    published_at TIMESTAMP ,
    created_at TIMESTAMP DEFAULT NOW () ,
    updated_at TIMESTAMP DEFAULT NOW ()
    );

CREATE INDEX idx_blog_posts_slug ON blog_posts ( slug );
CREATE INDEX idx_blog_posts_published ON blog_posts ( published );
CREATE INDEX idx_blog_posts_starred ON blog_posts ( starred );