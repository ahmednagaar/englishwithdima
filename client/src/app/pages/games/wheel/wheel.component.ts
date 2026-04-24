import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { ApiService } from '../../../core/services/api.service';
import { Grade } from '../../../core/models';

@Component({
  selector: 'app-wheel',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TranslateModule],
  template: `
    <section class="game-page">
      <div class="container">
        @if (!started()) {
          <!-- Grade Selection -->
          <div class="wheel-setup animate-scale-in">
            <span class="setup-icon">🎡</span>
            <h1>{{ 'GAMES.WHEEL' | translate }}</h1>
            <p>{{ 'GAMES.SPIN_DESC' | translate }}</p>
            <div class="form-group" style="max-width:300px;margin:1.5rem auto">
              <select [(ngModel)]="selectedGrade" name="grade">
                @for (g of grades(); track g.id) { <option [ngValue]="g.id">{{ g.nameAr }}</option> }
              </select>
            </div>
            <button class="btn btn-primary btn-lg" (click)="startGame()">{{ 'GAMES.START_SPINNING' | translate }}</button>
          </div>
        } @else if (gameOver()) {
          <div class="result-card animate-scale-in">
            <span class="result-emoji">{{ score() >= 500 ? '🏆' : '⭐' }}</span>
            <h2>{{ 'GAMES.GAME_OVER' | translate }}</h2>
            <div class="final-score">{{ score() }}</div>
            <p>{{ 'GAMES.CORRECT_OUT_OF' | translate:{ correct: correctCount(), total: totalSpins() } }}</p>
            <div class="result-btns">
              <button class="btn btn-primary" (click)="restartGame()">{{ 'GAMES.PLAY_AGAIN' | translate }}</button>
              <a routerLink="/games" class="btn btn-secondary">{{ 'COMMON.BACK' | translate }}</a>
            </div>
          </div>
        } @else {
          <div class="game-header">
            <h1>🎡 {{ 'GAMES.WHEEL' | translate }}</h1>
            <div class="game-info">
              <span class="game-score">{{ 'GAMES.SCORE' | translate }}: {{ score() }}</span>
              <span>{{ 'GAMES.ROUND' | translate }} {{ currentRound() }} / {{ totalSpins() }}</span>
            </div>
          </div>

          <div class="wheel-area">
            <!-- Wheel Visual -->
            <div class="wheel-container" (click)="spin()">
              <div class="wheel" [style.transform]="'rotate(' + wheelAngle() + 'deg)'" [class.spinning]="spinning()">
                @for (cat of categories; track cat.labelKey; let i = $index) {
                  <div class="wheel-segment" [style.transform]="'rotate(' + (i * 360 / categories.length) + 'deg)'" [style.background]="cat.color">
                    <span class="seg-label">{{ cat.icon }}</span>
                  </div>
                }
              </div>
              <div class="wheel-pointer">▼</div>
              <div class="wheel-center" [class.clickable]="!spinning()">
                {{ spinning() ? '🎡' : ('GAMES.SPIN' | translate) }}
              </div>
            </div>

            <!-- Question Area -->
            @if (currentQuestion()) {
              <div class="question-area animate-fade-in">
                <div class="q-category"><span [style.color]="currentCategory().color">{{ currentCategory().icon }} {{ currentCategory().labelKey | translate }}</span></div>
                <h2 class="q-text">{{ currentQuestion()!.questionText }}</h2>
                @if (currentQuestion()!.options?.length) {
                  <div class="options-grid">
                    @for (opt of currentQuestion()!.options; track opt.id) {
                      <button class="wheel-option" [class.selected]="selectedOption() === opt.id" [class.correct]="answered() && opt.isCorrect" [class.wrong]="answered() && selectedOption() === opt.id && !opt.isCorrect" (click)="answerQuestion(opt.id)" [disabled]="answered()">
                        {{ opt.optionText }}
                      </button>
                    }
                  </div>
                }
                @if (answered()) {
                  <div class="feedback" [class.correct]="lastCorrect()">{{ lastCorrect() ? ('GAMES.CORRECT_ANSWER' | translate) : ('GAMES.WRONG_ANSWER' | translate) }}</div>
                  <button class="btn btn-primary" (click)="nextRound()">{{ currentRound() < totalSpins() ? ('GAMES.NEXT_ROUND' | translate) : ('GAMES.SEE_RESULTS' | translate) }}</button>
                }
              </div>
            }
          </div>
        }
      </div>
    </section>
  `,
  styles: [`
    .game-page { padding:2rem 0 5rem; }
    .wheel-setup { text-align:center; padding:4rem 2rem; .setup-icon { font-size:5rem; display:block; margin-bottom:1rem; animation:float 3s ease-in-out infinite; } h1 { font-size:2rem; font-weight:900; margin-bottom:.5rem; } p { color:#6b7280; margin-bottom:.5rem; } }
    .game-header { display:flex; justify-content:space-between; align-items:center; margin-bottom:2rem; h1 { font-size:1.5rem; font-weight:800; } }
    .game-info { display:flex; gap:1.5rem; align-items:center; .game-score { font-weight:800; color:#a855f7; font-size:1.2rem; } }
    .wheel-area { display:grid; grid-template-columns:1fr 1fr; gap:3rem; align-items:start; @media(max-width:768px){ grid-template-columns:1fr; } }
    .wheel-container { position:relative; width:280px; height:280px; margin:0 auto; cursor:pointer; }
    .wheel { width:100%; height:100%; border-radius:50%; position:relative; overflow:hidden; border:6px solid #1e1b4b; transition:transform 4s cubic-bezier(0.17,0.67,0.12,0.99);
      &.spinning { transition:transform 4s cubic-bezier(0.17,0.67,0.12,0.99); }
    }
    .wheel-segment { position:absolute; width:50%; height:50%; top:0; left:50%; transform-origin:0% 100%; display:flex; align-items:center; justify-content:center;
      .seg-label { font-size:1.5rem; transform:rotate(22.5deg) translateY(-20px); }
    }
    .wheel-pointer { position:absolute; top:-16px; left:50%; transform:translateX(-50%); font-size:1.5rem; color:#1e1b4b; z-index:5; }
    .wheel-center { position:absolute; top:50%; left:50%; transform:translate(-50%,-50%); width:60px; height:60px; border-radius:50%; background:white; display:flex; align-items:center; justify-content:center; font-weight:900; font-size:.85rem; color:#6366f1; box-shadow:0 2px 10px rgba(0,0,0,.15); z-index:5;
      &.clickable { cursor:pointer; &:hover { transform:translate(-50%,-50%) scale(1.1); } }
    }
    .question-area { background:white; border-radius:20px; padding:2rem; box-shadow:0 4px 20px rgba(0,0,0,.06); }
    .q-category { margin-bottom:.75rem; span { font-weight:700; font-size:1rem; } }
    .q-text { font-size:1.2rem; font-weight:700; line-height:1.6; margin-bottom:1.5rem; }
    .options-grid { display:grid; grid-template-columns:1fr 1fr; gap:.75rem; margin-bottom:1.5rem; }
    .wheel-option { padding:1rem; border:2px solid #e5e7eb; border-radius:12px; background:white; font-weight:600; font-size:.95rem; cursor:pointer; transition:all .2s; text-align:center;
      &:hover:not(:disabled) { border-color:#a855f7; background:#faf5ff; }
      &.selected { border-color:#a855f7; background:#f5f3ff; }
      &.correct { border-color:#10b981; background:#d1fae5; color:#059669; }
      &.wrong { border-color:#ef4444; background:#fee2e2; color:#dc2626; }
    }
    .feedback { text-align:center; font-weight:700; font-size:1.1rem; margin-bottom:1rem; &.correct { color:#10b981; } &:not(.correct) { color:#ef4444; } }
    .result-card { max-width:400px; margin:4rem auto; text-align:center; background:white; border-radius:24px; padding:3rem; box-shadow:0 10px 40px rgba(0,0,0,.08);
      .result-emoji { font-size:5rem; display:block; margin-bottom:1rem; animation:float 2s ease-in-out infinite; }
      h2 { font-size:1.5rem; font-weight:800; margin-bottom:1rem; }
      .final-score { font-size:3rem; font-weight:900; color:#a855f7; }
      p { color:#6b7280; margin-bottom:1.5rem; }
      .result-btns { display:flex; gap:1rem; justify-content:center; }
    }
  `]
})
export class WheelComponent implements OnInit {
  grades = signal<Grade[]>([]);
  selectedGrade = 1;
  started = signal(false);
  gameOver = signal(false);
  spinning = signal(false);
  answered = signal(false);
  lastCorrect = signal(false);
  score = signal(0);
  currentRound = signal(0);
  totalSpins = signal(8);
  correctCount = signal(0);
  wheelAngle = signal(0);
  selectedOption = signal<number | null>(null);
  currentQuestion = signal<any>(null);
  private sessionId = 0;

