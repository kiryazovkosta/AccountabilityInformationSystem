import { ChangeDetectionStrategy, Component, computed, HostListener, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../services/shared/auth.service';

@Component({
  selector: 'app-header',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './header.html',
  styleUrl: './header.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Header {
  private readonly authService = inject(AuthService);

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
