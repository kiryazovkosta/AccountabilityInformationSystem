import { inject, Injectable } from '@angular/core';
import { httpResource } from '@angular/common/http';
import { apiUrl, Endpoints } from '../../../../core/http/endpoints-config';
import { AuthService } from '../../../../core/auth/auth.service';
import { UserListResponse } from './user-list.response';

@Injectable()
export class UsersService {
  private readonly authService = inject(AuthService);

  readonly users = httpResource<UserListResponse[]>(() =>
    this.authService.isAdministrator()
      ? { url: apiUrl(Endpoints.adminUsers) }
      : undefined
  );
}
