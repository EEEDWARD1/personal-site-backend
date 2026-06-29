package uk.co.eduardteodor.backend_java.auth;

public record MfaLoginRequest(String preAuthToken, String code) {}
