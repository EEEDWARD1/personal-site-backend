export const ADMIN_USER_ID = "user_3KvGpJwyF1axbnsKPdCYemBhkh6";

export function isAdminUser(userId: string | null | undefined): boolean {
  return userId === ADMIN_USER_ID;
}
