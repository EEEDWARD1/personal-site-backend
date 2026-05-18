package uk.co.eduardteodor.backend_java.freelance;

import jakarta.persistence.*;
import java.time.LocalDateTime;
import java.util.UUID;

@Entity
@Table(name = "freelance_work")
public class FreelanceWork {
    @Id
    @GeneratedValue(strategy = GenerationType.UUID)
    private UUID id;

    @Column(name = "user_id")
    private UUID userId;

    @Column(name = "client_name")
    private String clientName;

    @Column(name = "project_title", nullable = false)
    private String projectTitle;

    @Column(nullable = false, columnDefinition = "TEXT")
    private String description;

    @Column(columnDefinition = "TEXT")
    private String services;

    @Column(columnDefinition = "TEXT")
    private String testimonial;

    @Column(name = "website_url")
    private String websiteUrl;

    private Boolean featured;

    private Boolean published;

    @Column(name = "completed_at")
    private LocalDateTime completedAt;

    @Column(name = "created_at")
    private LocalDateTime createdAt;

    @Column(name = "updated_at")
    private LocalDateTime updatedAt;

    public FreelanceWork() {}

    public UUID getId() { return id;}
    public void setId(UUID id) { this.id = id; }

    public UUID getUserId() { return userId; }
    public void setUserId(UUID userId) { this.userId = userId; }

    public String getClientName() { return clientName;}
    public void setClientName(String clientName) { this.clientName = clientName; }

    public String getProjectTitle() { return projectTitle;}
    public void setProjectTitle(String projectTitle) { this.projectTitle = projectTitle; }

    public String getDescription() { return description;}
    public void setDescription(String description) { this.description = description; }

    public String getServices() { return services;}
    public void setServices(String services) { this.services = services; }

    public String getTestimonial() { return testimonial;}
    public void setTestimonial(String testimonial) { this.testimonial = testimonial; }

    public String getWebsiteUrl() { return websiteUrl;}
    public void setWebsiteUrl(String websiteUrl) { this.websiteUrl = websiteUrl; }

    public Boolean getFeatured() { return featured;}
    public void setFeatured(Boolean featured) { this.featured = featured; }

    public Boolean getPublished() { return published;}
    public void setPublished(Boolean published) { this.published = published; }

    public LocalDateTime getCompletedAt() { return completedAt;}
    public void setCompletedAt(LocalDateTime completedAt) { this.completedAt = completedAt; }

    public LocalDateTime getCreatedAt() { return createdAt;}
    public void setCreatedAt(LocalDateTime createdAt) { this.createdAt = createdAt; }

    public LocalDateTime getUpdatedAt() { return updatedAt;}
    public void setUpdatedAt(LocalDateTime updatedAt) { this.updatedAt = updatedAt; }
}
