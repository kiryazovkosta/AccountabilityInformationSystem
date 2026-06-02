import { inject, Injectable } from '@angular/core';
import { httpResource } from '@angular/common/http';
import { environment } from '../../../../../environments/environment';
import { AuthService } from '../../../../services/shared/auth.service';
import { UserListResponse } from './user-list.response';

@Injectable()
export class UsersService {
  private readonly authService = inject(AuthService);

  readonly users = httpResource<UserListResponse[]>(() =>
    this.authService.isAdministrator()
      ? { url: `${environment.apiBaseUrl}api/admin/users`, withCredentials: true }
      : undefined
  );
}
