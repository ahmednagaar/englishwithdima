import { Component, signal } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, TranslateModule],
  template: `
    <header class="header" [class.scrolled]="isScrolled()">
      <div class="container">
        <a routerLink="/" class="logo">
          <span class="logo-icon">📚</span>
          <span class="logo-text">{{ 'APP_NAME' | translate }}</span>
        </a>

        <nav class="nav-desktop">
          <a routerLink="/" routerLinkActive="active" [routerLinkActiveOptions]="{exact:true}">{{ 'NAV.HOME' | translate }}</a>
          <a routerLink="/grades">{{ 'NAV.TESTS' | translate }}</a>
          <a routerLink="/games">{{ 'NAV.GAMES' | translate }}</a>
          <a routerLink="/contact">{{ 'NAV.CONTACT' | translate }}</a>
          <a routerLink="/booking" class="btn-booking">{{ 'NAV.BOOKING' | translate }}</a>
        </nav>

        <div class="nav-actions">

          @if (auth.isLoggedIn()) {
            <div class="user-menu" (click)="userMenuOpen.set(!userMenuOpen())">
              <div class="avatar">{{ getInitial() }}</div>
              <span class="user-name">{{ auth.currentUser()?.firstName }}</span>
              @if (userMenuOpen()) {
                <div class="dropdown" (click)="$event.stopPropagation()">
                  @if (auth.isTeacher()) {
                    <a routerLink="/admin" (click)="userMenuOpen.set(false)">{{ 'NAV.ADMIN' | translate }}</a>
                  }
                  <a routerLink="/profile" (click)="userMenuOpen.set(false)">{{ 'NAV.PROFILE' | translate }}</a>
                  <a (click)="auth.logout(); userMenuOpen.set(false)" class="logout">{{ 'NAV.LOGOUT' | translate }}</a>
                </div>
              }
            </div>
          } @else {
            <a routerLink="/auth/login" class="btn-login">{{ 'NAV.LOGIN' | translate }}</a>
          }

          <button class="mobile-toggle" (click)="mobileOpen.set(!mobileOpen())">
            <span></span><span></span><span></span>
          </button>
        </div>
      </div>

      @if (mobileOpen()) {
        <nav class="nav-mobile" (click)="mobileOpen.set(false)">
          <a routerLink="/">{{ 'NAV.HOME' | translate }}</a>
          <a routerLink="/grades">{{ 'NAV.TESTS' | translate }}</a>
          <a routerLink="/games">{{ 'NAV.GAMES' | translate }}</a>
          <a routerLink="/contact">{{ 'NAV.CONTACT' | translate }}</a>
          <a routerLink="/booking">{{ 'NAV.BOOKING' | translate }}</a>
          @if (!auth.isLoggedIn()) {
            <a routerLink="/auth/login">{{ 'NAV.LOGIN' | translate }}</a>
            <a routerLink="/auth/register">{{ 'NAV.REGISTER' | translate }}</a>
          }
        </nav>
      }
    </header>
  `,
  styleUrl: './header.component.scss'
})
export class HeaderComponent {
  isScrolled = signal(false);
  mobileOpen = signal(false);
  userMenuOpen = signal(false);
  constructor(public auth: AuthService, private translate: TranslateService) {
    if (typeof window !== 'undefined') {
      window.addEventListener('scroll', () => this.isScrolled.set(window.scrollY > 50));
    }
  }

  getInitial(): string {
    return this.auth.currentUser()?.firstName?.charAt(0) || '?';
  }
}
