package uk.co.eduardteodor.backend_java.auth;

import com.google.zxing.BarcodeFormat;
import com.google.zxing.client.j2se.MatrixToImageWriter;
import com.google.zxing.common.BitMatrix;
import com.google.zxing.qrcode.QRCodeWriter;
import dev.samstevens.totp.code.*;
import dev.samstevens.totp.secret.DefaultSecretGenerator;
import dev.samstevens.totp.secret.SecretGenerator;
import dev.samstevens.totp.time.SystemTimeProvider;
import dev.samstevens.totp.time.TimeProvider;
import org.springframework.stereotype.Service;

import java.io.ByteArrayOutputStream;
import java.util.Base64;

@Service
public class MfaService {

    private final SecretGenerator secretGenerator = new DefaultSecretGenerator();

    /**
     * Generates a new random TOTP secret key.
     * Store this in the users table under mfa_secret.
     */
    public String generateSecret() {
        return secretGenerator.generate();
    }

    /**
     * Generates a QR code as a Base64 PNG string.
     * Send this to the frontend to display for scanning into Google Authenticator.
     *
     * @param secret   the TOTP secret stored for the user
     * @param username your username (shown in Google Authenticator)
     * @param issuer   the app name shown in Google Authenticator
     */
    public String generateQrCodeBase64(String secret, String username, String issuer) {
        try {
            String otpAuthUrl = String.format(
                    "otpauth://totp/%s:%s?secret=%s&issuer=%s&algorithm=SHA1&digits=6&period=30",
                    issuer, username, secret, issuer
            );

            QRCodeWriter qrCodeWriter = new QRCodeWriter();
            BitMatrix bitMatrix = qrCodeWriter.encode(otpAuthUrl, BarcodeFormat.QR_CODE, 200, 200);

            ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
            MatrixToImageWriter.writeToStream(bitMatrix, "PNG", outputStream);

            return Base64.getEncoder().encodeToString(outputStream.toByteArray());

        } catch (Exception e) {
            throw new RuntimeException("Failed to generate QR code", e);
        }
    }

    /**
     * Verifies a 6-digit TOTP code against the stored secret.
     * Handles time drift automatically — allows ±1 window (30 seconds either side).
     *
     * @param secret the stored mfa_secret for the user
     * @param code   the 6-digit code entered by the user
     * @return true if valid, false if not
     */
    public boolean verifyCode(String secret, String code) {
        TimeProvider timeProvider = new SystemTimeProvider();
        CodeGenerator codeGenerator = new DefaultCodeGenerator();
        CodeVerifier verifier = new DefaultCodeVerifier(codeGenerator, timeProvider);
        return verifier.isValidCode(secret, code);
    }
}