import { AfterViewInit, Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-header',
  imports: [RouterLink, RouterLinkActive, CommonModule],
  templateUrl: './header.html',
  styleUrl: './header.css'
})
export class Header implements AfterViewInit {
  // Authentication state properties
  isLoggedIn = false;
  userName = '';

  ngAfterViewInit() {
    this.initMobileMenu();
  }

  // Authentication methods
  login(event: Event) {
    event.preventDefault();
    // TODO: Implement actual login logic here
    console.log('Login clicked');
    // For demo purposes, simulate login
    this.isLoggedIn = true;
    this.userName = 'John Doe';
  }

  logout(event: Event) {
    event.preventDefault();
    // TODO: Implement actual logout logic here
    console.log('Logout clicked');
    // For demo purposes, simulate logout
    this.isLoggedIn = false;
    this.userName = '';
  }

  closeSubmenu(event: Event) {
    event.preventDefault();
    event.stopPropagation();
    // Hide the submenu panel
    const submenuPanel = (event.target as HTMLElement).closest('.submenu-panel') as HTMLElement;
    if (submenuPanel) {
      submenuPanel.style.display = 'none';
      // Reset hover state
      setTimeout(() => {
        submenuPanel.style.display = '';
      }, 100);
    }
  }

  private initMobileMenu() {
    const hamburger = document.querySelector('.hamburger') as HTMLElement;
    const navMenu = document.querySelector('.nav-menu') as HTMLElement;

    if (hamburger && navMenu) {
      hamburger.addEventListener('click', () => {
        navMenu.classList.toggle('active');
        // Close any open submenus when closing nav
        if (!navMenu.classList.contains('active')) {
          document.querySelectorAll('.has-submenu.show-submenu').forEach(sub => sub.classList.remove('show-submenu'));
        }
      });

      // Close menu when clicking a link (except submenu toggles)
      const navLinks = document.querySelectorAll('.nav-menu a:not(.profile-btn)');
      navLinks.forEach(link => {
        link.addEventListener('click', () => {
          navMenu.classList.remove('active');
          // Also close submenus
          document.querySelectorAll('.has-submenu.show-submenu').forEach(sub => sub.classList.remove('show-submenu'));
        });
      });

      // Submenu toggle for mobile
      const submenuToggles = document.querySelectorAll('.profile-btn');
      submenuToggles.forEach(toggle => {
        toggle.addEventListener('click', (e) => {
          if (window.innerWidth <= 700 && navMenu.classList.contains('active')) {
            e.preventDefault();
            const parent = toggle.closest('.has-submenu') as HTMLElement;
            if (parent) {
              parent.classList.toggle('show-submenu');
            }
          }
        });
      });

      // Close submenu when clicking outside (desktop only)
      document.addEventListener('click', (e) => {
        if (window.innerWidth > 700) {
          const target = e.target as HTMLElement;
          const submenu = target.closest('.has-submenu');
          if (!submenu) {
            // Hide all submenu panels
            const panels = document.querySelectorAll('.submenu-panel') as NodeListOf<HTMLElement>;
            panels.forEach(panel => {
              panel.style.display = 'none';
              setTimeout(() => {
                panel.style.display = '';
              }, 100);
            });
          }
        }
      });
    }
  }
}
