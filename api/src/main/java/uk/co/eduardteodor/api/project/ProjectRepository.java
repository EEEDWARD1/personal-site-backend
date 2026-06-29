package uk.co.eduardteodor.api.project;

import org.springframework.data.domain.Sort;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

public interface ProjectRepository extends JpaRepository<Project, UUID> {
    List<Project> findByPublishedTrue(Sort sort);

    List<Project> findByPublishedTrueAndFeaturedTrue(Sort sort);

    Optional<Project> findBySlug(String slug);

    Optional<Project> findBySlugAndPublishedTrue(String slug);
}
