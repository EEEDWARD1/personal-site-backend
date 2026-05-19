package uk.co.eduardteodor.backend_java.config;

import io.jsonwebtoken.Jwts;
import io.jsonwebtoken.security.Keys;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Component;

import javax.crypto.SecretKey;
import java.util.Date;

/**
 * JWT Utility — handles all JWT token operations.
 * Responsible for generating signed tokens, extracting claims,
 * and validating token integrity and expiry.
 * Secret and expiration are injected from application.yml.
 */
@Component
public class JwtUtil {

    // Injected from application.yml — used to sign and verify tokens
    @Value("${jwt.secret}")
    private String secret;

    // Injected from application.yml — token lifetime in milliseconds (86400000 = 24 hours)
    @Value("${jwt.expiration}")
    private long expiration;

    /**
     * Converts the plain text secret into a cryptographic key
     * using HMAC-SHA algorithm — required by the JWT library for signing.
     */
    private SecretKey getSigningKey() {
        return Keys.hmacShaKeyFor(secret.getBytes());
    }

    /**
     * Generates a signed JWT token for the given username.
     * Token contains: subject (username), issued at timestamp, expiration timestamp.
     * Signed with the HMAC-SHA secret key to prevent tampering.
     */
    public String generateToken(String username) {
        return Jwts.builder()
                .subject(username)
                .issuedAt(new Date())
                .expiration(new Date(System.currentTimeMillis() + expiration))
                .signWith(getSigningKey())
                .compact();
    }

    /**
     * Extracts the username (subject) embedded inside a JWT token.
     * Verifies the token signature before reading, tampered tokens will throw.
     */
    public String extractUsername(String token) {
        return Jwts.parser()
                .verifyWith(getSigningKey())
                .build()
                .parseSignedClaims(token)
                .getPayload()
                .getSubject();
    }

    /**
     * Validates a JWT token by attempting to parse and verify its signature.
     * Returns false if the token is expired, malformed, or tampered with.
     * Catches all exceptions silently,  invalid tokens simply return false.
     */
    public boolean isTokenValid(String token) {
        try {
            Jwts.parser()
                    .verifyWith(getSigningKey())
                    .build()
                    .parseSignedClaims(token);
            return true;
        } catch (Exception e) {
            return false;
        }
    }
}