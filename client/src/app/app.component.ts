import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from './shared/layout/header/header.component';
import { FooterComponent } from './shared/layout/footer/footer.component';
import { WhatsappButtonComponent } from './shared/components/whatsapp-button/whatsapp-button.component';
import { TranslateService } from '@ngx-translate/core';
import { SignalRService } from './core/services/signalr.service';
import { AuthService } from './core/services/auth.service';
import { SeoService } from './core/services/seo.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, HeaderComponent, FooterComponent, WhatsappButtonComponent],
  template: `
    <app-header />
    <main class="main-content">
      <router-outlet />
    </main>
    <app-footer />
    <app-whatsapp-button />

    <!-- Real-time Toast Notifications -->
    @if (signalR.toasts().length > 0) {
      <div class="toast-container">
        @for (toast of signalR.toasts(); track toast.id) {
          <div class="toast" [class]="'toast-' + toast.type" (click)="signalR.dismissToast(toast.id)">
            <span class="toast-icon">{{ toast.icon }}</span>
            <span class="toast-msg">{{ toast.message }}</span>
            <button class="toast-close">✕</button>
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
      inset-inline-end: 1.5rem;
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
      border-radius: 14px;
      background: white;
      box-shadow: 0 8px 30px rgba(0,0,0,0.12);
      border-inline-start: 4px solid #6366f1;
      cursor: pointer;
      animation: slideIn 0.3s ease;
      font-size: 0.9rem;
      font-weight: 500;
    }

    .toast-success { border-color: #10b981; }
    .toast-warning { border-color: #f59e0b; }
    .toast-info { border-color: #6366f1; }

    .toast-icon { font-size: 1.2rem; flex-shrink: 0; }
    .toast-msg { flex: 1; color: #1f2937; }
    .toast-close {
      background: none; border: none; color: #9ca3af;
      cursor: pointer; font-size: 0.85rem; padding: 0;
    }

    @keyframes slideIn {
      from { opacity: 0; transform: translateX(-30px); }
      to { opacity: 1; transform: translateX(0); }
    }
  `]
})
export class AppComponent implements OnInit {
  constructor(
    private translate: TranslateService,
    public signalR: SignalRService,
    private auth: AuthService,
    private seo: SeoService
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

    // Start SignalR for authenticated users
    if (this.auth.isLoggedIn()) {
      const user = this.auth.currentUser();
      this.signalR.start(user?.gradeId);
    }
  }
}
