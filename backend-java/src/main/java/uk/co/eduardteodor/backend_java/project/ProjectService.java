package uk.co.eduardteodor.backend_java.project;

import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

@Service
public class ProjectService {
    private final ProjectRepository projectRepository;

    public ProjectService(ProjectRepository projectRepository) {this.projectRepository = projectRepository;}

    //--------------------------------
    //Public - anyone can call this
    //--------------------------------

    //Get all Published projects
    public List<Project> getAllPublishedProjects() {
        return projectRepository.findByPublishedTrue();
    }

    //Get all Featured projects published projects
    public List<Project> getAllFeaturedAndPublishedProject() {
        return projectRepository.findByFeaturedTrueAndPublishedTrue();
    }
    //Get top 3 Featured projects
    public List<Project> getLatestProjects() {
        return projectRepository.findTop3ByFeaturedTrueAndPublishedTrue();
    }

    public Optional<Project> getProjectById(UUID id) {
        return projectRepository.findById(id);
    }

    //-----------------------------------
    //Admin - ONLY ADMIN CAN ACCESS THIS
    //-----------------------------------

    //Get all projects
    public List<Project> getAllProjects() {
        return projectRepository.findAll();
    }

    //Create Project
    public Project createProject(Project project) {
        return projectRepository.save(project);
    }

    //Update Project
    public Project updateProject(UUID id, Project updated){
        Project existing = projectRepository.findById(id).orElseThrow(() -> new RuntimeException("Post with id " + id + " not found"));

        existing.setTitle(updated.getTitle());
        existing.setDescription(updated.getDescription());
        existing.setSummary(updated.getSummary());
        existing.setTechStack(updated.getTechStack());
        existing.setGithubUrl(updated.getGithubUrl());
        existing.setLiveUrl(updated.getLiveUrl());
        existing.setStatus(updated.getStatus());
        existing.setFeatured(updated.isFeatured());
        existing.setPublished(updated.isPublished());

        return projectRepository.save(existing);
    }
    //Delete Project
    public void deleteProject(UUID id) {projectRepository.deleteById(id);}

}
