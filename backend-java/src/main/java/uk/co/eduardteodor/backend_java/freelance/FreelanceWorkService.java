package uk.co.eduardteodor.backend_java.freelance;


import org.springframework.stereotype.Service;

import java.util.List;

@Service
public class FreelanceWorkService {
    private final FreelanceWorkRepository freelanceWorkRepository;

    public FreelanceWorkService(FreelanceWorkRepository freelanceWorkRepository) {
        this.freelanceWorkRepository = freelanceWorkRepository;
    }

    public List<FreelanceWork> getALlPublishedFreelanceWork() {
        return freelanceWorkRepository.findByPublishedTrue();
    }
}
