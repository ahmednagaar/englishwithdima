import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { ApiService } from '../../../core/services/api.service';
import { Question, Grade } from '../../../core/models';

@Component({
  selector: 'app-admin-questions',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule],
  template: `
    <section class="admin-page">
      <div class="container">
        <div class="admin-header">
          <h1>{{ 'ADMIN.QUESTIONS_MGMT' | translate }}</h1>
          <button class="btn btn-primary" (click)="showForm.set(!showForm())">
            {{ showForm() ? ('ADMIN.CLOSE' | translate) : ('ADMIN.ADD_QUESTION_BTN' | translate) }}
          </button>
        </div>

        <!-- Create/Edit Form -->
        @if (showForm()) {
          <div class="form-card animate-scale-in">
            <h2>{{ editingId() ? ('ADMIN.EDIT_QUESTION' | translate) : ('ADMIN.NEW_QUESTION' | translate) }}</h2>
            <form (ngSubmit)="onSave()">
              <div class="form-row-3">
                <div class="form-group">
                  <label>{{ 'ADMIN.GRADE' | translate }}</label>
                  <select [(ngModel)]="form.gradeId" name="gradeId" required>
                    @for (g of grades(); track g.id) { <option [ngValue]="g.id">{{ g.nameAr }}</option> }
                  </select>
                </div>
                <div class="form-group">
                  <label>{{ 'ADMIN.QUESTION_TYPE' | translate }}</label>
                  <select [(ngModel)]="form.questionType" name="questionType" required>
                    @for (t of questionTypes; track t) { <option [value]="t">{{ 'ENUMS.QUESTION_TYPE.' + t | translate }}</option> }
                  </select>
                </div>
                <div class="form-group">
                  <label>{{ 'ADMIN.DIFFICULTY' | translate }}</label>
                  <select [(ngModel)]="form.difficultyLevel" name="difficultyLevel">
                    <option value="Easy">{{ 'ADMIN.EASY' | translate }}</option><option value="Medium">{{ 'ADMIN.MEDIUM' | translate }}</option><option value="Hard">{{ 'ADMIN.HARD' | translate }}</option>
                  </select>
                </div>
              </div>

              <div class="form-group">
                <label>{{ 'ADMIN.QUESTION_TEXT' | translate }}</label>
                <textarea [(ngModel)]="form.questionText" name="questionText" rows="3" required></textarea>
              </div>
              <div class="form-group">
                <label>{{ 'ADMIN.INSTRUCTION_AR' | translate }}</label>
                <input type="text" [(ngModel)]="form.instructionAr" name="instructionAr" placeholder="تعليمات السؤال">
              </div>

              <div class="form-row-3">
                <div class="form-group">
                  <label>{{ 'ADMIN.SKILL_CATEGORY' | translate }}</label>
                  <select [(ngModel)]="form.skillCategory" name="skillCategory">
                    @for (s of skills; track s) { <option [value]="s">{{ 'ENUMS.SKILL.' + s | translate }}</option> }
                  </select>
                </div>
                <div class="form-group">
                  <label>{{ 'ADMIN.POINTS' | translate }}</label>
                  <input type="number" [(ngModel)]="form.points" name="points" min="1">
                </div>
                <div class="form-group">
                  <label>{{ 'ADMIN.EST_TIME' | translate }}</label>
                  <input type="number" [(ngModel)]="form.estimatedTimeMinutes" name="time" min="1">
                </div>
              </div>

              <div class="form-group">
                <label>{{ 'ADMIN.CORRECT_ANSWER' | translate }}</label>
                <input type="text" [(ngModel)]="form.correctAnswer" name="correctAnswer">
              </div>
              <div class="form-group">
                <label>{{ 'ADMIN.EXPLANATION' | translate }}</label>
                <textarea [(ngModel)]="form.explanation" name="explanation" rows="2"></textarea>
              </div>
              <div class="form-group">
                <label>{{ 'ADMIN.HINT' | translate }}</label>
                <input type="text" [(ngModel)]="form.hintText" name="hintText">
              </div>

              <!-- MCQ Options -->
              @if (['MultipleChoice','TrueFalse'].includes(form.questionType)) {
                <div class="options-editor">
                  <h3>{{ 'ADMIN.OPTIONS' | translate }}</h3>
                  @for (opt of form.options; track opt.orderIndex; let i = $index) {
                    <div class="option-row">
                      <input type="text" [(ngModel)]="opt.optionText" [name]="'opt_' + i" [placeholder]="('ADMIN.OPTION_PLACEHOLDER' | translate).replace('{{num}}', '' + (i+1))">
                      <label class="correct-check">
                        <input type="checkbox" [(ngModel)]="opt.isCorrect" [name]="'correct_' + i"> ✓
                      </label>
                      <button type="button" class="btn-remove" (click)="removeOption(i)">✕</button>
                    </div>
                  }
                  <button type="button" class="btn btn-sm btn-secondary" (click)="addOption()">{{ 'ADMIN.ADD_OPTION' | translate }}</button>
                </div>
              }

              <div class="form-actions">
                <button type="button" class="btn btn-secondary" (click)="resetForm()">{{ 'ADMIN.CANCEL' | translate }}</button>
                <button type="submit" class="btn btn-primary" [disabled]="saving()">{{ saving() ? ('ADMIN.SAVING' | translate) : ('ADMIN.SAVE_QUESTION' | translate) }}</button>
              </div>
            </form>
          </div>
        }

        <!-- Filters -->
        <div class="filter-bar">
          <select [(ngModel)]="filterGrade" (ngModelChange)="loadQuestions()">
            <option [ngValue]="null">{{ 'ADMIN.ALL_GRADES' | translate }}</option>
            @for (g of grades(); track g.id) { <option [ngValue]="g.id">{{ g.nameAr }}</option> }
          </select>
          <select [(ngModel)]="filterType" (ngModelChange)="loadQuestions()">
            <option value="">{{ 'ADMIN.ALL_TYPES' | translate }}</option>
            @for (t of questionTypes; track t) { <option [value]="t">{{ 'ENUMS.QUESTION_TYPE.' + t | translate }}</option> }
          </select>
          <span class="count-badge">{{ questions().length }} {{ 'ADMIN.QUESTIONS_LABEL' | translate }}</span>
        </div>

        <!-- Question Table -->
        <div class="table-card">
          @if (loading()) {
            <div class="loading-spinner"></div>
          } @else {
            <div class="table-wrap">
              <table>
                <thead>
                  <tr><th>#</th><th>{{ 'ADMIN.QUESTION_COL' | translate }}</th><th>{{ 'ADMIN.TYPE_COL' | translate }}</th><th>{{ 'ADMIN.GRADE_COL' | translate }}</th><th>{{ 'ADMIN.DIFFICULTY_COL' | translate }}</th><th>{{ 'ADMIN.POINTS_COL' | translate }}</th><th>{{ 'ADMIN.ACTIONS_COL' | translate }}</th></tr>
                </thead>
                <tbody>
                  @for (q of questions(); track q.id; let i = $index) {
                    <tr>
                      <td>{{ i + 1 }}</td>
                      <td class="q-cell">{{ q.questionText | slice:0:60 }}{{ q.questionText.length > 60 ? '...' : '' }}</td>
                      <td><span class="badge badge-primary">{{ 'ENUMS.QUESTION_TYPE.' + q.questionType | translate }}</span></td>
                      <td>{{ getGradeName(q.gradeId) }}</td>
                      <td><span class="badge" [class]="'badge-' + q.difficultyLevel.toLowerCase()">{{ 'ENUMS.DIFFICULTY.' + q.difficultyLevel | translate }}</span></td>
                      <td>{{ q.points }}</td>
                      <td class="actions-cell">
                        <button class="btn-icon" (click)="editQuestion(q)" [title]="'COMMON.EDIT' | translate">✏️</button>
                        <button class="btn-icon danger" (click)="deleteQuestion(q.id)" [title]="'COMMON.DELETE' | translate">🗑️</button>
                      </td>
                    </tr>
                  } @empty {
                    <tr><td colspan="7" class="empty-cell">{{ 'ADMIN.NO_QUESTIONS' | translate }}</td></tr>
                  }
                </tbody>
              </table>
            </div>
          }
        </div>
      </div>
    </section>
  `,
  styles: [`
    .admin-page { padding:2rem 0 5rem; }
    .admin-header { display:flex; justify-content:space-between; align-items:center; margin-bottom:1.5rem;
      h1 { font-size:1.6rem; font-weight:800; color:#1f2937; }
    }
    .form-card { background:white; border-radius:20px; padding:2rem; box-shadow:0 4px 20px rgba(0,0,0,.06); margin-bottom:2rem;
      h2 { font-size:1.2rem; font-weight:700; margin-bottom:1.5rem; color:#6366f1; }
    }
    .form-row-3 { display:grid; grid-template-columns:repeat(3,1fr); gap:1rem; @media(max-width:768px){ grid-template-columns:1fr; } }
    .options-editor { margin:1.5rem 0; padding:1.25rem; background:#f9fafb; border-radius:12px;
      h3 { font-size:1rem; font-weight:700; margin-bottom:.75rem; }
    }
    .option-row { display:flex; align-items:center; gap:.5rem; margin-bottom:.5rem;
      input[type="text"] { flex:1; padding:.5rem .75rem; border:2px solid #e5e7eb; border-radius:8px; font-size:.9rem; }
      .correct-check { display:flex; align-items:center; gap:.25rem; font-weight:700; color:#10b981; cursor:pointer; white-space:nowrap; }
      .btn-remove { background:none; border:none; cursor:pointer; font-size:1rem; color:#ef4444; padding:.25rem; }
    }
    .form-actions { display:flex; gap:1rem; justify-content:flex-end; margin-top:1.5rem; }
    .filter-bar { display:flex; align-items:center; gap:1rem; margin-bottom:1rem; flex-wrap:wrap;
      select { padding:.5rem 1rem; border:2px solid #e5e7eb; border-radius:10px; font-size:.9rem; background:white; }
      .count-badge { margin-inline-start:auto; font-weight:700; color:#6366f1; font-size:.9rem; }
    }
    .table-card { background:white; border-radius:16px; overflow:hidden; box-shadow:0 2px 12px rgba(0,0,0,.04); }
    .table-wrap { overflow-x:auto; }
    table { width:100%; border-collapse:collapse;
      th { background:#f9fafb; padding:.75rem 1rem; text-align:start; font-weight:700; font-size:.85rem; color:#6b7280; border-bottom:2px solid #e5e7eb; }
      td { padding:.75rem 1rem; border-bottom:1px solid #f3f4f6; font-size:.9rem; }
      tr:hover td { background:#f5f3ff; }
    }
    .q-cell { max-width:300px; font-weight:500; }
    .badge-easy { background:#d1fae5; color:#059669; } .badge-medium { background:#fef3c7; color:#d97706; } .badge-hard { background:#fee2e2; color:#dc2626; }
    .actions-cell { white-space:nowrap; }
    .btn-icon { background:none; border:none; cursor:pointer; font-size:1rem; padding:.25rem .4rem; border-radius:6px; transition:background .2s;
      &:hover { background:#f3f4f6; }
      &.danger:hover { background:#fee2e2; }
    }
    .empty-cell { text-align:center; color:#9ca3af; padding:2rem !important; }
  `]
})
export class QuestionsComponent implements OnInit {
  questions = signal<Question[]>([]);
  grades = signal<Grade[]>([]);
  showForm = signal(false);
  editingId = signal<number | null>(null);
  loading = signal(true);
  saving = signal(false);
  filterGrade: number | null = null;
  filterType = '';

