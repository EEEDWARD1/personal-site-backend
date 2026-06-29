package uk.co.eduardteodor.backend_java.blog;

import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.stereotype.Service;
import uk.co.eduardteodor.backend_java.user.User;
import uk.co.eduardteodor.backend_java.user.UserRepository;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

@Service
public class BlogPostService {
    private final BlogPostRepository blogPostRepository;
    private final UserRepository userRepository;

    public BlogPostService(BlogPostRepository blogPostRepository, UserRepository userRepository) {
        this.blogPostRepository = blogPostRepository;
        this.userRepository = userRepository;
    }

    // Public
    public List<BlogPost> getAllPublishedPosts(){
        return blogPostRepository.findByPublishedTrue();
    }

    public Optional<BlogPost> getPostBySlug(String slug) {
        return blogPostRepository.findBySlugAndPublishedTrue(slug);
    }

    public List<BlogPost> getHomepagePosts() {
        return blogPostRepository.findHomepagePosts();
    }

    // Admin
    public List<BlogPost> getAllPosts() {
        return blogPostRepository.findAll();
    }

    public BlogPost createPost(BlogPost blogPost) {
        // Get the authenticated user's username from security context
        String username = SecurityContextHolder.getContext()
                .getAuthentication().getName();

        // Look up the user to get their UUID
        User user = userRepository.findByUsername(username)
                .orElseThrow(() -> new RuntimeException("User not found"));

        blogPost.setUserId(user.getId());
        return blogPostRepository.save(blogPost);
    }

    public BlogPost updatePost(UUID id, BlogPost updated) {
        BlogPost existing = blogPostRepository.findById(id)
                .orElseThrow(() -> new RuntimeException("Post with id " + id + " not found"));
        existing.setTitle(updated.getTitle());
        existing.setSlug(updated.getSlug());
        existing.setContent(updated.getContent());
        existing.setExcerpt(updated.getExcerpt());
        existing.setPublished(updated.isPublished());
        existing.setStarred(updated.isStarred());
        existing.setPublishedAt(updated.getPublishedAt());
        return blogPostRepository.save(existing);
    }

    public void deletePost(UUID id) {
        blogPostRepository.deleteById(id);
    }

    public BlogPost toggleStar(UUID id) {
        BlogPost existing = blogPostRepository.findById(id)
                .orElseThrow(() -> new RuntimeException("Post with id " + id + " not found"));
        existing.setStarred(!existing.isStarred());
        return blogPostRepository.save(existing);
    }
}
