import { Component, OnInit, signal, computed, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { ApiService } from '../../../core/services/api.service';
import { AttemptStart, Question, SubmitAttempt, AnswerSubmission } from '../../../core/models';

@Component({
  selector: 'app-test-take',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule],
  template: `
    <section class="test-take-page">
      @if (loading()) {
        <div class="loading-spinner"></div>
      } @else if (attempt()) {
        <!-- Timer Bar -->
        <div class="timer-bar" [class.danger]="timeRemaining() < 60">
          <div class="container timer-content">
            <span class="test-title">{{ attempt()!.testTitle }}</span>
            <div class="timer-info">
              <span class="question-progress">{{ currentIndex() + 1 }} / {{ attempt()!.questions.length }}</span>
              @if (attempt()!.isTimedTest) {
                <span class="timer-clock" [class.warning]="timeRemaining() < 120">⏱️ {{ formatTime(timeRemaining()) }}</span>
              }
            </div>
          </div>
          <div class="progress-fill" [style.width]="((currentIndex() + 1) / attempt()!.questions.length * 100) + '%'"></div>
        </div>

        <!-- Question Card -->
        <div class="container">
          <div class="question-card animate-fade-in">
            @if (currentQuestion()) {
              <div class="q-header">
                <span class="q-number">{{ 'TESTS.QUESTION_SHORT' | translate }}{{ currentIndex() + 1 }}</span>
                <span class="q-type badge badge-primary">{{ 'ENUMS.QUESTION_TYPE.' + currentQuestion()!.questionType | translate }}</span>
                <span class="q-points">{{ currentQuestion()!.points }} {{ 'TESTS.POINTS_LABEL' | translate }}</span>
              </div>
              <h2 class="q-text">{{ currentQuestion()!.questionText }}</h2>

              @if (currentQuestion()!.imageUrl) { <img [src]="currentQuestion()!.imageUrl" class="q-image" alt="Question image"> }

              <!-- MCQ Options -->
              @if (currentQuestion()!.options.length > 0) {
                <div class="options-list">
                  @for (opt of currentQuestion()!.options; track opt.id) {
                    <label class="option-item" [class.selected]="isOptionSelected(opt.id)" (click)="selectOption(opt.id)">
                      <span class="option-radio" [class.checked]="isOptionSelected(opt.id)"></span>
                      <span class="option-text">{{ opt.optionText }}</span>
                    </label>
                  }
                </div>
              } @else {
                <!-- Text Answer -->
                <div class="form-group">
                  <textarea [(ngModel)]="textAnswer" name="textAnswer" rows="3" [placeholder]="'TESTS.TYPE_ANSWER' | translate"></textarea>
                </div>
              }
            }

            <!-- Navigation -->
            <div class="q-nav">
              <button class="btn btn-secondary" (click)="prevQuestion()" [disabled]="currentIndex() === 0">
                {{ 'TESTS.PREVIOUS' | translate }}
              </button>
              @if (currentIndex() < attempt()!.questions.length - 1) {
                <button class="btn btn-primary" (click)="nextQuestion()">{{ 'TESTS.NEXT' | translate }}</button>
              } @else {
                <button class="btn btn-success btn-lg" (click)="submitTest()">{{ 'TESTS.SUBMIT' | translate }} 🚀</button>
              }
            </div>

            <!-- Question Dots -->
            <div class="q-dots">
              @for (q of attempt()!.questions; track q.id; let i = $index) {
                <button class="dot" [class.current]="i === currentIndex()" [class.answered]="answers[i]" (click)="goToQuestion(i)">
                  {{ i + 1 }}
                </button>
              }
            </div>
          </div>
        </div>
      }
    </section>
  `,
  styles: [`
    .test-take-page { padding:0 0 5rem; }
    .timer-bar { background:white; border-bottom:1px solid #e5e7eb; padding:.75rem 0; position:sticky; top:70px; z-index:50; box-shadow:0 2px 10px rgba(0,0,0,.05);
      &.danger { background:#fef2f2; }
    }
    .timer-content { display:flex; justify-content:space-between; align-items:center; }
    .test-title { font-weight:700; color:#1f2937; font-size:.95rem; }
    .timer-info { display:flex; align-items:center; gap:1.25rem; }
    .question-progress { font-weight:600; color:#6366f1; }
    .timer-clock { font-weight:800; font-size:1.1rem; color:#1f2937; &.warning { color:#ef4444; } }
    .progress-fill { height:3px; background:linear-gradient(90deg,#6366f1,#a855f7); transition:width .3s; }
    .question-card { max-width:720px; margin:2rem auto; background:white; border-radius:20px; padding:2.5rem; box-shadow:0 4px 20px rgba(0,0,0,.06); }
    .q-header { display:flex; align-items:center; gap:.75rem; margin-bottom:1rem; }
    .q-number { font-weight:900; font-size:1.2rem; color:#6366f1; }
    .q-points { margin-inline-start:auto; font-size:.85rem; color:#6b7280; font-weight:600; }
    .q-text { font-size:1.2rem; font-weight:700; color:#1f2937; line-height:1.6; margin-bottom:1.5rem; }
    .q-image { max-height:200px; border-radius:12px; margin-bottom:1.5rem; }
    .options-list { display:flex; flex-direction:column; gap:.6rem; margin-bottom:2rem; }
    .option-item { display:flex; align-items:center; gap:.75rem; padding:1rem 1.25rem; border:2px solid #e5e7eb; border-radius:14px; cursor:pointer; transition:all .2s;
      &:hover { border-color:#a5b4fc; background:#f5f3ff; }
      &.selected { border-color:#6366f1; background:#eef2ff; }
    }
    .option-radio { width:22px; height:22px; border-radius:50%; border:2px solid #d1d5db; transition:all .2s; flex-shrink:0; position:relative;
      &.checked { border-color:#6366f1; &::after { content:''; width:12px; height:12px; background:#6366f1; border-radius:50%; position:absolute; top:3px; inset-inline-start:3px; } }
    }
    .option-text { font-size:1rem; color:#374151; font-weight:500; }
    .q-nav { display:flex; justify-content:space-between; margin-top:2rem; padding-top:1.5rem; border-top:1px solid #f3f4f6; }
    .q-dots { display:flex; flex-wrap:wrap; gap:.4rem; justify-content:center; margin-top:1.5rem; }
    .dot { width:32px; height:32px; border-radius:8px; border:2px solid #e5e7eb; background:white; font-size:.75rem; font-weight:700; cursor:pointer; transition:all .2s; color:#6b7280;
      &.current { border-color:#6366f1; background:#eef2ff; color:#6366f1; }
      &.answered { background:#6366f1; color:white; border-color:#6366f1; }
    }
  `]
})
export class TestTakeComponent implements OnInit, OnDestroy {
  attempt = signal<AttemptStart | null>(null);
  currentIndex = signal(0);
  loading = signal(true);
  timeRemaining = signal(0);
  textAnswer = '';
  answers: Record<number, AnswerSubmission> = {};
  private timer: any;
  private questionStartTime = Date.now();