  questionTypes = ['MultipleChoice', 'TrueFalse', 'FillInTheBlank', 'Matching', 'Ordering', 'ShortAnswer', 'Listening', 'ReadingComprehension', 'Writing', 'SentenceCorrection', 'DialogueCompletion'];
  skills = ['Reading', 'Writing', 'Listening', 'Speaking', 'Grammar', 'Vocabulary', 'Phonics', 'Spelling'];

  form: any = this.getEmptyForm();

  constructor(private api: ApiService, private translate: TranslateService) {}

  ngOnInit() {
    this.api.getGrades().subscribe(r => { if (r.success) this.grades.set(r.data); });
    this.loadQuestions();
  }

  loadQuestions() {
    this.loading.set(true);
    const f: any = {};
    if (this.filterGrade) f.gradeId = this.filterGrade;
    if (this.filterType) f.questionType = this.filterType;
    this.api.getQuestions(f).subscribe({ next: r => { this.loading.set(false); if (r.success) this.questions.set(r.data); }, error: () => this.loading.set(false) });
  }

  onSave() {
    this.saving.set(true);
    const obs = this.editingId() ? this.api.updateQuestion(this.editingId()!, this.form) : this.api.createQuestion(this.form);
    obs.subscribe({ next: () => { this.saving.set(false); this.resetForm(); this.loadQuestions(); }, error: () => this.saving.set(false) });
  }

