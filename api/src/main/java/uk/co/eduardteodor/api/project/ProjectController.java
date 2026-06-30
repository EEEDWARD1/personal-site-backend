package uk.co.eduardteodor.api.project;

import jakarta.validation.Valid;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.Size;
import org.springframework.dao.DataIntegrityViolationException;
import org.springframework.data.domain.Sort;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.server.ResponseStatusException;

import java.util.List;
import java.util.UUID;

@RestController
public class ProjectController {

    private static final Sort PUBLIC_SORT = Sort.by(
            Sort.Order.asc("displayOrder"),
            Sort.Order.desc("featured"),
            Sort.Order.desc("createdAt")
    );

    private static final Sort ADMIN_SORT = Sort.by(
            Sort.Order.asc("displayOrder"),
            Sort.Order.desc("updatedAt")
    );

    private final ProjectRepository projectRepository;

    public ProjectController(ProjectRepository projectRepository) {
        this.projectRepository = projectRepository;
    }

    @GetMapping("/api/projects")
    public List<ProjectResponse> listPublishedProjects(@RequestParam(defaultValue = "false") boolean featuredOnly) {
        List<Project> projects = featuredOnly
                ? projectRepository.findByPublishedTrueAndFeaturedTrue(PUBLIC_SORT)
                : projectRepository.findByPublishedTrue(PUBLIC_SORT);

        return projects.stream().map(ProjectResponse::from).toList();
    }

    @GetMapping("/api/projects/{slug}")
    public ProjectResponse getPublishedProject(@PathVariable String slug) {
        Project project = projectRepository.findBySlugAndPublishedTrue(slug)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Project not found"));
        return ProjectResponse.from(project);
    }

    @GetMapping("/api/admin/projects")
    public List<ProjectResponse> listAllProjects() {
        return projectRepository.findAll(ADMIN_SORT).stream().map(ProjectResponse::from).toList();
    }

    @GetMapping("/api/admin/projects/{id}")
    public ProjectResponse getProjectById(@PathVariable UUID id) {
        Project project = projectRepository.findById(id)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Project not found"));
        return ProjectResponse.from(project);
    }

    @PostMapping("/api/admin/projects")
    public ResponseEntity<ProjectResponse> createProject(@Valid @RequestBody ProjectUpsertRequest request) {
        Project project = new Project();
        apply(project, request);
        return ResponseEntity.status(HttpStatus.CREATED).body(ProjectResponse.from(save(project)));
    }

    @PutMapping("/api/admin/projects/{id}")
    public ProjectResponse updateProject(@PathVariable UUID id, @Valid @RequestBody ProjectUpsertRequest request) {
        Project project = projectRepository.findById(id)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Project not found"));
        apply(project, request);
        return ProjectResponse.from(save(project));
    }

    @DeleteMapping("/api/admin/projects/{id}")
    public ResponseEntity<Void> deleteProject(@PathVariable UUID id) {
        if (!projectRepository.existsById(id)) {
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Project not found");
        }
        projectRepository.deleteById(id);
        return ResponseEntity.noContent().build();
    }

    private void apply(Project project, ProjectUpsertRequest request) {
        project.setSlug(request.slug().trim());
        project.setTitle(request.title().trim());
        project.setSummary(request.summary() == null ? null : request.summary().trim());
        project.setDescription(request.description());
        project.setTechStack(request.techStack());
        project.setGithubUrl(request.githubUrl() == null ? null : request.githubUrl().trim());
        project.setLiveUrl(request.liveUrl() == null ? null : request.liveUrl().trim());
        project.setThumbnailUrl(request.thumbnailUrl() == null ? null : request.thumbnailUrl().trim());
        project.setHeroUrl(request.heroUrl() == null ? null : request.heroUrl().trim());
        project.setStatus(request.status().trim());
        project.setFeatured(request.featured());
        project.setPublished(request.published());
        project.setDisplayOrder(request.displayOrder());
    }

    private Project save(Project project) {
        try {
            return projectRepository.save(project);
        } catch (DataIntegrityViolationException ex) {
            throw new ResponseStatusException(HttpStatus.CONFLICT, "Project slug must be unique");
        }
    }

    public record ProjectUpsertRequest(
            @NotBlank @Size(max = 255) String slug,
            @NotBlank @Size(max = 255) String title,
            @Size(max = 500) String summary,
            @NotBlank String description,
            String techStack,
            @Size(max = 500) String githubUrl,
            @Size(max = 500) String liveUrl,
            @Size(max = 500) String thumbnailUrl,
            @Size(max = 500) String heroUrl,
            @NotBlank @Size(max = 50) String status,
            boolean featured,
            boolean published,
            int displayOrder
    ) {
    }

    public record ProjectResponse(
            UUID id,
            String slug,
            String title,
            String summary,
            String description,
            String techStack,
            String githubUrl,
            String liveUrl,
            String thumbnailUrl,
            String heroUrl,
            String status,
            boolean featured,
            boolean published,
            int displayOrder,
            java.time.LocalDateTime createdAt,
            java.time.LocalDateTime updatedAt
    ) {
        static ProjectResponse from(Project project) {
            return new ProjectResponse(
                    project.getId(),
                    project.getSlug(),
                    project.getTitle(),
                    project.getSummary(),
                    project.getDescription(),
                    project.getTechStack(),
                    project.getGithubUrl(),
                    project.getLiveUrl(),
                    project.getThumbnailUrl(),
                    project.getHeroUrl(),
                    project.getStatus(),
                    project.isFeatured(),
                    project.isPublished(),
                    project.getDisplayOrder(),
                    project.getCreatedAt(),
                    project.getUpdatedAt()
            );
        }
    }
}
