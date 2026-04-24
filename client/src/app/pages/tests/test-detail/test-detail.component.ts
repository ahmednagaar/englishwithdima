import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, ActivatedRoute, Router } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { ApiService } from '../../../core/services/api.service';
import { AuthService } from '../../../core/services/auth.service';
import { TestDetail } from '../../../core/models';

@Component({
  selector: 'app-test-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslateModule],
  template: `
    <section class="test-detail-page">
      <div class="container">
        @if (loading()) {
          <div class="loading-spinner"></div>
        } @else if (test()) {
          <div class="test-hero animate-fade-in-up">
            <div class="test-hero-content">
              <span class="badge badge-primary">{{ 'ENUMS.TEST_TYPE.' + test()!.testType | translate }}</span>
              <h1>{{ test()!.titleAr }}</h1>
              <p class="test-en-title">{{ test()!.titleEn }}</p>
              @if (test()!.descriptionAr) { <p class="test-desc">{{ test()!.descriptionAr }}</p> }
            </div>
          </div>

          <div class="detail-grid animate-fade-in-up" style="animation-delay:.15s">
            <div class="detail-card">
              <div class="detail-stats">
                <div class="stat-item"><span class="stat-icon">📝</span><span class="stat-value">{{ test()!.questionCount }}</span><span class="stat-label">{{ 'TESTS.QUESTIONS' | translate }}</span></div>
                <div class="stat-item"><span class="stat-icon">⭐</span><span class="stat-value">{{ test()!.totalPoints }}</span><span class="stat-label">{{ 'COMMON.POINTS' | translate }}</span></div>
                <div class="stat-item"><span class="stat-icon">🎯</span><span class="stat-value">{{ test()!.passingScore }}%</span><span class="stat-label">{{ 'TESTS.PASSING_SCORE' | translate }}</span></div>
                @if (test()!.isTimedTest) {
                  <div class="stat-item"><span class="stat-icon">⏱️</span><span class="stat-value">{{ test()!.timeLimitMinutes }}</span><span class="stat-label">{{ 'COMMON.MINUTES' | translate }}</span></div>
                }
              </div>

              @if (test()!.instructions) {
                <div class="instructions-box">
                  <h3>📋 {{ 'TESTS.INSTRUCTIONS' | translate }}</h3>
                  <p>{{ test()!.instructions }}</p>
                </div>
              }

              <div class="test-info-list">
                @if (test()!.gradeName) { <div class="info-row"><span>📚 {{ 'TESTS.GRADE_LABEL' | translate }}</span><span>{{ test()!.gradeName }}</span></div> }
                @if (test()!.unitName) { <div class="info-row"><span>📖 {{ 'TESTS.UNIT_LABEL' | translate }}</span><span>{{ test()!.unitName }}</span></div> }
                <div class="info-row"><span>🔄 {{ 'TESTS.RETAKES' | translate }}</span><span>{{ test()!.allowRetake ? (('TESTS.ALLOWED' | translate) + (test()!.maxRetakeCount ? ' (' + test()!.maxRetakeCount + ')' : '')) : ('TESTS.NOT_ALLOWED' | translate) }}</span></div>
                <div class="info-row"><span>🔀 {{ 'TESTS.SHUFFLE' | translate }}</span><span>{{ test()!.shuffleQuestions ? ('TESTS.YES' | translate) : ('TESTS.NO' | translate) }}</span></div>
              </div>

              <div class="action-area">
                @if (auth.isLoggedIn()) {
                  <a [routerLink]="['/tests', test()!.id, 'take']" class="btn btn-primary btn-lg">{{ 'TESTS.START_TEST' | translate }} 🚀</a>
                } @else {
                  <a routerLink="/auth/login" class="btn btn-primary btn-lg">{{ 'TESTS.LOGIN_TO_START' | translate }}</a>
                }
              </div>
            </div>

            <div class="sidebar">
              <div class="sidebar-card">
                <h3>📊 {{ 'TESTS.RESULTS' | translate }}</h3>
                @if (attempts().length) {
                  @for (attempt of attempts(); track attempt.attemptId) {
                    <div class="attempt-row" [class.passed]="attempt.passed">
                      <div class="attempt-info">
                        <span class="attempt-num">#{{ attempt.attemptNumber }}</span>
                        <span class="attempt-score">{{ attempt.percentage }}%</span>
                      </div>
                      <span class="attempt-badge" [class.pass]="attempt.passed">{{ attempt.passed ? '✅' : '❌' }}</span>
                    </div>
                  }
                } @else {
                  <p class="no-attempts">{{ 'TESTS.NO_ATTEMPTS' | translate }}</p>
                }
              </div>
            </div>
          </div>
        }
      </div>
    </section>
  `,
  styles: [`
    .test-detail-page { padding:2rem 0 5rem; }
    .test-hero { background:linear-gradient(135deg,#f5f3ff,#ede9fe); border-radius:20px; padding:2.5rem; margin-bottom:2rem;
      .badge { margin-bottom:.75rem; }
      h1 { font-size:2rem; font-weight:800; color:#1e1b4b; margin-bottom:.25rem; }
      .test-en-title { font-size:1.1rem; color:#6366f1; font-weight:600; margin-bottom:.5rem; }
      .test-desc { color:#4b5563; line-height:1.7; }
    }
    .detail-grid { display:grid; grid-template-columns:1fr 340px; gap:2rem; @media(max-width:768px){ grid-template-columns:1fr; } }
    .detail-card { background:white; border-radius:20px; padding:2rem; box-shadow:0 4px 20px rgba(0,0,0,.06); }
    .detail-stats { display:grid; grid-template-columns:repeat(4,1fr); gap:1rem; margin-bottom:2rem; @media(max-width:600px){ grid-template-columns:repeat(2,1fr); } }
    .stat-item { text-align:center; padding:1rem; border-radius:14px; background:#f9fafb;
      .stat-icon { font-size:1.5rem; display:block; margin-bottom:.25rem; }
      .stat-value { font-size:1.5rem; font-weight:900; color:#6366f1; display:block; }
      .stat-label { font-size:.75rem; color:#6b7280; font-weight:500; }
    }
    .instructions-box { background:#fffbeb; border:1px solid #fde68a; border-radius:12px; padding:1.25rem; margin-bottom:1.5rem;
      h3 { font-size:1rem; font-weight:700; margin-bottom:.5rem; }
      p { font-size:.9rem; color:#4b5563; line-height:1.7; }
    }
    .test-info-list { margin-bottom:2rem; }
    .info-row { display:flex; justify-content:space-between; padding:.6rem 0; border-bottom:1px solid #f3f4f6; font-size:.9rem;
      span:first-child { color:#6b7280; }
      span:last-child { font-weight:600; color:#1f2937; }
    }
    .action-area { text-align:center; }
    .sidebar-card { background:white; border-radius:16px; padding:1.5rem; box-shadow:0 4px 15px rgba(0,0,0,.06);
      h3 { font-size:1rem; font-weight:700; margin-bottom:1rem; }
    }
    .attempt-row { display:flex; align-items:center; justify-content:space-between; padding:.6rem; border-radius:10px; margin-bottom:.5rem; background:#f9fafb;
      &.passed { background:#f0fdf4; }
      .attempt-num { font-weight:600; color:#6b7280; margin-inline-end:.5rem; }
      .attempt-score { font-weight:800; font-size:1.1rem; color:#1f2937; }
    }
    .no-attempts { color:#9ca3af; font-size:.9rem; text-align:center; padding:1rem 0; }
  `]
})
export class TestDetailComponent implements OnInit {
  test = signal<TestDetail | null>(null);
  attempts = signal<any[]>([]);
  loading = signal(true);

  constructor(private api: ApiService, public auth: AuthService, private route: ActivatedRoute) {}

  ngOnInit() {
    const id = +this.route.snapshot.params['id'];
    this.api.getTest(id).subscribe({
      next: res => { this.loading.set(false); if (res.success) this.test.set(res.data); },
      error: () => this.loading.set(false)
    });
    if (this.auth.isLoggedIn()) {
      this.api.getTestResults(id).subscribe(res => { if (res.success) this.attempts.set(res.data); });
    }
  }
}
