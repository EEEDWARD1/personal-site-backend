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
    public List<Project> getPublishedProjects() {
        return projectService.getAllPublishedProjects();
    }

    @GetMapping("/projects/featured")
    public List<Project> getAllFeaturedProjectsAndPublishedProject() {
        return projectService.getAllFeaturedAndPublishedProject();
    }

    @GetMapping("/projects/{id}")
    public ResponseEntity<Project> getProjectById(@PathVariable UUID id) {
        return projectService.getProjectById(id)
                .map(ResponseEntity::ok)
                .orElse(ResponseEntity.notFound().build());
    }

    @GetMapping("/admin/projects")
    public List<Project> getAllProjects() {
        return projectService.getAllProjects();
    }

    @PostMapping("/admin/projects")
    public Project createProject(@RequestBody Project project){
        return projectService.createProject(project);
    }

    @PutMapping("/admin/projects/{id}")
    public Project updateProject(@PathVariable UUID id, @RequestBody Project project){
        return projectService.updateProject(id, project);
    }

    @DeleteMapping("/admin/projects/{id}")
    public ResponseEntity<Void> deleteProject(@PathVariable UUID id) {
        projectService.deleteProject(id);
        return ResponseEntity.noContent().build();
    }

}
