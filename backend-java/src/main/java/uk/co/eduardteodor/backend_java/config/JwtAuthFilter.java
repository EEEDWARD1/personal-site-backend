package uk.co.eduardteodor.backend_java.config;

import jakarta.servlet.FilterChain;
import jakarta.servlet.ServletException;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.security.core.userdetails.UserDetailsService;
import org.springframework.security.web.authentication.WebAuthenticationDetailsSource;
import org.springframework.stereotype.Component;
import org.springframework.web.filter.OncePerRequestFilter;

import java.io.IOException;

/**
 * JWT Authentication Filter - runs once on every incoming HTTP request.
 * Intercepts requests to check for a valid JWT token in the Authorization header.
 * If valid, authenticates the user in the Spring Security context so protected
 * routes can be accessed. Extends OncePerRequestFilter to guarantee single execution
 * per request.
 */
@Component
public class JwtAuthFilter extends OncePerRequestFilter {

    private final JwtUtil jwtUtil;
    private final UserDetailsService userDetailsService;

    public JwtAuthFilter(JwtUtil jwtUtil, UserDetailsService userDetailsService) {
        this.jwtUtil = jwtUtil;
        this.userDetailsService = userDetailsService;
    }

    @Override
    protected void doFilterInternal(HttpServletRequest request,
                                    HttpServletResponse response,
                                    FilterChain filterChain)
            throws ServletException, IOException {

        // Step 1: Read the Authorization header from the incoming request
        // Expected format: "Bearer eyJ..."
        String authHeader = request.getHeader("Authorization");

        System.out.println("=== JWT FILTER ===");
        System.out.println("Method: " + request.getMethod());
        System.out.println("URI: " + request.getRequestURI());
        System.out.println("Auth Header: " + authHeader);


        // Step 2: If no header or wrong format, skip auth and continue as unauthenticated
        // Public routes will still work, protected routes will be blocked by SecurityConfig
        if (authHeader == null || !authHeader.startsWith("Bearer ")) {
            filterChain.doFilter(request, response);
            return;
        }

        // Step 3: Strip "Bearer " prefix to get the raw token string
        String token = authHeader.substring(7);
        System.out.println("Token valid: " + jwtUtil.isTokenValid(token));

        // Step 4: Ask JwtUtil to validate the token — checks signature and expiry
        // If invalid or expired, let request through unauthenticated
        if (!jwtUtil.isTokenValid(token)) {
            filterChain.doFilter(request, response);
            return;
        }

        // Step 5: Token is valid, extract the username embedded inside it
        String username = jwtUtil.extractUsername(token);
        // Step 6: Only authenticate if username exists and request isn't already authenticated
        // Prevents double authentication on the same request
        if (username != null && SecurityContextHolder.getContext().getAuthentication() == null) {

            // Step 7: Load the full user details from the database using the extracted username
            UserDetails userDetails = userDetailsService.loadUserByUsername(username);

            // Step 8: Create a Spring Security authentication token with the user's details
            // Second parameter (credentials) is null, we don't need the password after token validation
            UsernamePasswordAuthenticationToken authToken =
                    new UsernamePasswordAuthenticationToken(
                            userDetails,
                            null,
                            userDetails.getAuthorities()
                    );


            // Step 9: Attach request details (IP address, session) to the auth token
            authToken.setDetails(new WebAuthenticationDetailsSource().buildDetails(request));

            // Step 10: Register the authenticated user in the Spring Security context
            // This is what makes Spring Security treat this request as authenticated
            SecurityContextHolder.getContext().setAuthentication(authToken);
        }
        // Step 11: Pass the request on to the next filter or controller
        filterChain.doFilter(request, response);
    }
}