  currentQuestion = computed(() => this.attempt()?.questions[this.currentIndex()] || null);

  constructor(private api: ApiService, private route: ActivatedRoute, private router: Router) {}

  ngOnInit() {
    const testId = +this.route.snapshot.params['id'];
    this.api.startTest(testId).subscribe({
      next: res => {
        this.loading.set(false);
        if (res.success) {
          this.attempt.set(res.data);
          if (res.data.isTimedTest && res.data.timeLimitMinutes) {
            this.timeRemaining.set(res.data.timeLimitMinutes * 60);
            this.startTimer();
          }
        }
      },
      error: () => this.loading.set(false)
    });
  }

  ngOnDestroy() { clearInterval(this.timer); }

  selectOption(optionId: number) {
    const q = this.currentQuestion();
    if (!q) return;
    const existing = this.answers[this.currentIndex()];
    const ids = existing?.selectedOptionIds || [];
    const idx = ids.indexOf(optionId);
    if (idx >= 0) ids.splice(idx, 1); else ids.push(optionId);
    this.answers[this.currentIndex()] = { questionId: q.id, selectedOptionIds: ids, timeSpentSeconds: this.getTimeSpent() };
  }

  isOptionSelected(optionId: number): boolean {
    return this.answers[this.currentIndex()]?.selectedOptionIds?.includes(optionId) || false;
  }

  nextQuestion() {
    this.saveCurrentAnswer();
    if (this.currentIndex() < (this.attempt()?.questions.length || 0) - 1) {
      this.currentIndex.set(this.currentIndex() + 1);
      this.questionStartTime = Date.now();
      this.loadExistingAnswer();
    }
  }

  prevQuestion() {
    this.saveCurrentAnswer();
    if (this.currentIndex() > 0) {
      this.currentIndex.set(this.currentIndex() - 1);
      this.questionStartTime = Date.now();
      this.loadExistingAnswer();
    }
  }

  goToQuestion(i: number) {
    this.saveCurrentAnswer();
    this.currentIndex.set(i);
    this.questionStartTime = Date.now();
    this.loadExistingAnswer();
  }

  submitTest() {
    this.saveCurrentAnswer();
    const a = this.attempt();
    if (!a) return;
    const submission: SubmitAttempt = {
      attemptId: a.attemptId, testId: a.testId,
      answers: Object.values(this.answers)
    };
    this.api.submitTest(a.testId, submission).subscribe(res => {
      if (res.success) this.router.navigate(['/tests', a.testId, 'result', a.attemptId]);
    });
  }

  formatTime(seconds: number): string {
    const m = Math.floor(seconds / 60);
    const s = seconds % 60;
    return `${m}:${s.toString().padStart(2, '0')}`;
  }

  private startTimer() {
    this.timer = setInterval(() => {
      const t = this.timeRemaining() - 1;
      this.timeRemaining.set(t);
      if (t <= 0) { clearInterval(this.timer); this.submitTest(); }
    }, 1000);
  }

  private saveCurrentAnswer() {
    const q = this.currentQuestion();
    if (!q) return;
    if (!this.answers[this.currentIndex()] && this.textAnswer) {
      this.answers[this.currentIndex()] = { questionId: q.id, answerText: this.textAnswer, timeSpentSeconds: this.getTimeSpent() };
    }
    if (this.answers[this.currentIndex()]) {
      this.answers[this.currentIndex()].timeSpentSeconds = this.getTimeSpent();
      if (this.textAnswer) this.answers[this.currentIndex()].answerText = this.textAnswer;
    }
  }

  private loadExistingAnswer() {
    const existing = this.answers[this.currentIndex()];
    this.textAnswer = existing?.answerText || '';
  }

  private getTimeSpent(): number { return Math.floor((Date.now() - this.questionStartTime) / 1000); }
}
