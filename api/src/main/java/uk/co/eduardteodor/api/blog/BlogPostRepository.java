package uk.co.eduardteodor.api.blog;

import org.springframework.data.domain.Sort;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

public interface BlogPostRepository extends JpaRepository<BlogPost, UUID> {
    List<BlogPost> findByPublishedTrue(Sort sort);

    List<BlogPost> findByPublishedTrueAndStarredTrue(Sort sort);

    Optional<BlogPost> findBySlug(String slug);

    Optional<BlogPost> findBySlugAndPublishedTrue(String slug);
}
