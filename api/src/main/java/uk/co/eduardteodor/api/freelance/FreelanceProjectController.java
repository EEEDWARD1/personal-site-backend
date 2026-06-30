package uk.co.eduardteodor.api.freelance;

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

import java.time.LocalDateTime;
import java.util.List;
import java.util.UUID;

@RestController
public class FreelanceProjectController {

    private static final Sort PUBLIC_SORT = Sort.by(
            Sort.Order.asc("displayOrder"),
            Sort.Order.desc("featured"),
            Sort.Order.desc("completedAt"),
            Sort.Order.desc("createdAt")
    );

    private static final Sort ADMIN_SORT = Sort.by(
            Sort.Order.asc("displayOrder"),
            Sort.Order.desc("updatedAt")
    );

    private final FreelanceProjectRepository freelanceProjectRepository;

    public FreelanceProjectController(FreelanceProjectRepository freelanceProjectRepository) {
        this.freelanceProjectRepository = freelanceProjectRepository;
    }

    @GetMapping("/api/freelance")
    public List<FreelanceProjectResponse> listPublishedProjects(@RequestParam(defaultValue = "false") boolean featuredOnly) {
        List<FreelanceProject> projects = featuredOnly
                ? freelanceProjectRepository.findByPublishedTrueAndFeaturedTrue(PUBLIC_SORT)
                : freelanceProjectRepository.findByPublishedTrue(PUBLIC_SORT);

        return projects.stream().map(FreelanceProjectResponse::from).toList();
    }

    @GetMapping("/api/freelance/{slug}")
    public FreelanceProjectResponse getPublishedProject(@PathVariable String slug) {
        FreelanceProject project = freelanceProjectRepository.findBySlugAndPublishedTrue(slug)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Freelance project not found"));
        return FreelanceProjectResponse.from(project);
    }

    @GetMapping("/api/admin/freelance-projects")
    public List<FreelanceProjectResponse> listAllProjects() {
        return freelanceProjectRepository.findAll(ADMIN_SORT).stream().map(FreelanceProjectResponse::from).toList();
    }

    @GetMapping("/api/admin/freelance-projects/{id}")
    public FreelanceProjectResponse getProjectById(@PathVariable UUID id) {
        FreelanceProject project = freelanceProjectRepository.findById(id)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Freelance project not found"));
        return FreelanceProjectResponse.from(project);
    }

    @PostMapping("/api/admin/freelance-projects")
    public ResponseEntity<FreelanceProjectResponse> createProject(@Valid @RequestBody FreelanceProjectUpsertRequest request) {
        FreelanceProject project = new FreelanceProject();
        apply(project, request);
        return ResponseEntity.status(HttpStatus.CREATED).body(FreelanceProjectResponse.from(save(project)));
    }

    @PutMapping("/api/admin/freelance-projects/{id}")
    public FreelanceProjectResponse updateProject(@PathVariable UUID id, @Valid @RequestBody FreelanceProjectUpsertRequest request) {
        FreelanceProject project = freelanceProjectRepository.findById(id)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Freelance project not found"));
        apply(project, request);
        return FreelanceProjectResponse.from(save(project));
    }

    @DeleteMapping("/api/admin/freelance-projects/{id}")
    public ResponseEntity<Void> deleteProject(@PathVariable UUID id) {
        if (!freelanceProjectRepository.existsById(id)) {
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Freelance project not found");
        }
        freelanceProjectRepository.deleteById(id);
        return ResponseEntity.noContent().build();
    }

    private void apply(FreelanceProject project, FreelanceProjectUpsertRequest request) {
        project.setSlug(request.slug().trim());
        project.setClientName(request.clientName() == null ? null : request.clientName().trim());
        project.setProjectTitle(request.projectTitle().trim());
        project.setSummary(request.summary() == null ? null : request.summary().trim());
        project.setDescription(request.description());
        project.setServices(request.services() == null ? List.of() : request.services().stream().map(String::trim).filter(s -> !s.isBlank()).toList());
        project.setTestimonial(request.testimonial() == null ? null : request.testimonial().trim());
        project.setWebsiteUrl(request.websiteUrl() == null ? null : request.websiteUrl().trim());
        project.setThumbnailUrl(request.thumbnailUrl() == null ? null : request.thumbnailUrl().trim());
        project.setHeroURL(request.heroUrl() == null ? null : request.heroUrl().trim());
        project.setFeatured(request.featured());
        project.setPublished(request.published());
        project.setCompletedAt(request.completedAt());
        project.setDisplayOrder(request.displayOrder());
    }

    private FreelanceProject save(FreelanceProject project) {
        try {
            return freelanceProjectRepository.save(project);
        } catch (DataIntegrityViolationException ex) {
            throw new ResponseStatusException(HttpStatus.CONFLICT, "Freelance project slug must be unique");
        }
    }

    public record FreelanceProjectUpsertRequest(
            @NotBlank @Size(max = 255) String slug,
            @Size(max = 255) String clientName,
            @NotBlank @Size(max = 255) String projectTitle,
            @Size(max = 500) String summary,
            @NotBlank String description,
            List<String> services,
            String testimonial,
            @Size(max = 500) String websiteUrl,
            @Size(max = 500) String thumbnailUrl,
            @Size(max = 500) String heroUrl,
            boolean featured,
            boolean published,
            LocalDateTime completedAt,
            int displayOrder
    ) {
    }

    public record FreelanceProjectResponse(
            UUID id,
            String slug,
            String clientName,
            String projectTitle,
            String summary,
            String description,
            List<String> services,
            String testimonial,
            String websiteUrl,
            String thumbnailUrl,
            String heroUrl,
            boolean featured,
            boolean published,
            LocalDateTime completedAt,
            int displayOrder,
            LocalDateTime createdAt,
            LocalDateTime updatedAt
    ) {
        static FreelanceProjectResponse from(FreelanceProject project) {
            return new FreelanceProjectResponse(
                    project.getId(),
                    project.getSlug(),
                    project.getClientName(),
                    project.getProjectTitle(),
                    project.getSummary(),
                    project.getDescription(),
                    project.getServices(),
                    project.getTestimonial(),
                    project.getWebsiteUrl(),
                    project.getThumbnailUrl(),
                    project.getHeroURL(),
                    project.isFeatured(),
                    project.isPublished(),
                    project.getCompletedAt(),
                    project.getDisplayOrder(),
                    project.getCreatedAt(),
                    project.getUpdatedAt()
            );
        }
    }
}
