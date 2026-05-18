import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { DatePipe } from '@angular/common';
import { AuthService } from '../../services/shared/auth.service';

@Component({
  selector: 'app-user-profile',
  imports: [DatePipe],
  templateUrl: './user-profile.html',
  styleUrl: './user-profile.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class UserProfile {
  private readonly authService = inject(AuthService);

  readonly user = this.authService.currentUser;
  readonly initials = computed(() => {
    const name = this.authService.currentUser()?.fullName ?? '';
    return name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2);
  });
}
