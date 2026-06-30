package uk.co.eduardteodor.api;

import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;

public class BcryptRunner {
    public static void main(String[] args) {
        System.out.println(new BCryptPasswordEncoder().encode("10Key.sep."));
    }
}