import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { ApiService } from '../../../core/services/api.service';
import { Test, Grade } from '../../../core/models';

@Component({
  selector: 'app-admin-tests',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule],
  template: `
    <section class="admin-page">
      <div class="container">
        <div class="admin-header">
          <h1>{{ 'ADMIN.TESTS_MGMT' | translate }}</h1>
          <button class="btn btn-primary" (click)="toggleForm()">
            {{ showForm() ? '✕' : ('ADMIN.CREATE_TEST_BTN' | translate) }}
          </button>
        </div>

        @if (showForm()) {
          <div class="form-card animate-scale-in">
            <h2>{{ editingId() ? ('ADMIN.EDIT_TEST' | translate) : ('ADMIN.NEW_TEST' | translate) }}</h2>
            <form (ngSubmit)="onSave()">
              <div class="form-row">
                <div class="form-group"><label>{{ 'ADMIN.TITLE_AR' | translate }}</label><input type="text" [(ngModel)]="form.titleAr" name="titleAr" required></div>
                <div class="form-group"><label>{{ 'ADMIN.TITLE_EN' | translate }}</label><input type="text" [(ngModel)]="form.titleEn" name="titleEn" required></div>
              </div>
              <div class="form-row">
                <div class="form-group"><label>{{ 'ADMIN.DESC_AR' | translate }}</label><textarea [(ngModel)]="form.descriptionAr" name="descAr" rows="2"></textarea></div>
                <div class="form-group"><label>{{ 'ADMIN.DESC_EN' | translate }}</label><textarea [(ngModel)]="form.descriptionEn" name="descEn" rows="2"></textarea></div>
              </div>
              <div class="form-row-3">
                <div class="form-group">
                  <label>{{ 'ADMIN.GRADE' | translate }} *</label>
                  <select [(ngModel)]="form.gradeId" name="gradeId" required>
                    @for (g of grades(); track g.id) { <option [ngValue]="g.id">{{ g.nameAr }}</option> }
                  </select>
                </div>
                <div class="form-group">
                  <label>{{ 'ADMIN.TEST_TYPE' | translate }}</label>
                  <select [(ngModel)]="form.testType" name="testType">
                    <option value="Quiz">{{ 'ADMIN.QUIZ' | translate }}</option><option value="Monthly">{{ 'ADMIN.MONTHLY' | translate }}</option><option value="Final">{{ 'ADMIN.FINAL' | translate }}</option>
                  </select>
                </div>
                <div class="form-group">
                  <label>{{ 'ADMIN.PASSING_SCORE' | translate }}</label>
                  <input type="number" [(ngModel)]="form.passingScore" name="passingScore" min="1" max="100">
                </div>
              </div>
              <div class="form-row-3">
                <div class="form-group">
                  <label><input type="checkbox" [(ngModel)]="form.isTimedTest" name="timed"> {{ 'ADMIN.TIMED_TEST' | translate }}</label>
                </div>
                @if (form.isTimedTest) {
                  <div class="form-group"><label>{{ 'ADMIN.TIME_LIMIT' | translate }}</label><input type="number" [(ngModel)]="form.timeLimitMinutes" name="timeLimit" min="1"></div>
                }
                <div class="form-group">
                  <label><input type="checkbox" [(ngModel)]="form.allowRetake" name="retake"> {{ 'ADMIN.ALLOW_RETAKE' | translate }}</label>
                </div>
              </div>
              <div class="form-row-3">
                <div class="form-group"><label><input type="checkbox" [(ngModel)]="form.shuffleQuestions" name="shuffle"> {{ 'ADMIN.SHUFFLE_QUESTIONS' | translate }}</label></div>
                <div class="form-group"><label><input type="checkbox" [(ngModel)]="form.shuffleOptions" name="shuffleOpt"> {{ 'ADMIN.SHUFFLE_OPTIONS' | translate }}</label></div>
                <div class="form-group"><label><input type="checkbox" [(ngModel)]="form.showExplanations" name="expl"> {{ 'ADMIN.SHOW_EXPLANATIONS' | translate }}</label></div>
              </div>
              <div class="form-group"><label>{{ 'ADMIN.INSTRUCTIONS' | translate }}</label><textarea [(ngModel)]="form.instructions" name="instructions" rows="2"></textarea></div>
              <div class="form-actions">
                <button type="button" class="btn btn-secondary" (click)="cancelForm()">{{ 'ADMIN.CANCEL' | translate }}</button>
                <button type="submit" class="btn btn-primary" [disabled]="saving()">{{ saving() ? ('ADMIN.CREATING' | translate) : (editingId() ? ('ADMIN.SAVE' | translate) : ('ADMIN.CREATE_TEST_SUBMIT' | translate)) }}</button>
              </div>
            </form>
          </div>
        }

        <!-- Filter Bar -->
        <div class="filter-bar">
          <select [(ngModel)]="filterGrade" (ngModelChange)="loadTests()">
            <option [ngValue]="null">{{ 'ADMIN.ALL_GRADES' | translate }}</option>
            @for (g of grades(); track g.id) { <option [ngValue]="g.id">{{ g.nameAr }}</option> }
          </select>
          <span class="count-badge">{{ tests().length }} {{ 'ADMIN.TESTS_LABEL' | translate }}</span>
        </div>

        <!-- Test Cards -->
        <div class="tests-admin-grid">
          @if (loading()) {
            <div class="loading-spinner"></div>
          } @else {
            @for (test of tests(); track test.id; let i = $index) {
              <div class="test-admin-card animate-fade-in-up" [style.animation-delay]="i * 0.05 + 's'">
                <div class="tac-header">
                  <span class="badge" [class]="test.isPublished ? 'badge-success' : 'badge-warning'">{{ test.isPublished ? ('ADMIN.PUBLISHED' | translate) : ('ADMIN.DRAFT' | translate) }}</span>
                  <span class="badge badge-primary">{{ 'ENUMS.TEST_TYPE.' + test.testType | translate }}</span>
                </div>
                <h3>{{ test.titleAr }}</h3>
                <p>{{ test.descriptionAr }}</p>
                <div class="tac-meta">
                  <span>📝 {{ test.questionCount }} {{ 'ADMIN.QUESTIONS_SHORT' | translate }}</span>
                  <span>⭐ {{ test.totalPoints }} {{ 'ADMIN.PTS' | translate }}</span>
                  @if (test.isTimedTest) { <span>⏱️ {{ test.timeLimitMinutes }}{{ 'ADMIN.MIN' | translate }}</span> }
                </div>
                <div class="tac-actions">
                  <button class="btn btn-sm btn-secondary" (click)="editTest(test)">{{ 'ADMIN.EDIT' | translate }}</button>
                  <button class="btn btn-sm" [class]="test.isPublished ? 'btn-warning' : 'btn-success'" (click)="togglePublish(test)">
                    {{ test.isPublished ? ('ADMIN.UNPUBLISH' | translate) : ('ADMIN.PUBLISH' | translate) }}
                  </button>
                  <button class="btn btn-sm btn-danger" (click)="deleteTest(test)">{{ 'ADMIN.DELETE' | translate }}</button>
                </div>
              </div>
            } @empty {
              <div class="empty-state"><span>📋</span><p>{{ 'ADMIN.NO_TESTS' | translate }}</p></div>
            }
          }
        </div>
      </div>
    </section>
  `,
  styles: [`
    .admin-page { padding:2rem 0 5rem; }
    .admin-header { display:flex; justify-content:space-between; align-items:center; margin-bottom:1.5rem; h1 { font-size:1.6rem; font-weight:800; } }
    .form-card { background:white; border-radius:20px; padding:2rem; box-shadow:0 4px 20px rgba(0,0,0,.06); margin-bottom:2rem; h2 { font-size:1.2rem; font-weight:700; margin-bottom:1.5rem; color:#6366f1; } }
    .form-row { display:grid; grid-template-columns:1fr 1fr; gap:1rem; @media(max-width:600px){ grid-template-columns:1fr; } }
    .form-row-3 { display:grid; grid-template-columns:repeat(3,1fr); gap:1rem; @media(max-width:768px){ grid-template-columns:1fr; } }
    .form-actions { display:flex; gap:1rem; justify-content:flex-end; margin-top:1.5rem; }
    .filter-bar { display:flex; align-items:center; gap:1rem; margin-bottom:1.5rem; flex-wrap:wrap;
      select { padding:.5rem 1rem; border:2px solid #e5e7eb; border-radius:10px; font-size:.9rem; background:white; }
      .count-badge { margin-inline-start:auto; font-weight:700; color:#6366f1; font-size:.9rem; }
    }
    .tests-admin-grid { display:grid; grid-template-columns:repeat(3,1fr); gap:1.25rem; @media(max-width:1024px){ grid-template-columns:repeat(2,1fr); } @media(max-width:600px){ grid-template-columns:1fr; } }
    .test-admin-card { background:white; border-radius:16px; padding:1.5rem; box-shadow:0 2px 12px rgba(0,0,0,.04); transition:all .3s; animation:fadeInUp .5s ease forwards; opacity:0;
      &:hover { transform:translateY(-3px); box-shadow:0 8px 25px rgba(0,0,0,.08); }
      h3 { font-weight:700; margin:.5rem 0 .15rem; color:#1f2937; }
      p { font-size:.85rem; color:#6b7280; margin-bottom:.75rem; }
    }
    .tac-header { display:flex; gap:.5rem; margin-bottom:.5rem; }
    .tac-meta { display:flex; flex-wrap:wrap; gap:.75rem; font-size:.8rem; color:#6b7280; margin-bottom:1rem; }
    .tac-actions { display:flex; gap:.5rem; flex-wrap:wrap; }
    .empty-state { grid-column:1/-1; text-align:center; padding:4rem; span { font-size:4rem; display:block; margin-bottom:1rem; } p { color:#9ca3af; font-size:1.1rem; } }
    .btn-danger { background:#ef4444 !important; color:white !important; border:none !important; &:hover { background:#dc2626 !important; } }
    .btn-success { background:#10b981 !important; color:white !important; border:none !important; &:hover { background:#059669 !important; } }
    .btn-warning { background:#f59e0b !important; color:white !important; border:none !important; &:hover { background:#d97706 !important; } }
  `]
})
export class AdminTestsComponent implements OnInit {
  tests = signal<Test[]>([]);
  grades = signal<Grade[]>([]);
  showForm = signal(false);
  editingId = signal<number | null>(null);
  loading = signal(true);
  saving = signal(false);
  filterGrade: number | null = null;
  form: any = this.getEmptyForm();

