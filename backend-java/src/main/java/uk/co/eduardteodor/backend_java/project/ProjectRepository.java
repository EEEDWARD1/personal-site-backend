package uk.co.eduardteodor.backend_java.project;

import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

@Repository
public interface ProjectRepository extends JpaRepository<Project, UUID> {
    List<Project> findByPublishedTrue();
    Optional<Project> findByIdAndPublishedTrue(UUID id);

    @Query("SELECT p FROM Project p WHERE p.published = true AND p.featured = true ORDER BY p.createdAt DESC LIMIT 3")
    List<Project> findHomepageProjects();
}