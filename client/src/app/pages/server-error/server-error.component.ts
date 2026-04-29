import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-server-error',
  standalone: true,
  imports: [RouterLink],
  template: `
    <section class="error-page">
      <div class="error-content animate-fade-in-up">
        <span class="error-emoji">🔧</span>
        <h1>حدث خطأ ما!</h1>
        <p>نعمل على إصلاحه بسرعة 🛠️</p>
        <p class="error-sub">لا تقلق — بياناتك في أمان</p>
        <div class="error-actions">
          <button (click)="retry()" class="btn btn-primary">حاول مجدداً</button>
          <a routerLink="/" class="btn btn-secondary">العودة للرئيسية</a>
        </div>
      </div>
    </section>
  `,
  styles: [`
    .error-page {
      min-height: 70vh; display: flex; align-items: center; justify-content: center;
      padding: 2rem; text-align: center;
    }
    .error-content {
      max-width: 420px;
    }
    .error-emoji {
      font-size: 5rem; display: block; margin-bottom: 1rem;
      animation: wrenchSwing 2s ease-in-out infinite;
    }
    h1 { font-size: 1.75rem; font-weight: 900; color: #1f2937; margin-bottom: .5rem; }
    p { color: #6b7280; font-size: 1.1rem; }
    .error-sub { font-size: .9rem; margin-top: .25rem; margin-bottom: 2rem; }
    .error-actions {
      display: flex; gap: .75rem; justify-content: center; flex-wrap: wrap;
      .btn { min-height: 48px; padding: .75rem 1.5rem; border-radius: 12px; font-weight: 700; }
    }
    @keyframes wrenchSwing {
      0%, 100% { transform: rotate(0deg); }
      25% { transform: rotate(15deg); }
      75% { transform: rotate(-15deg); }
    }
  `]
})
export class ServerErrorComponent {
  retry() {
    window.location.reload();
  }
}
