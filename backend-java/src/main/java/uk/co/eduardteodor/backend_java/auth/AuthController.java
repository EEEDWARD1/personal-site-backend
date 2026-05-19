package uk.co.eduardteodor.backend_java.auth;

import org.springframework.http.ResponseEntity;
import org.springframework.security.authentication.AuthenticationManager;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.core.Authentication;
import org.springframework.security.core.AuthenticationException;
import org.springframework.web.bind.annotation.*;
import uk.co.eduardteodor.backend_java.config.JwtUtil;
import uk.co.eduardteodor.backend_java.user.User;
import uk.co.eduardteodor.backend_java.user.UserRepository;

import java.util.Map;

@RestController
@RequestMapping("/api/auth")
public class AuthController {

    private final AuthenticationManager authenticationManager;
    private final JwtUtil jwtUtil;
    private final UserRepository userRepository;
    private final MfaService mfaService;

    public AuthController(AuthenticationManager authenticationManager, JwtUtil jwtUtil, UserRepository userRepository, MfaService mfaService) {
        this.authenticationManager = authenticationManager;
        this.jwtUtil = jwtUtil;
        this.userRepository = userRepository;
        this.mfaService = mfaService;
    }

    @PostMapping("/login")
    public ResponseEntity<?> login(@RequestBody LoginRequest loginRequest) {
        try {
            Authentication authentication = authenticationManager.authenticate(
                    new UsernamePasswordAuthenticationToken(
                            loginRequest.username(),
                            loginRequest.password()
                    )
            );

            // Fetch user to check MFA status
            User user = userRepository.findByUsername(loginRequest.username())
                    .orElseThrow(() -> new RuntimeException("User not found"));

            // Step B — MFA enabled, don't issue JWT yet
            if (user.isMfaEnabled()) {
                // Issue a short-lived pre-auth token so Step D can be verified securely
                String preAuthToken = jwtUtil.generatePreAuthToken(loginRequest.username());
                return ResponseEntity.ok(Map.of(
                        "status", "MFA_REQUIRED",
                        "preAuthToken", preAuthToken
                ));
            }

            // No MFA — issue full JWT immediately
            String token = jwtUtil.generateToken(authentication.getName());
            return ResponseEntity.ok(new LoginResponse(token));

        } catch (AuthenticationException e) {
            return ResponseEntity.status(401).body("Invalid credentials");
        }
    }

    // Step D — verify MFA code and issue full JWT
    @PostMapping("/mfa/login")
    public ResponseEntity<?> verifyMfaLogin(@RequestBody MfaLoginRequest request) {
        // Validate pre-auth token
        if (!jwtUtil.isTokenValid(request.preAuthToken())) {
            return ResponseEntity.status(401).body("Invalid or expired pre-auth token");
        }

        String username = jwtUtil.extractUsername(request.preAuthToken());
        User user = userRepository.findByUsername(username)
                .orElseThrow(() -> new RuntimeException("User not found"));

        // Step D — verify the 6-digit code
        if (mfaService.verifyCode(user.getMfaSecret(), request.code())) {
            String token = jwtUtil.generateToken(username);
            return ResponseEntity.ok(new LoginResponse(token));
        }

        return ResponseEntity.status(401).body("Invalid MFA code");
    }

    @PostMapping("/mfa/setup")
    public ResponseEntity<?> setupMfa(Authentication authentication) {
        String username = authentication.getName();
        User user = userRepository.findByUsername(username)
                .orElseThrow(() -> new RuntimeException("User not found"));

        // Generate and save secret
        String secret = mfaService.generateSecret();
        user.setMfaSecret(secret);
        user.setMfaEnabled(false); // not enabled until first successful verify
        userRepository.save(user);

        // Return QR code for scanning
        String qrCode = mfaService.generateQrCodeBase64(secret, username, "Eduard Personal Site");
        return ResponseEntity.ok(Map.of(
                "secret", secret,  // show once, save to password manager
                "qrCode", qrCode   // base64 PNG for frontend to display
        ));
    }

    @PostMapping("/mfa/verify-setup")
    public ResponseEntity<?> verifySetup(@RequestBody MfaVerifyRequest request,
                                         Authentication authentication) {
        String username = authentication.getName();
        User user = userRepository.findByUsername(username)
                .orElseThrow(() -> new RuntimeException("User not found"));

        if (mfaService.verifyCode(user.getMfaSecret(), request.code())) {
            user.setMfaEnabled(true);
            userRepository.save(user);
            return ResponseEntity.ok(Map.of("message", "MFA enabled successfully"));
        }
        return ResponseEntity.status(400).body(Map.of("message", "Invalid code"));
    }
}