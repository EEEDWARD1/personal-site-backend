package uk.co.eduardteodor.backend_java.blog;

import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

@Service
public class BlogPostService {
    private final BlogPostRepository blogPostRepository;

    public BlogPostService(BlogPostRepository blogPostRepository) {
        this.blogPostRepository = blogPostRepository;
    }

    public List<BlogPost> getAllPublishedPosts(){
        return blogPostRepository.findByPublishedTrue();
    }

    public Optional<BlogPost> getPostBySlug(String slug) {
        return blogPostRepository.findBySlugAndPublishedTrue(slug);
    }

    public List<BlogPost> getHomepagePosts() {
        return blogPostRepository.findHomepagePosts();
    }
}
