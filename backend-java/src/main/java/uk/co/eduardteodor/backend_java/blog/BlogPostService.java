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

    //--------------------------------
    //Public - anyone can call this
    //--------------------------------
    public List<BlogPost> getALlPublishedPosts(){
        return blogPostRepository.findByPublishedTrue();
    }

    public Optional<BlogPost> getPostBySlug(String slug){
        return blogPostRepository.findBySlug(slug);
    }

    public List<BlogPost> getLatestPosts() {
        return blogPostRepository.findTop3ByPublishedTrueOrderByPublishedAtDesc();
    }

    //--------------------------------
    //Admin - ONLY ADMIN CAN ACCESS THIS
    //--------------------------------
    public List<BlogPost> getAllPosts(){
        return blogPostRepository.findAll();
    }

    public BlogPost createPost(BlogPost blogPost){
        return blogPostRepository.save(blogPost);
    }

    public BlogPost updatePost(UUID id, BlogPost updated){
        BlogPost existing = blogPostRepository.findById(id).orElseThrow(() -> new RuntimeException("Post with id " + id + " not found"));

        existing.setTitle(updated.getTitle());
        existing.setSlug(updated.getSlug());
        existing.setContent(updated.getContent());
        existing.setExcerpt(updated.getExcerpt());
        existing.setPublished(updated.isPublished());
        existing.setPublishedAt(updated.getPublishedAt());

        return blogPostRepository.save(existing);
    }

    public void deletePost(UUID id){
        blogPostRepository.deleteById(id);
    }

}
