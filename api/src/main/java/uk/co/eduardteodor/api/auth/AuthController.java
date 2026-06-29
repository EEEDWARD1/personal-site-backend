package uk.co.eduardteodor.api.auth;

import jakarta.validation.Valid;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.Pattern;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.security.authentication.AuthenticationManager;
import org.springframework.security.authentication.BadCredentialsException;
import org.springframework.security.core.AuthenticationException;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.server.ResponseStatusException;
import uk.co.eduardteodor.api.config.JwtUtil;
import uk.co.eduardteodor.api.user.User;
import uk.co.eduardteodor.api.user.UserRepository;

@RestController
@RequestMapping("/api/auth")
public class AuthController {

    private final AuthenticationManager authenticationManager;
    private final UserRepository userRepository;
    private final JwtUtil jwtUtil;
    private final MfaService mfaService;

    public AuthController(AuthenticationManager authenticationManager,
                          UserRepository userRepository,
                          JwtUtil jwtUtil,
                          MfaService mfaService) {
        this.authenticationManager = authenticationManager;
        this.userRepository = userRepository;
        this.jwtUtil = jwtUtil;
        this.mfaService = mfaService;
    }

    @PostMapping("/login")
    public ResponseEntity<AuthResponse> login(@Valid @RequestBody LoginRequest request) {
        try {
            authenticationManager.authenticate(
                    new UsernamePasswordAuthenticationToken(request.username(), request.password())
            );
        } catch (BadCredentialsException ex) {
            throw new ResponseStatusException(HttpStatus.UNAUTHORIZED, "Invalid credentials");
        } catch (AuthenticationException ex) {
            throw new ResponseStatusException(HttpStatus.UNAUTHORIZED, "Invalid credentials");
        }

        User user = userRepository.findByUsername(request.username())
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.UNAUTHORIZED, "Invalid credentials"));

        if (!user.isTotpEnabled()) {
            if (user.getTotpSecret() == null || user.getTotpSecret().isBlank()) {
                user.setTotpSecret(mfaService.generateSecret());
                user.setTotpEnabled(false);
                userRepository.save(user);
            }

            return ResponseEntity.ok(AuthResponse.totpSetupRequired(
                    jwtUtil.generatePreAuthToken(user.getUsername()),
                    user.getTotpSecret(),
                    mfaService.generateQrCodeBase64(user.getTotpSecret(), user.getUsername())
            ));
        }

        return ResponseEntity.ok(AuthResponse.totpRequired(jwtUtil.generatePreAuthToken(user.getUsername())));
    }

    @PostMapping("/totp/verify")
    public ResponseEntity<AuthResponse> verifyTotp(@Valid @RequestBody TotpVerificationRequest request) {
        if (!jwtUtil.isTokenValid(request.preAuthToken()) || !jwtUtil.isPreAuthToken(request.preAuthToken())) {
            throw new ResponseStatusException(HttpStatus.UNAUTHORIZED, "Invalid or expired pre-auth token");
        }

        User user = userRepository.findByUsername(jwtUtil.extractUsername(request.preAuthToken()))
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.UNAUTHORIZED, "Invalid pre-auth token"));

        if (user.getTotpSecret() == null || user.getTotpSecret().isBlank()) {
            throw new ResponseStatusException(HttpStatus.CONFLICT, "TOTP setup has not been started");
        }

        if (!mfaService.verifyCode(user.getTotpSecret(), request.code())) {
            throw new ResponseStatusException(HttpStatus.UNAUTHORIZED, "Invalid TOTP code");
        }

        if (!user.isTotpEnabled()) {
            user.setTotpEnabled(true);
            userRepository.save(user);
        }

        return ResponseEntity.ok(AuthResponse.authenticated(jwtUtil.generateToken(user.getUsername())));
    }

    public record LoginRequest(
            @NotBlank String username,
            @NotBlank String password
    ) {
    }

    public record TotpVerificationRequest(
            @NotBlank String preAuthToken,
            @NotBlank
            @Pattern(regexp = "\\d{6}", message = "TOTP code must be 6 digits")
            String code
    ) {
    }

    public record AuthResponse(
            String status,
            String token,
            String preAuthToken,
            String secret,
            String qrCode
    ) {
        static AuthResponse authenticated(String token) {
            return new AuthResponse("AUTHENTICATED", token, null, null, null);
        }

        static AuthResponse totpRequired(String preAuthToken) {
            return new AuthResponse("TOTP_REQUIRED", null, preAuthToken, null, null);
        }

        static AuthResponse totpSetupRequired(String preAuthToken, String secret, String qrCode) {
            return new AuthResponse("TOTP_SETUP_REQUIRED", null, preAuthToken, secret, qrCode);
        }
    }
}
