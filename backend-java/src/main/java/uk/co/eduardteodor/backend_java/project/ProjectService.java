package uk.co.eduardteodor.backend_java.project;

import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.stereotype.Service;
import uk.co.eduardteodor.backend_java.user.User;
import uk.co.eduardteodor.backend_java.user.UserRepository;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

@Service
public class ProjectService {
    private final ProjectRepository projectRepository;
    private final UserRepository userRepository;

    public ProjectService(ProjectRepository projectRepository, UserRepository userRepository) {
        this.projectRepository = projectRepository;
        this.userRepository = userRepository;
    }

    // PUBLIC
    public List<Project> getAllPublishedProjects() {
        return projectRepository.findByPublishedTrue();
    }

    public List<Project> getHomepageProjects() {
        return projectRepository.findHomepageProjects();
    }

    public Optional<Project> getPublishedProjectById(UUID id) {
        return projectRepository.findByIdAndPublishedTrue(id);
    }


    public List<Project> getAllProjects() {
        return projectRepository.findAll();
    }

    public Optional<Project> getProjectById(UUID uuid) {
        return projectRepository.findById(uuid);
    }

    public Project createProject(Project project) {
        String username = SecurityContextHolder.getContext().getAuthentication().getName();
        User user = userRepository.findByUsername(username).orElseThrow(() -> new RuntimeException("User not found"));

        project.setUserId(user.getId());
        return projectRepository.save(project);
    }

    public Project updateProject(UUID id, Project updated) {
        Project existing = projectRepository.findById(id)
                .orElseThrow(() -> new RuntimeException("Post with id " + id + " not found"));
        existing.setTitle(updated.getTitle());
        existing.setDescription(updated.getDescription());
        existing.setSummary(updated.getSummary());
        existing.setTechStack(updated.getTechStack());
        existing.setGithubUrl(updated.getGithubUrl());
        existing.setLiveUrl(updated.getLiveUrl());
        existing.setStatus(updated.getStatus());
        existing.setFeatured(updated.isFeatured());
        existing.setPublished(updated.isPublished());
        existing.setUpdatedAt(updated.getUpdatedAt());

        return projectRepository.save(existing);
    }

    public void deleteProject(UUID uuid) {
        projectRepository.deleteById(uuid);
    }
}