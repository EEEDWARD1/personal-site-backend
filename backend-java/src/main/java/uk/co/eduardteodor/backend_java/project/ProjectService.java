package uk.co.eduardteodor.backend_java.project;

import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

@Service
public class ProjectService {
    private final ProjectRepository projectRepository;

    public ProjectService(ProjectRepository projectRepository) {
        this.projectRepository = projectRepository;
    }

    // PUBLIC
    public List<Project> getAllPublishedProjects() {
        return projectRepository.findByPublishedTrue();
    }

    public List<Project> getHomepageProjects() {
        return projectRepository.findHomepageProjects();
    }

    public Optional<Project> getProjectById(UUID id) {
        return projectRepository.findByIdAndPublishedTrue(id);
    }
}