  constructor(private api: ApiService, private translate: TranslateService) {}

  ngOnInit() {
    this.api.getGrades().subscribe(r => { if (r.success) this.grades.set(r.data); });
    this.loadTests();
  }

  loadTests() {
    this.loading.set(true);
    const f: any = {};
    if (this.filterGrade) f.gradeId = this.filterGrade;
    this.api.getTests(f).subscribe({ next: r => { this.loading.set(false); if (r.success) this.tests.set(r.data); }, error: () => this.loading.set(false) });
  }

  toggleForm() {
    if (this.showForm()) {
      this.cancelForm();
    } else {
      this.form = this.getEmptyForm();
      this.editingId.set(null);
      this.showForm.set(true);
    }
  }

  cancelForm() {
    this.form = this.getEmptyForm();
    this.editingId.set(null);
    this.showForm.set(false);
  }

  onSave() {
    this.saving.set(true);
    const obs = this.editingId()
      ? this.api.updateTest(this.editingId()!, this.form)
      : this.api.createTest(this.form);
    obs.subscribe({
      next: () => { this.saving.set(false); this.cancelForm(); this.loadTests(); },
      error: () => this.saving.set(false)
    });
  }

  editTest(test: Test) {
    this.editingId.set(test.id);
    this.form = {
      titleAr: test.titleAr, titleEn: test.titleEn,
      descriptionAr: test.descriptionAr || '', descriptionEn: test.descriptionEn || '',
      gradeId: test.gradeId, testType: test.testType,
      passingScore: test.passingScore, isTimedTest: test.isTimedTest,
      timeLimitMinutes: test.timeLimitMinutes || 30, allowRetake: test.allowRetake,
      shuffleQuestions: true, shuffleOptions: true, showExplanations: true,
      instructions: test.instructions || ''
    };
    this.showForm.set(true);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  togglePublish(test: Test) {
    this.api.publishTest(test.id, !test.isPublished).subscribe({
      next: () => this.loadTests()
    });
  }

  deleteTest(test: Test) {
    if (!confirm(this.translate.instant('ADMIN.DELETE_CONFIRM_TEST', { title: test.titleAr }))) return;
    this.api.deleteTest(test.id).subscribe({
      next: () => this.loadTests()
    });
  }

  private getEmptyForm() {
    return {
      titleAr: '', titleEn: '', descriptionAr: '', descriptionEn: '',
      gradeId: 1, testType: 'Quiz', passingScore: 60,
      isTimedTest: false, timeLimitMinutes: 30, allowRetake: true,
      shuffleQuestions: true, shuffleOptions: true, showExplanations: true,
      instructions: ''
    };
  }
}
