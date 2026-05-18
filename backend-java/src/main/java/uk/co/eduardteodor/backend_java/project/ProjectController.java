package uk.co.eduardteodor.backend_java.project;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.UUID;

@RestController
@RequestMapping("/api")
public class ProjectController {
    private final ProjectService projectService;

    public ProjectController(ProjectService projectService) {
        this.projectService = projectService;
    }

    //Public Endpoints
    @GetMapping("/projects")
    public List<Project> getAllPublishedProjects() {
        return projectService.getAllPublishedProjects();
    }

    @GetMapping("/projects/homepage")
    public List<Project> getHomepageProjects() {
        return projectService.getHomepageProjects();
    }

    @GetMapping("/projects/{uuid}")
    public ResponseEntity<Project> getProject(@PathVariable UUID uuid) {
        return projectService.getProjectById(uuid)
                .map(ResponseEntity::ok)
                .orElse(ResponseEntity.notFound().build());
    }

}
