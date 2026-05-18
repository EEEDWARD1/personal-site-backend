package uk.co.eduardteodor.backend_java.blog;

import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

@Repository
public interface BlogPostRepository extends JpaRepository<BlogPost, UUID>{
    List<BlogPost> findByPublishedTrue();

    Optional<BlogPost> findBySlugAndPublishedTrue(String slug);

    @Query("SELECT b FROM BlogPost b WHERE b.published = true ORDER BY b.starred DESC, b.publishedAt DESC LIMIT 3")
    List<BlogPost> findHomepagePosts();
}
