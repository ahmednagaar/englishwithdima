import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, ActivatedRoute } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { ApiService } from '../../../core/services/api.service';
import { AttemptResult } from '../../../core/models';

@Component({
  selector: 'app-test-result',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslateModule],
  template: `
    <section class="result-page">
      <div class="container">
        @if (loading()) {
          <div class="loading-spinner"></div>
        } @else if (result()) {
          <div class="result-hero animate-scale-in" [class.passed]="result()!.passed" [class.failed]="!result()!.passed">
            <div class="result-icon">{{ result()!.passed ? '🎉' : '💪' }}</div>
            <h1>{{ result()!.passed ? ('TESTS.PASSED' | translate) : ('TESTS.FAILED' | translate) }}</h1>
            <div class="score-circle">
              <svg viewBox="0 0 120 120">
                <circle cx="60" cy="60" r="54" fill="none" stroke="#e5e7eb" stroke-width="8"/>
                <circle cx="60" cy="60" r="54" fill="none" [attr.stroke]="result()!.passed ? '#10b981' : '#ef4444'" stroke-width="8" stroke-linecap="round" [style.stroke-dasharray]="339.3" [style.stroke-dashoffset]="339.3 - (339.3 * result()!.percentage / 100)" transform="rotate(-90 60 60)"/>
              </svg>
              <span class="score-text">{{ result()!.percentage }}%</span>
            </div>
            <p class="score-detail">{{ result()!.score }} / {{ result()!.maxScore }} {{ 'COMMON.POINTS' | translate }}</p>
          </div>

          <div class="stats-row animate-fade-in-up" style="animation-delay:.2s">
            <div class="stat-box correct"><span class="stat-icon">✅</span><span class="stat-val">{{ result()!.correctAnswers }}</span><span class="stat-lbl">{{ 'TESTS.CORRECT' | translate }}</span></div>
            <div class="stat-box wrong"><span class="stat-icon">❌</span><span class="stat-val">{{ result()!.wrongAnswers }}</span><span class="stat-lbl">{{ 'TESTS.WRONG' | translate }}</span></div>
            <div class="stat-box skipped"><span class="stat-icon">⏭️</span><span class="stat-val">{{ result()!.skippedAnswers }}</span><span class="stat-lbl">{{ 'TESTS.SKIPPED' | translate }}</span></div>
            <div class="stat-box time"><span class="stat-icon">⏱️</span><span class="stat-val">{{ formatTime(result()!.timeSpentSeconds) }}</span><span class="stat-lbl">{{ 'TESTS.TIME' | translate }}</span></div>
          </div>

          @if (result()!.badgesEarned.length) {
            <div class="badges-earned animate-fade-in-up" style="animation-delay:.3s">
              <h3>{{ 'TESTS.BADGES_EARNED' | translate }}</h3>
              <div class="badges-list">
                @for (badge of result()!.badgesEarned; track badge) {
                  <span class="earned-badge">{{ badge }}</span>
                }
              </div>
            </div>
          }

          <!-- Answer Review -->
          <div class="review-section animate-fade-in-up" style="animation-delay:.35s">
            <h2>{{ 'TESTS.REVIEW' | translate }}</h2>
            @for (ans of result()!.answerDetails; track ans.questionId; let i = $index) {
              <div class="review-card" [class.correct]="ans.isCorrect" [class.wrong]="!ans.isCorrect">
                <div class="review-header">
                  <span class="review-num">{{ 'TESTS.QUESTION_SHORT' | translate }}{{ i + 1 }}</span>
                  <span class="review-status">{{ ans.isCorrect ? '✅' : '❌' }}</span>
                  <span class="review-pts">+{{ ans.pointsEarned }} {{ 'TESTS.POINTS_LABEL' | translate }}</span>
                </div>
                <p class="review-q">{{ ans.questionText }}</p>
                <div class="review-answers">
                  @if (ans.selectedAnswer) { <div class="your-answer"><strong>{{ 'TESTS.YOUR_ANSWER' | translate }}</strong> {{ ans.selectedAnswer }}</div> }
                  @if (!ans.isCorrect && ans.correctAnswer) { <div class="correct-answer"><strong>{{ 'TESTS.CORRECT_ANSWER' | translate }}</strong> {{ ans.correctAnswer }}</div> }
                  @if (ans.explanation) { <div class="explanation">💡 {{ ans.explanation }}</div> }
                </div>
              </div>
            }
          </div>

          <div class="result-actions animate-fade-in-up">
            <a [routerLink]="['/tests']" class="btn btn-secondary">{{ 'COMMON.BACK' | translate }}</a>
            <a [routerLink]="['/tests', result()!.testId]" class="btn btn-primary">{{ 'TESTS.TRY_AGAIN_BTN' | translate }}</a>
          </div>
        }
      </div>
    </section>
  `,
  styles: [`
    .result-page { padding:2rem 0 5rem; }
    .result-hero { text-align:center; padding:3rem 2rem; border-radius:24px; margin-bottom:2rem;
      &.passed { background:linear-gradient(135deg,#f0fdf4,#dcfce7); }
      &.failed { background:linear-gradient(135deg,#fef2f2,#fee2e2); }
      .result-icon { font-size:4rem; margin-bottom:.5rem; animation:float 2s ease-in-out infinite; }
      h1 { font-size:2rem; font-weight:900; }
    }
    .score-circle { width:140px; height:140px; margin:1.5rem auto; position:relative;
      svg { width:100%; height:100%; }
      .score-text { position:absolute; inset:0; display:flex; align-items:center; justify-content:center; font-size:2rem; font-weight:900; color:#1f2937; }
    }
    .score-detail { color:#6b7280; font-size:1rem; }
    .stats-row { display:grid; grid-template-columns:repeat(4,1fr); gap:1rem; margin-bottom:2rem; @media(max-width:600px){ grid-template-columns:repeat(2,1fr); } }
    .stat-box { background:white; border-radius:14px; padding:1.25rem; text-align:center; box-shadow:0 2px 10px rgba(0,0,0,.04);
      .stat-icon { font-size:1.5rem; display:block; }
      .stat-val { font-size:1.5rem; font-weight:900; display:block; margin:.25rem 0; }
      .stat-lbl { font-size:.8rem; color:#6b7280; }
      &.correct .stat-val { color:#10b981; }
      &.wrong .stat-val { color:#ef4444; }
    }
    .badges-earned { background:linear-gradient(135deg,#fefce8,#fef9c3); border-radius:16px; padding:1.5rem; margin-bottom:2rem; text-align:center;
      h3 { margin-bottom:.75rem; }
      .earned-badge { display:inline-block; padding:.4rem 1rem; background:white; border-radius:20px; font-weight:600; margin:.25rem; box-shadow:0 2px 8px rgba(0,0,0,.06); }
    }
    .review-section { margin-bottom:2rem; h2 { font-size:1.3rem; font-weight:800; margin-bottom:1rem; } }
    .review-card { background:white; border-radius:14px; padding:1.25rem; margin-bottom:.75rem; border-inline-start:4px solid; transition:all .2s;
      &.correct { border-color:#10b981; }
      &.wrong { border-color:#ef4444; }
    }
    .review-header { display:flex; align-items:center; gap:.5rem; margin-bottom:.5rem;
      .review-num { font-weight:800; color:#6366f1; }
      .review-pts { margin-inline-start:auto; font-size:.85rem; color:#6b7280; font-weight:600; }
    }
    .review-q { font-weight:600; color:#1f2937; margin-bottom:.75rem; }
    .your-answer { font-size:.9rem; color:#374151; margin-bottom:.25rem; }
    .correct-answer { font-size:.9rem; color:#059669; margin-bottom:.25rem; }
    .explanation { font-size:.85rem; color:#6b7280; background:#f9fafb; padding:.5rem .75rem; border-radius:8px; margin-top:.5rem; }
    .result-actions { display:flex; gap:1rem; justify-content:center; }
  `]
})
export class TestResultComponent implements OnInit {
  result = signal<AttemptResult | null>(null);
  loading = signal(true);

  constructor(private api: ApiService, private route: ActivatedRoute) {}

  ngOnInit() {
    const attemptId = +this.route.snapshot.params['attemptId'];
    this.api.getAttemptDetail(attemptId).subscribe({
      next: res => { this.loading.set(false); if (res.success) this.result.set(res.data); },
      error: () => this.loading.set(false)
    });
  }

  formatTime(seconds: number): string {
    const m = Math.floor(seconds / 60);
    const s = seconds % 60;
    return `${m}:${s.toString().padStart(2, '0')}`;
  }
}
