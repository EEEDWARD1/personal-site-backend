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

    // ADMIN ENDPOINTS
    @GetMapping("/admin/blog")
    public List<BlogPost> getAllPosts() {
        return blogPostService.getAllPosts();
    }

    @PostMapping("/admin/blog")
    public ResponseEntity<BlogPost> createPost(@RequestBody BlogPost blogPost) {
        return ResponseEntity.status(201).body(blogPostService.createPost(blogPost));
    }

    @PutMapping("/admin/blog/{uuid}")
    public BlogPost updatePost(@PathVariable UUID uuid, @RequestBody BlogPost blogPost) {
        return blogPostService.updatePost(uuid, blogPost);
    }

    @DeleteMapping("/admin/blog/{uuid}")
    public ResponseEntity<Void> deletePost(@PathVariable UUID uuid) {
        blogPostService.deletePost(uuid);
        return ResponseEntity.noContent().build();
    }

    @PatchMapping("/admin/blog/{uuid}/star")
    public ResponseEntity<BlogPost> starPost(@PathVariable UUID uuid) {
        return ResponseEntity.ok(blogPostService.toggleStar(uuid));
    }

}
