import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-not-found',
  standalone: true,
  imports: [RouterLink, TranslateModule],
  template: `
    <div class="not-found">
      <div class="not-found-content">
        <div class="not-found-icon">🦉</div>
        <h1>404</h1>
        <h2>عذراً، الصفحة التي تبحث عنها ليست هنا!</h2>
        <p>يبدو أنك وصلت لصفحة غير موجودة. لا تقلق، يمكنك العودة للصفحة الرئيسية.</p>
        <a routerLink="/" class="btn btn-primary btn-lg">
          <i class="fas fa-home"></i>
          العودة للرئيسية
        </a>
      </div>
    </div>
  `,
  styles: [`
    .not-found {
      min-height: 70vh;
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 2rem;
    }

    .not-found-content {
      text-align: center;
      max-width: 480px;
    }

    .not-found-icon {
      font-size: 100px;
      margin-bottom: 1rem;
      animation: float 3s ease-in-out infinite;
    }

    h1 {
      font-size: 6rem;
      font-weight: 900;
      background: linear-gradient(135deg, var(--primary), var(--accent));
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
      background-clip: text;
      line-height: 1;
      margin-bottom: 1rem;
    }

    h2 {
      font-size: 1.4rem;
      font-weight: 700;
      color: var(--gray-800);
      margin-bottom: 0.75rem;
    }

    p {
      color: var(--gray-500);
      margin-bottom: 2rem;
      font-size: 1rem;
    }

    @keyframes float {
      0%, 100% { transform: translateY(0) rotate(0deg); }
      25% { transform: translateY(-10px) rotate(-5deg); }
      75% { transform: translateY(-5px) rotate(5deg); }
    }

    @media (max-width: 480px) {
      h1 { font-size: 4rem; }
      h2 { font-size: 1.1rem; }
      .not-found-icon { font-size: 72px; }
    }
  `]
})
export class NotFoundComponent {}
