export interface UserListResponse {
  id: string;
  username: string;
  email: string;
  firstName: string;
  middleName: string;
  lastName: string;
  identityId?: string;
  isConfirmed: boolean;
  isLocked: boolean;
  roles: string[];
}
