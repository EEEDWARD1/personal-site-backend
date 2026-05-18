package uk.co.eduardteodor.backend_java.freelance;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.UUID;

@RestController
public class FreelanceWorkController {
    private final FreelanceWorkService freelanceWorkService;

    public FreelanceWorkController(
            FreelanceWorkService freelanceWorkService
    ) {
        this.freelanceWorkService = freelanceWorkService;
    }

    @GetMapping("/freelance")
    public List<FreelanceWork> getFreelanceWorks() {
        return freelanceWorkService.getALlPublishedFreelanceWork();
    }
}
