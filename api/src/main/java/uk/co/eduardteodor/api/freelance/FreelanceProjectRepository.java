package uk.co.eduardteodor.api.freelance;

import org.springframework.data.domain.Sort;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

public interface FreelanceProjectRepository extends JpaRepository<FreelanceProject, UUID> {
    List<FreelanceProject> findByPublishedTrue(Sort sort);

    List<FreelanceProject> findByPublishedTrueAndFeaturedTrue(Sort sort);

    Optional<FreelanceProject> findBySlug(String slug);

    Optional<FreelanceProject> findBySlugAndPublishedTrue(String slug);
}
