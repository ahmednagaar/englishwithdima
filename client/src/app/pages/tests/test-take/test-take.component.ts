import { Component, OnInit, signal, computed, OnDestroy, HostListener } from '@angular/core';
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
        <div class="container" style="padding-top:2rem">
          <div class="question-card">
            <div class="skeleton skeleton-text" style="width:40%;height:20px"></div>
            <div class="skeleton skeleton-title" style="width:90%;margin:1.5rem 0"></div>
            <div class="skeleton skeleton-text" style="width:100%;height:60px;margin-bottom:12px"></div>
            <div class="skeleton skeleton-text" style="width:100%;height:60px;margin-bottom:12px"></div>
            <div class="skeleton skeleton-text" style="width:100%;height:60px;margin-bottom:12px"></div>
            <div class="skeleton skeleton-text" style="width:100%;height:60px"></div>
          </div>
        </div>
      } @else if (attempt()) {
        <!-- Timer Bar -->
        <div class="timer-bar" [class.warning]="timerState() === 'warning'" [class.danger]="timerState() === 'danger'">
          <div class="container timer-content">
            <button class="question-counter" (click)="showOverview = !showOverview" aria-label="عرض جميع الأسئلة">
              <i class="fas fa-th"></i>
              {{ currentIndex() + 1 }} / {{ attempt()!.questions.length }}
            </button>
            <span class="test-title">{{ attempt()!.testTitle }}</span>
            <div class="timer-info">
              @if (attempt()!.isTimedTest) {
                <span class="timer-clock" [class]="timerState()">
                  ⏱️ {{ formatTime(timeRemaining()) }}
                </span>
              }
            </div>
          </div>
          <div class="progress-fill" [style.width]="((currentIndex() + 1) / attempt()!.questions.length * 100) + '%'"></div>
        </div>

        <!-- Question Overview Panel -->
        @if (showOverview) {
          <div class="overview-backdrop" (click)="showOverview = false"></div>
          <div class="overview-panel">
            <div class="overview-header">
              <h3>الأسئلة</h3>
              <button (click)="showOverview = false" aria-label="إغلاق"><i class="fas fa-times"></i></button>
            </div>
            <div class="overview-grid">
              @for (q of attempt()!.questions; track q.id; let i = $index) {
                <button class="overview-dot"
                  [class.current]="i === currentIndex()"
                  [class.answered]="answers[i]"
                  [class.flagged]="flaggedQuestions.has(i)"
                  (click)="goToQuestion(i); showOverview = false">
                  {{ i + 1 }}
                </button>
              }
            </div>
            <div class="overview-legend">
              <span><span class="legend-dot answered"></span> مُجاب</span>
              <span><span class="legend-dot flagged"></span> مُعلّم</span>
              <span><span class="legend-dot"></span> غير مُجاب</span>
            </div>
          </div>
        }

        <!-- Question Card -->
        <div class="container">
          <div class="question-card animate-fade-in">
            @if (currentQuestion()) {
              <div class="q-header">
                <span class="q-number">{{ 'TESTS.QUESTION_SHORT' | translate }}{{ currentIndex() + 1 }}</span>
                <span class="q-type badge badge-primary">{{ 'ENUMS.QUESTION_TYPE.' + currentQuestion()!.questionType | translate }}</span>
                <span class="q-points">{{ currentQuestion()!.points }} {{ 'TESTS.POINTS_LABEL' | translate }}</span>
                <button class="flag-btn" [class.flagged]="flaggedQuestions.has(currentIndex())"
                  (click)="toggleFlag()" aria-label="علّم للمراجعة">
                  <i class="fas fa-flag"></i>
                </button>
              </div>
              <h2 class="q-text">{{ currentQuestion()!.questionText }}</h2>

              @if (currentQuestion()!.imageUrl) { <img [src]="currentQuestion()!.imageUrl" class="q-image" alt="صورة السؤال" loading="lazy"> }

              @if (currentQuestion()!.questionType === 'TrueFalse') {
                <div class="tf-container">
                  <button class="tf-btn true-btn" [class.selected]="isTrueFalseSelected('True')" (click)="selectTrueFalse('True')">
                    <span class="tf-icon">✓</span>
                    <span class="tf-label">صح</span>
                  </button>
                  <button class="tf-btn false-btn" [class.selected]="isTrueFalseSelected('False')" (click)="selectTrueFalse('False')">
                    <span class="tf-icon">✗</span>
                    <span class="tf-label">خطأ</span>
                  </button>
                </div>
              } @else if (currentQuestion()!.options.length > 0) {
                <div class="options-list">
                  @for (opt of currentQuestion()!.options; track opt.id; let j = $index) {
                    <label class="option-item" [class.selected]="isOptionSelected(opt.id)" (click)="selectOption(opt.id)"
                      [attr.data-letter]="optionLetters[j]">
                      <span class="option-letter">{{ optionLetters[j] }}</span>
                      <span class="option-text">{{ opt.optionText }}</span>
                    </label>
                  }
                </div>
              } @else if (currentQuestion()!.questionType === 'FillInBlank') {
                <div class="form-group">
                  <label>اكتب الإجابة</label>
                  <input type="text" [(ngModel)]="textAnswer" name="textAnswer" class="fill-input"
                    [placeholder]="'TESTS.TYPE_ANSWER' | translate"
                    (input)="saveCurrentAnswer()" autocomplete="off">
                </div>
              } @else {
                <div class="writing-group">
                  <label>اكتب إجابتك هنا</label>
                  <textarea [(ngModel)]="textAnswer" name="textAnswer" rows="5" class="writing-area"
                    [maxlength]="500" placeholder="اكتب هنا..."
                    (input)="saveCurrentAnswer()"></textarea>
                  <div class="writing-footer">
                    <span class="char-counter" [class.near]="textAnswer.length > 400" [class.limit]="textAnswer.length >= 500">
                      {{ textAnswer.length }} / 500
                    </span>
                    <span class="writing-note">✍️ ستراجع المعلمة إجابتك</span>
                  </div>
                </div>
              }
            }

            <!-- Navigation -->
            <div class="q-nav">
              <button class="btn btn-secondary" (click)="prevQuestion()" [disabled]="currentIndex() === 0">
                <i class="fas fa-arrow-right"></i>
                {{ 'TESTS.PREVIOUS' | translate }}
              </button>
              @if (currentIndex() < attempt()!.questions.length - 1) {
                <button class="btn btn-primary" (click)="nextQuestion()">
                  {{ 'TESTS.NEXT' | translate }}
                  <i class="fas fa-arrow-left"></i>
                </button>
              } @else {
                <button class="btn btn-success btn-lg" (click)="confirmSubmit()">
                  مراجعة وإرسال 🚀
                </button>
              }
            </div>

            <!-- Question Dots (mobile compact) -->
            <div class="q-dots">
              @for (q of attempt()!.questions; track q.id; let i = $index) {
                <button class="dot" [class.current]="i === currentIndex()" [class.answered]="answers[i]"
                  [class.flagged]="flaggedQuestions.has(i)" (click)="goToQuestion(i)">
                  {{ i + 1 }}
                </button>
              }
            </div>
          </div>
        </div>
      }

      <!-- Submit Confirmation Dialog -->
      @if (showSubmitDialog) {
        <div class="dialog-backdrop" (click)="showSubmitDialog = false">
          <div class="dialog" (click)="$event.stopPropagation()">
            <div class="dialog-icon">{{ unansweredCount() > 0 ? '⚠️' : '✅' }}</div>
            @if (unansweredCount() > 0) {
              <h3>لديك {{ unansweredCount() }} سؤال بدون إجابة</h3>
              <p>هل تريد إرسال الاختبار رغم ذلك؟</p>
            } @else {
              <h3>هل أنت متأكد من إرسال الاختبار؟</h3>
              <p>أجبت على جميع الأسئلة ✓</p>
            }
            <div class="dialog-actions">
              <button class="btn btn-secondary" (click)="showSubmitDialog = false">العودة للمراجعة</button>
              <button class="btn btn-success" (click)="submitTest()">إرسال الاختبار</button>
            </div>
          </div>
        </div>
      }

      <!-- Leave Confirmation Dialog -->
      @if (showLeaveDialog) {
        <div class="dialog-backdrop">
          <div class="dialog">
            <div class="dialog-icon">🚪</div>
            <h3>هل تريد مغادرة الاختبار؟</h3>
            <p>ستُفقد إجاباتك إذا لم تُكمله</p>
            <div class="dialog-actions">
              <button class="btn btn-primary" (click)="showLeaveDialog = false">استمر في الاختبار</button>
              <button class="btn btn-secondary" (click)="confirmLeave()">اخرج</button>
            </div>
          </div>
        </div>
      }
    </section>
  `,
  styles: [`
    .test-take-page { padding:0 0 5rem; }

    /* Timer Bar */
    .timer-bar {
      background:white; border-bottom:1px solid #e5e7eb;
      padding:.75rem 0; position:sticky; top:70px; z-index:50;
      box-shadow:0 2px 10px rgba(0,0,0,.05); transition: background 0.3s;
      &.warning { background:#fffbeb; }
      &.danger { background:#fef2f2; }
    }
    .timer-content { display:flex; justify-content:space-between; align-items:center; }
    .test-title { font-weight:700; color:#1f2937; font-size:.95rem;
      @media(max-width:600px) { display:none; }
    }
    .question-counter {
      display:flex; align-items:center; gap:.5rem;
      font-weight:700; color:var(--primary); font-size:.95rem;
      background:var(--primary-bg); padding:6px 14px; border-radius:10px;
      min-height:40px; cursor:pointer; transition: all 0.2s;
      &:hover { background:rgba(99,102,241,0.15); }
    }
    .timer-clock {
      font-weight:800; font-size:1.1rem; color:#1f2937;
      font-variant-numeric: tabular-nums;
      &.warning { color:var(--warning); animation: pulse 1s infinite; }
      &.danger { color:var(--danger); animation: pulse 0.5s infinite, shake 0.5s infinite; }
    }
    .progress-fill { height:3px; background:linear-gradient(90deg,#6366f1,#a855f7); transition:width .3s; }

    /* Question Card */
    .question-card {
      max-width:720px; margin:2rem auto; background:white;
      border-radius:20px; padding:2rem; box-shadow:0 4px 20px rgba(0,0,0,.06);
      @media(max-width:600px) { margin:1rem auto; padding:1.25rem; border-radius:16px; }
    }
    .q-header { display:flex; align-items:center; gap:.75rem; margin-bottom:1rem; flex-wrap:wrap; }
    .q-number { font-weight:900; font-size:1.2rem; color:#6366f1; }
    .q-points { margin-inline-start:auto; font-size:.85rem; color:#6b7280; font-weight:600; }
    .flag-btn {
      width:36px; height:36px; border-radius:8px; display:flex;
      align-items:center; justify-content:center; color:var(--gray-400);
      transition: all 0.2s; min-height:36px;
      &:hover { background:var(--gray-100); }
      &.flagged { color:var(--warning); background:rgba(245,158,11,0.1); }
    }
    .q-text {
      font-size: var(--text-question, 22px); font-weight:700;
      color:#1f2937; line-height:1.6; margin-bottom:1.5rem;
    }
    .q-image { max-height:200px; border-radius:12px; margin-bottom:1.5rem; }

    /* Options */
    .options-list { display:flex; flex-direction:column; gap:.6rem; margin-bottom:2rem; }
    .option-item {
      display:flex; align-items:center; gap:.75rem;
      padding:1rem 1.25rem; border:2px solid #e5e7eb; border-radius:14px;
      cursor:pointer; transition:all .2s; min-height:64px;
      &:hover { border-color:#a5b4fc; background:#f5f3ff; }
      &.selected {
        border-color:#6366f1; background:#eef2ff;
        .option-letter { background:#6366f1; color:white; }
      }
      &:active { transform:scale(0.98); }
    }
    .option-letter {
      min-width:36px; height:36px; border-radius:50%;
      background:var(--gray-200); display:flex; align-items:center;
      justify-content:center; font-weight:700; font-size:0.9rem;
      transition: all 0.2s; flex-shrink:0;
    }
    .option-text { font-size: var(--text-answer, 19px); color:#374151; font-weight:500; }

    /* True/False */
    .tf-container { display:grid; grid-template-columns:1fr 1fr; gap:16px; margin-bottom:2rem; }
    .tf-btn {
      min-height:100px; border-radius:20px; border:3px solid #e5e7eb;
      font-size:28px; font-weight:700; display:flex; flex-direction:column;
      align-items:center; justify-content:center; gap:8px; cursor:pointer;
      transition:all .2s; background:white;
      .tf-icon { font-size:40px; }
      .tf-label { font-size:20px; }
      &:active { transform:scale(0.96); }
      &.true-btn.selected  { background:#DCFCE7; border-color:#22C55E; color:#166534; }
      &.false-btn.selected { background:#FEE2E2; border-color:#EF4444; color:#991B1B; }
    }

    /* Fill input */
    .fill-input {
      width:100%; padding:14px; border:2px solid #e5e7eb; border-radius:12px;
      font-size:18px; transition:border-color .2s;
      &:focus { border-color:#6366f1; outline:none; }
    }

    /* Writing */
    .writing-group { margin-bottom:2rem; }
    .writing-area {
      width:100%; min-height:140px; resize:vertical; border:2px solid #e5e7eb;
      border-radius:12px; padding:14px; font-size:16px; line-height:1.6;
      transition:border-color .2s;
      &:focus { border-color:#6366f1; outline:none; }
    }
    .writing-footer { display:flex; justify-content:space-between; align-items:center; margin-top:8px; }
    .char-counter {
      font-size:13px; color:#9ca3af; font-variant-numeric:tabular-nums;
      &.near { color:#f59e0b; }
      &.limit { color:#ef4444; font-weight:700; }
    }
    .writing-note { font-size:12px; color:#6b7280; }

    /* Navigation */
    .q-nav { display:flex; justify-content:space-between; margin-top:2rem; padding-top:1.5rem; border-top:1px solid #f3f4f6; gap:0.75rem; }
    .q-dots {
      display:flex; flex-wrap:wrap; gap:.4rem; justify-content:center; margin-top:1.5rem;
      @media(max-width:600px) { gap:.3rem; }
    }
    .dot {
      width:34px; height:34px; border-radius:8px; border:2px solid #e5e7eb;
      background:white; font-size:.75rem; font-weight:700; cursor:pointer;
      transition:all .2s; color:#6b7280; display:flex; align-items:center;
      justify-content:center; min-height:34px;
      &.current { border-color:#6366f1; background:#eef2ff; color:#6366f1; }
      &.answered { background:#6366f1; color:white; border-color:#6366f1; }
      &.flagged { border-color:var(--warning); &.answered { box-shadow: 0 0 0 2px var(--warning); } }
    }

    /* Overview Panel */
    .overview-backdrop {
      position:fixed; inset:0; background:rgba(0,0,0,0.3); z-index:100;
    }
    .overview-panel {
      position:fixed; top:50%; left:50%; transform:translate(-50%,-50%);
      background:white; border-radius:20px; padding:1.5rem; z-index:101;
      width:min(380px, 90vw); box-shadow:0 20px 60px rgba(0,0,0,0.2);
      animation: scaleIn 0.2s ease;
    }
    .overview-header {
      display:flex; justify-content:space-between; align-items:center;
      margin-bottom:1rem;
      h3 { font-size:1.1rem; font-weight:700; }
      button { width:32px; height:32px; border-radius:8px; display:flex;
        align-items:center; justify-content:center; color:var(--gray-500);
        min-height:32px; &:hover { background:var(--gray-100); }
      }
    }
    .overview-grid {
      display:grid; grid-template-columns:repeat(6,1fr); gap:8px;
    }
    .overview-dot {
      width:100%; aspect-ratio:1; border-radius:10px; border:2px solid #e5e7eb;
      background:white; font-weight:700; font-size:.85rem; cursor:pointer;
      transition:all .2s; color:#6b7280; min-height:auto;
      &.current { border-color:#6366f1; background:#eef2ff; color:#6366f1; }
      &.answered { background:#6366f1; color:white; border-color:#6366f1; }
      &.flagged { border-color:var(--warning); }
    }
    .overview-legend {
      display:flex; gap:1rem; justify-content:center; margin-top:1rem;
      font-size:.8rem; color:var(--gray-500);
      span { display:flex; align-items:center; gap:4px; }
    }
    .legend-dot {
      width:12px; height:12px; border-radius:4px; border:2px solid #e5e7eb; background:white;
      &.answered { background:#6366f1; border-color:#6366f1; }
      &.flagged { border-color:var(--warning); }
    }

    /* Dialogs */
    .dialog-backdrop {
      position:fixed; inset:0; background:rgba(0,0,0,0.5); z-index:1000;
      display:flex; align-items:center; justify-content:center; padding:1rem;
    }
    .dialog {
      background:white; border-radius:20px; padding:2rem; text-align:center;
      max-width:400px; width:100%; animation: scaleIn 0.3s ease;
    }
    .dialog-icon { font-size:3rem; margin-bottom:1rem; }
    .dialog h3 { font-size:1.2rem; font-weight:700; margin-bottom:0.5rem; }
    .dialog p { color:var(--gray-500); margin-bottom:1.5rem; }
    .dialog-actions { display:flex; gap:0.75rem; justify-content:center; flex-wrap:wrap; }

    @keyframes pulse {
      0%, 100% { transform: scale(1); }
      50% { transform: scale(1.05); }
    }
    @keyframes shake {
      0%, 100% { transform: translateX(0); }
      25% { transform: translateX(-3px); }
      75% { transform: translateX(3px); }
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
  flaggedQuestions = new Set<number>();
  showOverview = false;
  showSubmitDialog = false;
  showLeaveDialog = false;
  optionLetters = ['A', 'B', 'C', 'D', 'E', 'F'];
  private timer: any;
  private questionStartTime = Date.now();

  currentQuestion = computed(() => this.attempt()?.questions[this.currentIndex()] || null);

  timerState = computed(() => {
    const t = this.timeRemaining();
    if (t <= 60) return 'danger';
    if (t <= 300) return 'warning';
    return 'normal';
  });

  unansweredCount = computed(() => {
    const total = this.attempt()?.questions.length || 0;
    const answered = Object.keys(this.answers).length;
    return total - answered;
  });

  constructor(private api: ApiService, private route: ActivatedRoute, private router: Router) {}

  // Protect against accidental back navigation during test
  @HostListener('window:popstate')
  onPopState() {
    if (this.attempt()) {
      this.showLeaveDialog = true;
      history.pushState(null, '', location.href);
    }
  }

  // Warn before closing tab/browser
  @HostListener('window:beforeunload', ['$event'])
  onBeforeUnload(event: BeforeUnloadEvent) {
    if (this.attempt()) {
      event.preventDefault();
      event.returnValue = '';
    }
  }

  ngOnInit() {
    // Push state to intercept back button
    history.pushState(null, '', location.href);

    const testId = +this.route.snapshot.params['id'];

    // Try to restore answers from localStorage
    this.restoreFromLocalStorage(testId);

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
    this.saveToLocalStorage();
  }

  isOptionSelected(optionId: number): boolean {
    return this.answers[this.currentIndex()]?.selectedOptionIds?.includes(optionId) || false;
  }

  selectTrueFalse(value: 'True' | 'False') {
    const q = this.currentQuestion();
    if (!q) return;
    const opt = q.options.find(o => o.optionText === value);
    if (opt) {
      this.answers[this.currentIndex()] = { questionId: q.id, selectedOptionIds: [opt.id], timeSpentSeconds: this.getTimeSpent() };
      this.saveToLocalStorage();
    }
  }

  isTrueFalseSelected(value: string): boolean {
    const q = this.currentQuestion();
    if (!q) return false;
    const opt = q.options.find(o => o.optionText === value);
    return opt ? this.isOptionSelected(opt.id) : false;
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

  toggleFlag() {
    if (this.flaggedQuestions.has(this.currentIndex())) {
      this.flaggedQuestions.delete(this.currentIndex());
    } else {
      this.flaggedQuestions.add(this.currentIndex());
    }
  }

  confirmSubmit() {
    this.saveCurrentAnswer();
    this.showSubmitDialog = true;
  }

  confirmLeave() {
    this.showLeaveDialog = false;
    this.router.navigate(['/tests']);
  }

  submitTest() {
    this.saveCurrentAnswer();
    this.showSubmitDialog = false;
    const a = this.attempt();
    if (!a) return;
    const submission: SubmitAttempt = {
      attemptId: a.attemptId, testId: a.testId,
      answers: Object.values(this.answers)
    };
    this.api.submitTest(a.testId, submission).subscribe(res => {
      if (res.success) {
        this.clearLocalStorage(a.testId);
        this.router.navigate(['/tests', a.testId, 'result', a.attemptId]);
      }
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

  saveCurrentAnswer() {
    const q = this.currentQuestion();
    if (!q) return;
    if (!this.answers[this.currentIndex()] && this.textAnswer) {
      this.answers[this.currentIndex()] = { questionId: q.id, answerText: this.textAnswer, timeSpentSeconds: this.getTimeSpent() };
    }
    if (this.answers[this.currentIndex()]) {
      this.answers[this.currentIndex()].timeSpentSeconds = this.getTimeSpent();
      if (this.textAnswer) this.answers[this.currentIndex()].answerText = this.textAnswer;
    }
    this.saveToLocalStorage();
  }

  private loadExistingAnswer() {
    const existing = this.answers[this.currentIndex()];
    this.textAnswer = existing?.answerText || '';
  }

  private getTimeSpent(): number { return Math.floor((Date.now() - this.questionStartTime) / 1000); }

  // --- LocalStorage auto-save ---
  private saveToLocalStorage() {
    try {
      const testId = this.attempt()?.testId;
      if (testId) {
        localStorage.setItem(`ewd_test_${testId}`, JSON.stringify(this.answers));
      }
    } catch {}
  }

  private restoreFromLocalStorage(testId: number) {
    try {
      const saved = localStorage.getItem(`ewd_test_${testId}`);
      if (saved) {
        this.answers = JSON.parse(saved);
      }
    } catch {}
  }

  private clearLocalStorage(testId: number) {
    try { localStorage.removeItem(`ewd_test_${testId}`); } catch {}
  }
}
