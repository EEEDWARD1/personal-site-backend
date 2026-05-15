package uk.co.eduardteodor.backend_java.blog;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;
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
    public List<BlogPost> getPublishedPosts() {
        return blogPostService.getALlPublishedPosts();
    }

    @GetMapping("/blog/latest")
    public List<BlogPost> getLatestPosts() {
        return blogPostService.getLatestPosts();
    }

    @GetMapping("/blog/{slug}")
    public ResponseEntity<BlogPost> getPostBySlug(@PathVariable String slug) {
        return blogPostService.getPostBySlug(slug).
                map(ResponseEntity::ok).
                orElse(ResponseEntity.notFound().build());
    }

    // ADMIN ENDPOINTS
    @GetMapping("/admin/blog")
    public List<BlogPost> getAllPosts() {
        return blogPostService.getAllPosts();
    }

    @PostMapping("/admin/blog")
    public BlogPost createPost(@RequestBody BlogPost blogPost) {
        return blogPostService.createPost(blogPost);
    }

    @PutMapping("/admin/blog/{id}")
    public BlogPost updatePost(@PathVariable UUID id, @RequestBody BlogPost blogPost) {
        return blogPostService.updatePost(id, blogPost);
    }

    @DeleteMapping("/admin/blog/{id}")
    public ResponseEntity<Void> deletePost(@PathVariable UUID id) {
        blogPostService.deletePost(id);
        return ResponseEntity.noContent().build();
    }
}
