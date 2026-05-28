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
  readonly userName = computed(() => this.authService.currentUser()?.fullName ?? null);
  readonly isMenuOpen = signal(false);
  readonly isSubmenuOpen = signal(false);

  toggleMenu(): void {
    const opening = !this.isMenuOpen();
    this.isMenuOpen.set(opening);
    if (!opening) {
      this.isSubmenuOpen.set(false);
    }
  }

  closeMenu(): void {
    this.isMenuOpen.set(false);
    this.isSubmenuOpen.set(false);
  }

  toggleSubmenu(event: Event): void {
    event.preventDefault();
    this.isSubmenuOpen.update(v => !v);
  }

  closeSubmenu(event: Event): void {
    event.preventDefault();
    event.stopPropagation();
    this.isSubmenuOpen.set(false);
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (window.innerWidth > 700) {
      if (!(event.target as HTMLElement).closest('.has-submenu')) {
        this.isSubmenuOpen.set(false);
      }
    }
  }
}
