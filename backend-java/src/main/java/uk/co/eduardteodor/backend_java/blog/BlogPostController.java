package uk.co.eduardteodor.backend_java.blog;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

@RestController
@RequestMapping("/api")
public class BlogPostController {
    private final BlogPostService blogPostService;

    public BlogPostController(BlogPostService blogPostService) {
        this.blogPostService = blogPostService;
    }

    // PUBLIC ENDPOINTS
    @GetMapping("/blog")
    public List<BlogPost> getAllPublishedPosts() {
        return blogPostService.getAllPublishedPosts();
    }

    @GetMapping("/blog/homepage")
    public List<BlogPost> getAllPublishedPostsHomepage() {
        return blogPostService.getHomepagePosts();
    }

    @GetMapping("/blog/{slug}")
    public ResponseEntity<BlogPost> getPostBySlug(@PathVariable String slug) {
        return blogPostService.getPostBySlug(slug)
                .map(ResponseEntity::ok)
                .orElse(ResponseEntity.notFound().build());
    }

}
