export interface UserListResponse {
  id: string;
  username: string;
  identityId?: string;
  isConfirmed: boolean;
  isLocked: boolean;
}
