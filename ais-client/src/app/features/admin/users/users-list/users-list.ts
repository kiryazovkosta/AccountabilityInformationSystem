import { Component, inject } from '@angular/core';
import { UsersService } from './users.service';

@Component({
  selector: 'app-users-list',
  imports: [],
  templateUrl: './users-list.html',
  styleUrl: './users-list.css',
  providers: [UsersService],
})
export class UsersList {
  protected readonly usersService = inject(UsersService);
}