  categories = [
    { labelKey: 'GAMES.GRAMMAR', icon: '📖', color: '#6366f1' }, { labelKey: 'GAMES.VOCABULARY', icon: '📝', color: '#a855f7' },
    { labelKey: 'GAMES.READING', icon: '📚', color: '#06b6d4' }, { labelKey: 'GAMES.LISTENING', icon: '🎧', color: '#10b981' },
    { labelKey: 'GAMES.SPELLING', icon: '✏️', color: '#f59e0b' }, { labelKey: 'GAMES.PHONICS', icon: '🔤', color: '#ef4444' },
    { labelKey: 'GAMES.WRITING', icon: '✍️', color: '#ec4899' }, { labelKey: 'GAMES.BONUS', icon: '⭐', color: '#8b5cf6' },
  ];

  currentCategory = signal(this.categories[0]);

  constructor(private api: ApiService) {}
  ngOnInit() { this.api.getGrades().subscribe(r => { if (r.success) this.grades.set(r.data); }); }

  startGame() {
    this.started.set(true);
    this.api.startWheelGame(this.selectedGrade).subscribe(res => {
      if (res.success) { this.sessionId = res.data.sessionId; this.totalSpins.set(res.data.totalRounds || 8); }
    });
  }

  spin() {
    if (this.spinning()) return;
    this.spinning.set(true);
    this.answered.set(false);
    this.selectedOption.set(null);
    this.currentQuestion.set(null);
    const extraSpins = 1440 + Math.random() * 720;
    this.wheelAngle.set(this.wheelAngle() + extraSpins);
    const catIndex = Math.floor(Math.random() * this.categories.length);
    this.currentCategory.set(this.categories[catIndex]);

    setTimeout(() => {
      this.spinning.set(false);
      this.currentRound.set(this.currentRound() + 1);
      this.api.spinWheel(this.sessionId).subscribe(res => {
        if (res.success && res.data) { this.currentQuestion.set(res.data); }
        else { this.currentQuestion.set({ questionText: 'ما هو الماضي البسيط لفعل "go"؟', options: [{ id: 1, optionText: 'went', isCorrect: true }, { id: 2, optionText: 'goed', isCorrect: false }, { id: 3, optionText: 'gone', isCorrect: false }, { id: 4, optionText: 'going', isCorrect: false }] }); }
      });
    }, 4000);
  }

  answerQuestion(optId: number) {
    if (this.answered()) return;
    this.selectedOption.set(optId);
    this.answered.set(true);
    const opt = this.currentQuestion()?.options?.find((o: any) => o.id === optId);
    this.lastCorrect.set(opt?.isCorrect || false);
    if (opt?.isCorrect) { this.score.set(this.score() + 100); this.correctCount.set(this.correctCount() + 1); }

    // Send answer to backend
    if (this.sessionId) {
      this.api.answerWheel({
        sessionId: this.sessionId,
        questionAttemptId: this.currentQuestion()?.attemptId || 0,
        selectedOptionId: optId
      }).subscribe();
    }
  }

  nextRound() {
    if (this.currentRound() >= this.totalSpins()) {
      // End session on backend
      if (this.sessionId) {
        this.api.endWheelGame(this.sessionId).subscribe(res => {
          if (res.success && res.data) {
            this.score.set(res.data.totalScore);
          }
        });
      }
      this.gameOver.set(true);
      return;
    }
    this.spin();
  }

  restartGame() { this.started.set(false); this.gameOver.set(false); this.score.set(0); this.currentRound.set(0); this.correctCount.set(0); this.wheelAngle.set(0); }
}
