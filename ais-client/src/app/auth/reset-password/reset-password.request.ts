export interface ResetPasswordRequest {
  userId: string;
  code: string;
  newPassword: string;
  confirmPassword: string;
}

export interface ResetPasswordFormData {
  newPassword: string;
  confirmPassword: string;
}
