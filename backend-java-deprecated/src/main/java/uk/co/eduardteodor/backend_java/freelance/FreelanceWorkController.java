package uk.co.eduardteodor.backend_java.freelance;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.UUID;

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

    //Admin Endpoints
    @GetMapping("/admin/freelance")
    public List<FreelanceWork> getALlFreelanceWorks() {
        return freelanceWorkService.getAllFreelanceWork();
    }

    @GetMapping("/admin/freelance/{uuid}")
    public ResponseEntity<FreelanceWork> getFreelanceWorkById(@PathVariable UUID uuid) {
        return freelanceWorkService.getFreelanceWorkByID(uuid)
                .map(ResponseEntity::ok)
                .orElse(ResponseEntity.notFound().build());
    }

    @PostMapping("/admin/freelance")
    public ResponseEntity<FreelanceWork> createFreelanceWork(@RequestBody FreelanceWork freelanceWork) {
        return ResponseEntity.status(201).body(freelanceWorkService.createFreelanceWork(freelanceWork));
    }

    @PutMapping("/admin/freelance/{uuid}")
    public FreelanceWork updateFreelanceWork(@PathVariable UUID uuid, @RequestBody FreelanceWork freelanceWork) {
        return freelanceWorkService.updateFreelanceWork(uuid, freelanceWork);
    }

    @DeleteMapping("/admin/freelance/{uuid}")
    public ResponseEntity<Void> deleteFreelanceWork(@PathVariable UUID uuid) {
        freelanceWorkService.deleteFreelanceWork(uuid);
        return ResponseEntity.noContent().build();
    }
}
