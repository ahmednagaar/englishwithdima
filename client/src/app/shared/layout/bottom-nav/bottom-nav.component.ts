import { Component, signal, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive, Router, NavigationEnd } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { filter, Subscription } from 'rxjs';

@Component({
  selector: 'app-bottom-nav',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, TranslateModule],
  template: `
    @if (!hidden()) {
      <nav class="bottom-nav" [class.hide]="isTestPage()">
        <a routerLink="/" routerLinkActive="active" [routerLinkActiveOptions]="{exact: true}" class="nav-item">
          <i class="fas fa-home"></i>
          <span>الرئيسية</span>
        </a>
        <a routerLink="/tests" routerLinkActive="active" class="nav-item">
          <i class="fas fa-file-alt"></i>
          <span>الاختبارات</span>
        </a>
        <a routerLink="/games" routerLinkActive="active" class="nav-item">
          <i class="fas fa-gamepad"></i>
          <span>الألعاب</span>
        </a>
        <a routerLink="/leaderboard" routerLinkActive="active" class="nav-item">
          <i class="fas fa-trophy"></i>
          <span>المتصدرين</span>
        </a>
        <a routerLink="/contact" routerLinkActive="active" class="nav-item">
          <i class="fas fa-envelope"></i>
          <span>تواصل</span>
        </a>
      </nav>
    }
  `,
  styles: [`
    .bottom-nav {
      position: fixed;
      bottom: 0;
      inset-inline-start: 0;
      inset-inline-end: 0;
      height: 64px;
      background: white;
      border-top: 1px solid var(--gray-200);
      display: none;
      align-items: center;
      justify-content: space-around;
      z-index: 998;
      padding-bottom: env(safe-area-inset-bottom, 0);
      box-shadow: 0 -2px 10px rgba(0,0,0,0.05);

      @media (max-width: 768px) {
        display: flex;
      }
    }

    .bottom-nav.hide { display: none !important; }

    .nav-item {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      gap: 2px;
      padding: 6px 0;
      min-width: 56px;
      min-height: 48px;
      color: var(--gray-400);
      font-size: 0.65rem;
      font-weight: 600;
      text-decoration: none;
      transition: color 0.2s;

      i {
        font-size: 1.2rem;
        transition: transform 0.2s;
      }

      &.active {
        color: var(--primary);
        i { transform: scale(1.15); }
      }
    }
  `]
})
export class BottomNavComponent implements OnInit, OnDestroy {
  hidden = signal(false);
  isTestPage = signal(false);
  private sub?: Subscription;

  constructor(private router: Router) {}

  ngOnInit() {
    this.sub = this.router.events.pipe(
      filter(e => e instanceof NavigationEnd)
    ).subscribe((e: any) => {
      // Hide during test-taking and admin pages
      const url = e.urlAfterRedirects || e.url;
      this.isTestPage.set(
        url.includes('/take') || url.includes('/admin')
      );
    });

    // Hide when keyboard is open on mobile
    if (typeof window !== 'undefined' && window.visualViewport) {
      window.visualViewport.addEventListener('resize', () => {
        const keyboardOpen = window.visualViewport!.height < window.innerHeight * 0.75;
        this.hidden.set(keyboardOpen);
      });
    }
  }

  ngOnDestroy() {
    this.sub?.unsubscribe();
  }
}
