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
    public ResponseEntity<Project> getPublishedProject(@PathVariable UUID uuid) {
        return projectService.getPublishedProjectById(uuid)
                .map(ResponseEntity::ok)
                .orElse(ResponseEntity.notFound().build());
    }

    //Admin Endpoints
    @GetMapping("/admin/projects")
    public List<Project> getAllProjects() {
        return projectService.getAllProjects();
    }

    @GetMapping("/admin/projects/{uuid}")
    public ResponseEntity<Project> getProjectById(@PathVariable UUID uuid) {
        return projectService.getProjectById(uuid)
                .map(ResponseEntity::ok)
                .orElse(ResponseEntity.notFound().build());
    }

    @PostMapping("/admin/projects")
    public ResponseEntity<Project> createProject(@RequestBody Project project) {
        return ResponseEntity.status(201).body(projectService.createProject(project));
    }

    @PutMapping("/admin/projects/{uuid}")
    public Project updateProject(@PathVariable UUID uuid, @RequestBody Project project) {
        return projectService.updateProject(uuid, project);
    }

    @DeleteMapping("/admin/projects/{uuid}")
    public ResponseEntity<Void> deletePost(@PathVariable UUID uuid) {
        projectService.deleteProject(uuid);
        return ResponseEntity.noContent().build();
    }

}
