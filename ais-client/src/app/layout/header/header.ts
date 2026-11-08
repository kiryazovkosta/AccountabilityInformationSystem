import { ChangeDetectionStrategy, Component, computed, HostListener, inject, signal } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { APP_ROUTES } from '../../core/app-routes';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-header',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './header.html',
  styleUrl: './header.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Header {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  readonly isLoggedIn = this.authService.isLoggedIn;
  readonly isAdministrator = this.authService.isAdministrator;
  readonly userName = computed(() => this.authService.currentUser()?.fullName ?? null);
  readonly isMenuOpen = signal(false);
  readonly openSubmenuId = signal<string | null>(null);

  toggleMenu(): void {
    const opening = !this.isMenuOpen();
    this.isMenuOpen.set(opening);
    if (!opening) {
      this.openSubmenuId.set(null);
    }
  }

  logout(): void {
    this.closeMenu();
    // AuthService.logout() never errors and clears the local session either way.
    this.authService.logout().subscribe(() => this.router.navigate([APP_ROUTES.HOME]));
  }

  closeMenu(): void {
    this.isMenuOpen.set(false);
    this.openSubmenuId.set(null);
  }

  toggleSubmenu(id: string, event: Event): void {
    event.preventDefault();
    this.openSubmenuId.update(current => (current === id ? null : id));
  }

  closeSubmenu(event: Event): void {
    event.preventDefault();
    event.stopPropagation();
    this.openSubmenuId.set(null);
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (!(event.target as HTMLElement).closest('.has-submenu')) {
      this.openSubmenuId.set(null);
    }
  }
}
