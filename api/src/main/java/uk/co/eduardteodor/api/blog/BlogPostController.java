package uk.co.eduardteodor.api.blog;

import jakarta.validation.Valid;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.Size;
import org.springframework.dao.DataIntegrityViolationException;
import org.springframework.data.domain.Sort;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.server.ResponseStatusException;

import java.time.LocalDateTime;
import java.util.List;
import java.util.UUID;

@RestController
public class BlogPostController {

    private static final Sort PUBLIC_SORT = Sort.by(
            Sort.Order.desc("publishedAt"),
            Sort.Order.desc("createdAt")
    );

    private static final Sort ADMIN_SORT = Sort.by(Sort.Order.desc("updatedAt"));

    private final BlogPostRepository blogPostRepository;

    public BlogPostController(BlogPostRepository blogPostRepository) {
        this.blogPostRepository = blogPostRepository;
    }

    @GetMapping("/api/blog/posts")
    public List<BlogPostResponse> listPublishedPosts(@RequestParam(defaultValue = "false") boolean starredOnly) {
        List<BlogPost> posts = starredOnly
                ? blogPostRepository.findByPublishedTrueAndStarredTrue(PUBLIC_SORT)
                : blogPostRepository.findByPublishedTrue(PUBLIC_SORT);

        return posts.stream().map(BlogPostResponse::from).toList();
    }

    @GetMapping("/api/blog/posts/{slug}")
    public BlogPostResponse getPublishedPost(@PathVariable String slug) {
        BlogPost post = blogPostRepository.findBySlugAndPublishedTrue(slug)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Blog post not found"));
        return BlogPostResponse.from(post);
    }

    @GetMapping("/api/admin/blog-posts")
    public List<BlogPostResponse> listAllPosts() {
        return blogPostRepository.findAll(ADMIN_SORT).stream().map(BlogPostResponse::from).toList();
    }

    @GetMapping("/api/admin/blog-posts/{id}")
    public BlogPostResponse getPostById(@PathVariable UUID id) {
        BlogPost post = blogPostRepository.findById(id)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Blog post not found"));
        return BlogPostResponse.from(post);
    }

    @PostMapping("/api/admin/blog-posts")
    public ResponseEntity<BlogPostResponse> createPost(@Valid @RequestBody BlogPostUpsertRequest request) {
        BlogPost post = new BlogPost();
        apply(post, request);
        return ResponseEntity.status(HttpStatus.CREATED).body(BlogPostResponse.from(save(post)));
    }

    @PutMapping("/api/admin/blog-posts/{id}")
    public BlogPostResponse updatePost(@PathVariable UUID id, @Valid @RequestBody BlogPostUpsertRequest request) {
        BlogPost post = blogPostRepository.findById(id)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Blog post not found"));
        apply(post, request);
        return BlogPostResponse.from(save(post));
    }

    @DeleteMapping("/api/admin/blog-posts/{id}")
    public ResponseEntity<Void> deletePost(@PathVariable UUID id) {
        if (!blogPostRepository.existsById(id)) {
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Blog post not found");
        }
        blogPostRepository.deleteById(id);
        return ResponseEntity.noContent().build();
    }

    private void apply(BlogPost post, BlogPostUpsertRequest request) {
        post.setSlug(request.slug().trim());
        post.setTitle(request.title().trim());
        post.setContent(request.content());
        post.setExcerpt(request.excerpt() == null ? null : request.excerpt().trim());
        post.setStarred(request.starred());
        post.setPublished(request.published());

        LocalDateTime publishedAt = request.publishedAt();
        if (request.published()) {
            post.setPublishedAt(publishedAt != null ? publishedAt : post.getPublishedAt() != null ? post.getPublishedAt() : LocalDateTime.now());
        } else {
            post.setPublishedAt(publishedAt);
        }
    }

    private BlogPost save(BlogPost post) {
        try {
            return blogPostRepository.save(post);
        } catch (DataIntegrityViolationException ex) {
            throw new ResponseStatusException(HttpStatus.CONFLICT, "Blog post slug must be unique");
        }
    }

    public record BlogPostUpsertRequest(
            @NotBlank @Size(max = 255) String slug,
            @NotBlank @Size(max = 255) String title,
            @NotBlank String content,
            @Size(max = 500) String excerpt,
            boolean starred,
            boolean published,
            LocalDateTime publishedAt
    ) {
    }

    public record BlogPostResponse(
            UUID id,
            String slug,
            String title,
            String content,
            String excerpt,
            boolean starred,
            boolean published,
            LocalDateTime publishedAt,
            LocalDateTime createdAt,
            LocalDateTime updatedAt
    ) {
        static BlogPostResponse from(BlogPost post) {
            return new BlogPostResponse(
                    post.getId(),
                    post.getSlug(),
                    post.getTitle(),
                    post.getContent(),
                    post.getExcerpt(),
                    post.isStarred(),
                    post.isPublished(),
                    post.getPublishedAt(),
                    post.getCreatedAt(),
                    post.getUpdatedAt()
            );
        }
    }
}
