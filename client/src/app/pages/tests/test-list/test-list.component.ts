import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, ActivatedRoute } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { ApiService } from '../../../core/services/api.service';
import { Test, Grade } from '../../../core/models';

@Component({
  selector: 'app-test-list',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslateModule],
  template: `
    <section class="test-list-page">
      <div class="container">
        <div class="page-header animate-fade-in-up">
          <h1 class="section-title">{{ 'TESTS.TITLE' | translate }}</h1>
          <p class="section-subtitle">{{ 'TESTS.SUBTITLE' | translate }}</p>
        </div>

        <!-- Filters -->
        <div class="filters animate-fade-in-up">
          <div class="filter-chips">
            <button class="chip" [class.active]="!selectedGradeId()" (click)="filterByGrade(null)">{{ 'COMMON.ALL' | translate }}</button>
            @for (grade of grades(); track grade.id) {
              <button class="chip" [class.active]="selectedGradeId() === grade.id" (click)="filterByGrade(grade.id)">{{ grade.nameAr }}</button>
            }
          </div>
        </div>

        <!-- Test Cards -->
        @if (loading()) {
          <div class="loading-spinner"></div>
        } @else {
          <div class="tests-grid">
            @for (test of tests(); track test.id; let i = $index) {
              <div class="test-card animate-fade-in-up" [style.animation-delay]="i * 0.06 + 's'">
                <div class="test-card-header">
                  <span class="test-type-badge" [class]="'badge-' + test.testType.toLowerCase()">{{ 'ENUMS.TEST_TYPE.' + test.testType | translate }}</span>
                  @if (test.skillCategory) {
                    <span class="badge badge-primary">{{ 'ENUMS.SKILL.' + test.skillCategory | translate }}</span>
                  }
                </div>
                <h3>{{ test.titleAr }}</h3>
                <p class="test-title-en">{{ test.titleEn }}</p>
                @if (test.gradeName) { <span class="grade-tag">📚 {{ test.gradeName }}</span> }
                <div class="test-meta">
                  <span><i class="fas fa-list-ol"></i> {{ test.questionCount }} {{ 'TESTS.QUESTIONS' | translate }}</span>
                  @if (test.isTimedTest) { <span><i class="fas fa-clock"></i> {{ test.timeLimitMinutes }} {{ 'COMMON.MINUTES' | translate }}</span> }
                  <span><i class="fas fa-star"></i> {{ test.totalPoints }} {{ 'COMMON.POINTS' | translate }}</span>
                </div>
                <div class="test-footer">
                  <span class="pass-score">{{ 'TESTS.PASSING_SCORE' | translate }}: {{ test.passingScore }}%</span>
                  <a [routerLink]="['/tests', test.id]" class="btn btn-primary btn-sm">{{ 'TESTS.START_TEST' | translate }}</a>
                </div>
              </div>
            } @empty {
              <div class="empty-state">
                <span class="empty-icon">📝</span>
                <p>{{ 'TESTS.NO_TESTS' | translate }}</p>
              </div>
            }
          </div>
        }
      </div>
    </section>
  `,
  styles: [`
    .test-list-page { padding:2rem 0 5rem; }
    .filters { margin-bottom:2rem; }
    .filter-chips { display:flex; flex-wrap:wrap; gap:.5rem; }
    .chip { padding:.45rem 1rem; border-radius:20px; border:2px solid #e5e7eb; background:white; font-weight:600; font-size:.85rem; cursor:pointer; transition:all .2s; color:#4b5563;
      &:hover { border-color:#6366f1; color:#6366f1; }
      &.active { background:linear-gradient(135deg,#6366f1,#a855f7); color:white; border-color:transparent; }
    }
    .tests-grid { display:grid; grid-template-columns:repeat(3,1fr); gap:1.25rem; @media(max-width:1024px){ grid-template-columns:repeat(2,1fr); } @media(max-width:600px){ grid-template-columns:1fr; } }
    .test-card { background:white; border-radius:16px; padding:1.5rem; border:1px solid #f3f4f6; transition:all .3s; animation:fadeInUp .5s ease forwards; opacity:0;
      &:hover { transform:translateY(-4px); box-shadow:0 12px 35px rgba(0,0,0,.08); border-color:rgba(99,102,241,.2); }
      h3 { font-size:1.1rem; font-weight:700; color:#1f2937; margin:.75rem 0 .25rem; }
      .test-title-en { font-size:.85rem; color:#6b7280; margin-bottom:.5rem; }
    }
    .test-card-header { display:flex; align-items:center; gap:.5rem; }
    .test-type-badge { padding:.2rem .6rem; border-radius:6px; font-size:.75rem; font-weight:700; text-transform:uppercase;
      &.badge-monthly { background:#dbeafe; color:#2563eb; }
      &.badge-final { background:#fef3c7; color:#d97706; }
      &.badge-quiz { background:#d1fae5; color:#059669; }
    }
    .grade-tag { font-size:.8rem; color:#6366f1; }
    .test-meta { display:flex; flex-wrap:wrap; gap:.75rem; margin:1rem 0; font-size:.85rem; color:#6b7280;
      i { margin-inline-end:.3rem; color:#a5b4fc; }
    }
    .test-footer { display:flex; align-items:center; justify-content:space-between; border-top:1px solid #f3f4f6; padding-top:1rem; margin-top:.5rem;
      .pass-score { font-size:.8rem; color:#6b7280; font-weight:500; }
    }
    .empty-state { grid-column:1/-1; text-align:center; padding:4rem 2rem;
      .empty-icon { font-size:4rem; display:block; margin-bottom:1rem; }
      p { color:#6b7280; font-size:1.1rem; }
    }
  `]
})
export class TestListComponent implements OnInit {
  tests = signal<Test[]>([]);
  grades = signal<Grade[]>([]);
  selectedGradeId = signal<number | null>(null);
  loading = signal(true);

  constructor(private api: ApiService, private route: ActivatedRoute) {}

  ngOnInit() {
    this.api.getGrades().subscribe(res => { if (res.success) this.grades.set(res.data); });
    this.route.queryParams.subscribe(params => {
      const gradeId = params['gradeId'] ? +params['gradeId'] : null;
      this.selectedGradeId.set(gradeId);
      this.loadTests();
    });
  }

  filterByGrade(gradeId: number | null) {
    this.selectedGradeId.set(gradeId);
    this.loadTests();
  }

  private loadTests() {
    this.loading.set(true);
    const filters: any = {};
    if (this.selectedGradeId()) filters.gradeId = this.selectedGradeId();
    this.api.getTests(filters).subscribe({
      next: res => { this.loading.set(false); if (res.success) this.tests.set(res.data); },
      error: () => this.loading.set(false)
    });
  }
}
