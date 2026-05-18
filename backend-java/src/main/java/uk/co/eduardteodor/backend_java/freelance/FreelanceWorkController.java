package uk.co.eduardteodor.backend_java.freelance;

import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api")
public class FreelanceWorkController {
    private final FreelanceWorkService freelanceWorkService;

    public FreelanceWorkController(
            FreelanceWorkService freelanceWorkService
    ) {
        this.freelanceWorkService = freelanceWorkService;
    }

    @GetMapping("/freelance")
    public List<FreelanceWork> getFreelanceWorks() {
        return freelanceWorkService.getAllPublishedFreelanceWork();
    }
}
