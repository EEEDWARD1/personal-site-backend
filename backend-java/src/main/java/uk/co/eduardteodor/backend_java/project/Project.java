package uk.co.eduardteodor.backend_java.project;

import jakarta.persistence.*;
import java.time.LocalDateTime;
import java.util.UUID;

@Entity
@Table(name = "projects")
public class Project {
    @Id
    @GeneratedValue(strategy = GenerationType.UUID)
    private UUID id;

    @Column(nullable = false, unique = true)
    private String title;

    @Column(columnDefinition = "TEXT", nullable = false)
    private String description;

    private String summary;

    @Column(name = "tech_stack", columnDefinition = "TEXT")
    private String techStack;

    @Column(name = "github_url")
    private String githubUrl;

    @Column(name = "live_url")
    private String liveUrl;

    private String status;

    private boolean featured;

    private boolean published;

    private LocalDateTime createdAt;
    private LocalDateTime updatedAt;

    public Project() {}

    public UUID getId() {return id;}
    public void setId(UUID id) {this.id = id;}

    public String getTitle() {return title;}
    public void setTitle(String title) {this.title = title;}

    public String getDescription() {return description;}
    public void setDescription(String description) {this.description = description;}

    public String getSummary() {return summary;}
    public void setSummary(String summary) {this.summary = summary;}

    public String getTechStack() {return techStack;}
    public void setTechStack(String techStack) {this.techStack = techStack;}

    public String getGithubUrl() {return githubUrl;}
    public void setGithubUrl(String githubURL) {this.githubUrl = githubURL;}

    public String getLiveUrl() {return liveUrl;}
    public void setLiveUrl(String liveURL) {this.liveUrl = liveURL;}

    public String getStatus() {return status;}
    public void setStatus(String status) {this.status = status;}

    public boolean isFeatured() {return featured;}
    public void setFeatured(boolean featured) {this.featured = featured;}

    public boolean isPublished() {return published;}
    public void setPublished(boolean published) {this.published = published;}

    public LocalDateTime getCreatedAt() {return createdAt;}
    public void setCreatedAt(LocalDateTime createdAt) {this.createdAt = createdAt;}

    public LocalDateTime getUpdatedAt() {return updatedAt;}
    public void setUpdatedAt(LocalDateTime updatedAt) {this.updatedAt = updatedAt;}
}
