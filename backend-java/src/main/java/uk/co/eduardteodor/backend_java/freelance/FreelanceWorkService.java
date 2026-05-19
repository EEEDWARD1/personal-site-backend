package uk.co.eduardteodor.backend_java.freelance;

import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

@Service
public class FreelanceWorkService {
    private final FreelanceWorkRepository freelanceWorkRepository;

    // Public
    public FreelanceWorkService(FreelanceWorkRepository freelanceWorkRepository) {
        this.freelanceWorkRepository = freelanceWorkRepository;
    }

    public List<FreelanceWork> getAllPublishedFreelanceWork() {
        return freelanceWorkRepository.findByPublishedTrue();
    }

    // Admin
    public List<FreelanceWork> getAllFreelanceWork() {
        return freelanceWorkRepository.findAll();
    }

    public Optional<FreelanceWork> getFreelanceWorkByID(UUID uuid) {
        return freelanceWorkRepository.findById(uuid);
    }

    public FreelanceWork createFreelanceWork(FreelanceWork freelanceWork) {
        return freelanceWorkRepository.save(freelanceWork);
    }

    public FreelanceWork updateFreelanceWork(UUID id, FreelanceWork updated) {
        FreelanceWork existing = freelanceWorkRepository.findById(id)
                .orElseThrow(() -> new RuntimeException("Post with id " + id + " not found"));

        existing.setClientName(updated.getClientName());
        existing.setProjectTitle(updated.getProjectTitle());
        existing.setDescription(updated.getDescription());
        existing.setServices(updated.getServices());
        existing.setTestimonial(updated.getTestimonial());
        existing.setWebsiteUrl(updated.getWebsiteUrl());
        existing.setFeatured(updated.getFeatured());
        existing.setPublished(updated.getPublished());
        existing.setCompletedAt(updated.getCompletedAt());
        existing.setUpdatedAt(updated.getUpdatedAt());

        return freelanceWorkRepository.save(existing);
    }

    public void deleteFreelanceWork(UUID uuid) {
        freelanceWorkRepository.deleteById(uuid);
    }
}