  editQuestion(q: Question) {
    this.editingId.set(q.id);
    this.form = { ...q, options: q.options?.map(o => ({ ...o })) || [] };
    this.showForm.set(true);
  }

  deleteQuestion(id: number) {
    if (confirm(this.translate.instant('ADMIN.DELETE_CONFIRM'))) {
      this.api.deleteQuestion(id).subscribe(() => this.loadQuestions());
    }
  }

  addOption() { this.form.options.push({ optionText: '', isCorrect: false, orderIndex: this.form.options.length }); }
  removeOption(i: number) { this.form.options.splice(i, 1); }

  resetForm() { this.form = this.getEmptyForm(); this.editingId.set(null); this.showForm.set(false); }

  private getEmptyForm() {
    return { questionText: '', instructionAr: '', questionType: 'MultipleChoice', difficultyLevel: 'Medium', skillCategory: 'Grammar', gradeId: 1, points: 5, estimatedTimeMinutes: 2, correctAnswer: '', explanation: '', hintText: '', options: [{ optionText: '', isCorrect: true, orderIndex: 0 }, { optionText: '', isCorrect: false, orderIndex: 1 }, { optionText: '', isCorrect: false, orderIndex: 2 }, { optionText: '', isCorrect: false, orderIndex: 3 }] };
  }

  getGradeName(gradeId: number): string {
    const grade = this.grades().find(g => g.id === gradeId);
    return grade?.nameAr || gradeId.toString();
  }
}
