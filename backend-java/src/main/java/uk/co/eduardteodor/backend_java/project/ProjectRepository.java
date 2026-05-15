package uk.co.eduardteodor.backend_java.project;

import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

@Repository
public interface ProjectRepository extends JpaRepository<Project, UUID> {
    List<Project> findByPublishedTrue();
    List<Project> findByFeaturedTrueAndPublishedTrue();
    List<Project> findTop3ByFeaturedTrueAndPublishedTrue();
}