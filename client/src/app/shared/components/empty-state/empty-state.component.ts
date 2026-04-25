import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-empty-state',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="empty-state">
      <div class="empty-icon">{{ icon }}</div>
      <h3>{{ title }}</h3>
      <p>{{ message }}</p>
      @if (actionLabel) {
        <button class="btn btn-primary" (click)="onAction()">
          {{ actionLabel }}
        </button>
      }
    </div>
  `,
  styles: [`
    .empty-state {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 3rem 1.5rem;
      text-align: center;
      min-height: 300px;
    }

    .empty-icon {
      font-size: 72px;
      margin-bottom: 1rem;
      animation: float 3s ease-in-out infinite;
    }

    h3 {
      font-size: 1.3rem;
      font-weight: 700;
      color: var(--gray-800);
      margin-bottom: 0.5rem;
    }

    p {
      color: var(--gray-500);
      font-size: 0.95rem;
      max-width: 360px;
      margin-bottom: 1.5rem;
      line-height: 1.7;
    }

    @keyframes float {
      0%, 100% { transform: translateY(0); }
      50% { transform: translateY(-8px); }
    }
  `]
})
export class EmptyStateComponent {
  @Input() icon = '🦉';
  @Input() title = 'لا توجد بيانات';
  @Input() message = 'لم يتم العثور على محتوى هنا بعد.';
  @Input() actionLabel = '';
  @Input() actionFn: (() => void) | null = null;

  onAction() {
    this.actionFn?.();
  }
}
