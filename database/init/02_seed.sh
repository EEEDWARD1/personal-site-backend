#!/usr/bin/env bash
set -e

echo "ADMIN_PASSWORD_HASH is: $ADMIN_PASSWORD_HASH"

HASH="$ADMIN_PASSWORD_HASH"

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
    INSERT INTO users (email, password_hash)
    VALUES ('eduardteodor2004@gmail.com', '\$2b\$10\$bzKFKtP8DHIQOV3XLmDnw.cacqu8wh7HeifNOvWshHPwgChRRLjx6');
EOSQL