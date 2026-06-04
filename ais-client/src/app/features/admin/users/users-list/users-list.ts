import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { UsersService } from './users.service';
import { APP_ROUTES } from '../../../../common/app-routes';

@Component({
  selector: 'app-users-list',
  imports: [RouterLink],
  templateUrl: './users-list.html',
  styleUrl: './users-list.css',
  providers: [UsersService],
})
export class UsersList {
  protected readonly usersService = inject(UsersService);
  protected readonly routes = APP_ROUTES;
}
