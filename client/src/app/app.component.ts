import { Component, OnInit, OnDestroy, signal, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, Router, NavigationEnd } from '@angular/router';
import { HeaderComponent } from './shared/layout/header/header.component';
import { FooterComponent } from './shared/layout/footer/footer.component';
import { BottomNavComponent } from './shared/layout/bottom-nav/bottom-nav.component';
import { WhatsappButtonComponent } from './shared/components/whatsapp-button/whatsapp-button.component';
import { TranslateService } from '@ngx-translate/core';
import { SignalRService } from './core/services/signalr.service';
import { AuthService } from './core/services/auth.service';
import { SeoService } from './core/services/seo.service';
import { filter, Subscription } from 'rxjs';
import { trigger, transition, style, animate } from '@angular/animations';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, HeaderComponent, FooterComponent, BottomNavComponent, WhatsappButtonComponent],
  animations: [
    trigger('routeAnim', [
      transition('* <=> *', [
        style({ opacity: 0, transform: 'translateY(8px)' }),
        animate('200ms ease-out', style({ opacity: 1, transform: 'translateY(0)' }))
      ])
    ])
  ],
  template: `
    <!-- Offline Banner -->
    @if (!isOnline()) {
      <div class="offline-banner">
        📡 لا يوجد اتصال بالإنترنت — إجاباتك محفوظة محلياً
      </div>
    }

    <app-header />
    <main class="main-content" [@routeAnim]="currentRoute()">
      <router-outlet />
    </main>
    <app-footer />
    <app-bottom-nav />

    <!-- WhatsApp FAB (hidden during tests) -->
    @if (!isTestRoute()) {
      <app-whatsapp-button />
    }

    <!-- Real-time Toast Notifications -->
    @if (signalR.toasts().length > 0) {
      <div class="toast-container">
        @for (toast of signalR.toasts(); track toast.id) {
          <div class="toast" [class]="'toast-' + toast.type" (click)="signalR.dismissToast(toast.id)">
            <span class="toast-icon">{{ toast.icon }}</span>
            <span class="toast-msg">{{ toast.message }}</span>
            <button class="toast-close" aria-label="إغلاق">✕</button>
          </div>
        }
      </div>
    }
  `,
  styles: [`
    .main-content {
      min-height: calc(100vh - 140px);
    }

    .toast-container {
      position: fixed;
      top: 80px;
      inset-inline-start: 1.5rem;
      z-index: 10000;
      display: flex;
      flex-direction: column;
      gap: 0.5rem;
      max-width: 400px;
    }

    .toast {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      padding: 0.75rem 1rem;
      min-height: 48px;
      border-radius: 14px;
      background: white;
      box-shadow: 0 8px 30px rgba(0,0,0,0.12);
      border-inline-start: 4px solid #6366f1;
      cursor: pointer;
      animation: toastSlideIn 0.3s ease;
      font-size: 0.9rem;
      font-weight: 500;
    }

    .toast-success { border-color: #10b981; }
    .toast-warning { border-color: #f59e0b; }
    .toast-danger  { border-color: #ef4444; }
    .toast-info    { border-color: #6366f1; }

    .toast-icon { font-size: 1.2rem; flex-shrink: 0; }
    .toast-msg { flex: 1; color: #1f2937; }
    .toast-close {
      background: none; border: none; color: #9ca3af;
      cursor: pointer; font-size: 0.85rem; padding: 4px;
      min-height: auto;
    }

    @keyframes toastSlideIn {
      from { opacity: 0; transform: translateX(30px); }
      to { opacity: 1; transform: translateX(0); }
    }

    @media (max-width: 768px) {
      .toast-container {
        inset-inline-start: 0.75rem;
        inset-inline-end: 0.75rem;
        max-width: none;
      }

      .main-content {
        min-height: calc(100vh - 200px);
      }
    }
  `]
})
export class AppComponent implements OnInit, OnDestroy {
  isOnline = signal(true);
  currentRoute = signal('');
  isTestRoute = signal(false);
  private routeSub?: Subscription;

  constructor(
    private translate: TranslateService,
    public signalR: SignalRService,
    private auth: AuthService,
    private seo: SeoService,
    private router: Router
  ) {
    // Always start with Arabic
    this.translate.setDefaultLang('ar');
    this.translate.use('ar');
    localStorage.setItem('ewd_lang', 'ar');
    document.documentElement.lang = 'ar';
    document.documentElement.dir = 'rtl';
  }

  ngOnInit() {
    // Set default SEO
    this.seo.resetToDefault();

    // Set grade on body for adaptive font sizing
    this.setGradeAttribute();

    // Start SignalR for authenticated users
    if (this.auth.isLoggedIn()) {
      const user = this.auth.currentUser();
      this.signalR.start(user?.gradeId);
    }

    // Monitor online/offline
    this.isOnline.set(navigator.onLine);
    window.addEventListener('online', () => this.isOnline.set(true));
    window.addEventListener('offline', () => this.isOnline.set(false));

    // Track current route for animations and hiding elements
    this.routeSub = this.router.events.pipe(
      filter(e => e instanceof NavigationEnd)
    ).subscribe((e: any) => {
      const url = e.urlAfterRedirects || e.url;
      this.currentRoute.set(url);
      this.isTestRoute.set(url.includes('/take'));

      // Scroll to top on navigation
      window.scrollTo({ top: 0, behavior: 'smooth' });
    });
  }

  ngOnDestroy() {
    this.routeSub?.unsubscribe();
  }

  private setGradeAttribute() {
    const user = this.auth.currentUser();
    if (user?.gradeId) {
      document.body.setAttribute('data-grade', user.gradeId.toString());
    }

    // Also check guest session
    try {
      const guest = JSON.parse(localStorage.getItem('ewd_guest') || '{}');
      if (guest.gradeId) {
        document.body.setAttribute('data-grade', guest.gradeId.toString());
      }
    } catch {}
  }
}
