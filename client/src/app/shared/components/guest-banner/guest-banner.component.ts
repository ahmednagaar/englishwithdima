import { Component, computed, signal, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, Router, NavigationEnd } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { filter, Subscription } from 'rxjs';

@Component({
  selector: 'app-guest-banner',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    @if (showBanner()) {
      <div class="guest-banner" role="banner">
        <span class="banner-icon">⭐</span>
        <span class="banner-text">سجّل حسابك لحفظ نتائجك والمشاركة في المسابقات</span>
        <a routerLink="/auth/register" class="banner-cta">سجّل الآن</a>
        <button (click)="dismiss()" class="banner-close" aria-label="إغلاق">✕</button>
      </div>
    }
  `,
  styles: [`
    .guest-banner {
      display: flex; align-items: center; gap: 10px;
      padding: 10px 16px; background: linear-gradient(90deg, #fef3c7, #fde68a);
      border-bottom: 1px solid #f59e0b; font-size: 14px; color: #92400e;
      position: sticky; top: 70px; z-index: 45;
      @media(max-width: 600px) { flex-wrap: wrap; gap: 6px; }
    }
    .banner-icon { font-size: 18px; flex-shrink: 0; }
    .banner-text { flex: 1; font-weight: 500; }
    .banner-cta {
      padding: 5px 14px; border-radius: 20px; font-weight: 700; font-size: 13px;
      background: #f59e0b; color: white; white-space: nowrap; min-height: 32px;
      display: inline-flex; align-items: center; transition: background .2s;
      &:hover { background: #d97706; }
    }
    .banner-close {
      width: 28px; height: 28px; border-radius: 50%; display: flex;
      align-items: center; justify-content: center; font-size: 14px;
      color: #92400e; flex-shrink: 0; transition: background .2s;
      &:hover { background: rgba(0,0,0,0.08); }
    }
  `]
})
export class GuestBannerComponent implements OnInit, OnDestroy {
  private dismissCount = signal(+(sessionStorage.getItem('guest_banner_dismissed') || '0'));
  private currentUrl = signal('');
  private routeSub?: Subscription;

  constructor(private auth: AuthService, private router: Router) {}

  ngOnInit() {
    // Set initial URL
    this.currentUrl.set(this.router.url);
    // Reactively track route changes
    this.routeSub = this.router.events.pipe(
      filter(e => e instanceof NavigationEnd)
    ).subscribe((e: any) => {
      this.currentUrl.set(e.urlAfterRedirects || e.url);
    });
  }

  ngOnDestroy() {
    this.routeSub?.unsubscribe();
  }

  showBanner = computed(() => {
    const isGuest = !this.auth.getToken() && !!localStorage.getItem('ewd_guest');
    const notInTest = !this.currentUrl().includes('/take');
    return isGuest && notInTest && this.dismissCount() < 3;
  });

  dismiss() {
    this.dismissCount.update(c => c + 1);
    sessionStorage.setItem('guest_banner_dismissed', String(this.dismissCount()));
  }
}
